package com.assetsync.legacy;

import jakarta.annotation.PostConstruct;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;

/**
 * This parser is intentionally hardcoded to process a specific 9-column retail/warehouse
 * schema. To make this fully production-ready and agnostic, this service should be refactored
 * to accept a dynamic configuration object or properties mapping (e.g., column indices,
 * expected row length, and target data types). This would allow the engine to map any arbitrary
 * CSV to the target domain models without rewriting core parsing logic.
 *
 * Columns at index 2 (Supplier) and index 5 (Item Type) are explicitly ignored during
 * ingestion. Because this service acts purely as a deterministic data source for a mathematical
 * reconciliation audit, transmitting non-audited metadata introduces unnecessary network
 * payload overhead, serialization latency, and memory bloat at scale (300k+ rows).
 */
@Service
public class LegacyDataService {

    private static final Logger log = LoggerFactory.getLogger(LegacyDataService.class);

    private final List<LegacyItem> legacyData = new ArrayList<>();

    @PostConstruct
    public void loadCsvData() {
        try (var inputStream = getClass().getResourceAsStream("/legacy-sales.csv");
             var reader = new BufferedReader(new InputStreamReader(inputStream))) {

            reader.readLine(); // skip header

            String line;
            while ((line = reader.readLine()) != null) {
                if (line.trim().isEmpty()) {
                    continue;
                }

                // split by comma, ignoring commas within quotes
                String[] data = line.split(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                if (data.length == 9) {
                    try {
                        legacyData.add(new LegacyItem(
                                parseIntSafe(data[0]), // year
                                parseIntSafe(data[1]), // month
                                // index 2 is supplier
                                cleanString(data[3]), // item code
                                cleanString(data[4]), // item desc
                                // index 5 is item type
                                parseDoubleSafe(data[6]), // retail sales
                                parseDoubleSafe(data[7]), // retial transfers
                                parseDoubleSafe(data[8]) // warehouse sales
                        ));
                    } catch (Exception e) {
                        log.warn("Failed to parse row data: {}", line);
                    }
                } else {
                    log.warn("Skipping invalid row. Expected 9 columns, got {}: {}", data.length, line);
                }
            }

            log.info("Loaded {} legacy records into memory.", legacyData.size());

        } catch (Exception e) {
            log.error("Could not load legacy-sales.csv on startup", e);
            throw new RuntimeException("CSV load failed", e);
        }
    }

    private String cleanString(String val) {
        if (val == null) {
            return "";
        }
        return val.replace("\"", "").trim();
    }

    private double parseDoubleSafe(String val) {
        if (val == null || val.trim().isEmpty()) {
            return 0.0;
        }
        return Double.parseDouble(val.trim());
    }

    private int parseIntSafe(String val) {
        if (val == null || val.trim().isEmpty()) {
            return 0;
        }
        return Integer.parseInt(val.trim());
    }

    public List<LegacyItem> getAllData() {
        return legacyData;
    }

    /**
     * Randomly corrupts a specified number of rows to trigger C# audit discrepancies.
     */
    public int injectChaos(int count) {
        if (legacyData.isEmpty()) return 0;

        List<LegacyItem> scope = new ArrayList<>(getScopedData());
        java.util.Collections.shuffle(scope);

        List<LegacyItem> targets = scope.subList(0, Math.min(count, scope.size()));
        java.util.Random rand = new java.util.Random();
        int[] offsets = {1, 2, 6, 12};
        String[] metrics = {"retailSales", "retailTransfers", "warehouseSales"};

        for (LegacyItem item : targets) {
            String metric = metrics[rand.nextInt(metrics.length)];
            double offset = offsets[rand.nextInt(offsets.length)];
            if (rand.nextBoolean()) offset = -offset;

            LegacyItem corrupted = new LegacyItem(
                    item.year(), item.month(), item.itemCode(), item.itemDescription(),
                    metric.equals("retailSales") ? item.retailSales() + offset : item.retailSales(),
                    metric.equals("retailTransfers") ? item.retailTransfers() + offset : item.retailTransfers(),
                    metric.equals("warehouseSales") ? item.warehouseSales() + offset : item.warehouseSales()
            );

            for (int j = 0; j < legacyData.size(); j++) {
                LegacyItem g = legacyData.get(j);
                if (g.itemCode().equals(item.itemCode()) && g.year() == item.year() && g.month() == item.month()) {
                    legacyData.set(j, corrupted);
                    break;
                }
            }
        }

        log.warn("Chaos injected: corrupted {} rows.", targets.size());
        return targets.size();
    }

    public int chaosTargeted(String targetItemCode) {
        if (legacyData.isEmpty()) return 0;

        // TODO: clean this logic up a bit...
        List<LegacyItem> scope = new ArrayList<>(getScopedData());
        List<LegacyItem> targets = scope.subList(0, new ArrayList<>(getScopedData()).size());

        java.util.Random rand = new java.util.Random();
        String[] metrics = {"retailSales", "retailTransfers", "warehouseSales"};
        int[] offsets = {1, 2, 6, 12};
        int count = 0;

        for (LegacyItem item : targets) {
            if (item.itemCode().equals(targetItemCode)) {
                String metric = metrics[rand.nextInt(metrics.length)];
                double offset = offsets[rand.nextInt(offsets.length)];
                if (rand.nextBoolean()) offset = -offset;

                LegacyItem corrupted = new LegacyItem(
                        item.year(), item.month(), item.itemCode(), item.itemDescription(),
                        metric.equals("retailSales") ? item.retailSales() + offset : item.retailSales(),
                        metric.equals("retailTransfers") ? item.retailTransfers() + offset : item.retailTransfers(),
                        metric.equals("warehouseSales") ? item.warehouseSales() + offset : item.warehouseSales()
                );
                count ++;

                for (int j = 0; j < legacyData.size(); j++) {
                    LegacyItem g = legacyData.get(j);
                    if (g.itemCode().equals(item.itemCode()) && g.year() == item.year() && g.month() == item.month()) {
                        legacyData.set(j, corrupted);
                        break;
                    }
                }
            }
        }
        log.warn("Chaos injected: corrupted {} rows.", count);
        return count;
    }

    /**
     * Wipes the corrupted memory and reloads the clean CSV.
     */
    public void resetData() {
        legacyData.clear();
        loadCsvData();
        log.info("DATA RESET: Legacy memory wiped and reloaded from CSV.");
    }

    @Value("${app.data.use-subset}")
    private boolean useSubset;
    @Value("${app.data.subset-size}")
    private int subsetSize;
    /**
     * Scopes the legacy data to a subset for rapid UI development or returns the full set for production audits.
     * Sorting by ItemCode is enforced to ensure consistent, deterministic reconciliation across services.
     */
    private List<LegacyItem> getScopedData() {
        if (useSubset) {
            return legacyData.stream()
                    .sorted(Comparator.comparing(LegacyItem::itemCode))
                    .limit(subsetSize)
                    .toList();
        }
        return legacyData.stream()
                .sorted(Comparator.comparing(LegacyItem::itemCode))
                .toList();
    }
}