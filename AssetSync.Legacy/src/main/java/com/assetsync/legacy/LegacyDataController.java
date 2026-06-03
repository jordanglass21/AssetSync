package com.assetsync.legacy;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/legacy")
public class LegacyDataController {

    private final LegacyDataService legacyDataService;

    // This is where we wire up the dependency injection
    // Alternatively we can use @Autowire
    public LegacyDataController(LegacyDataService legacyDataService) {
        this.legacyDataService = legacyDataService; // Spring Boot automatically hands this controller the service

    }

    @GetMapping("/sales")
    public List<LegacyItem> getLegacySalesData() {
        return legacyDataService.getAllData();
    }

    @PostMapping("/chaos")
    public String injectChaos(@RequestParam(defaultValue = "10") int count) {
        int altered = legacyDataService.injectChaos(count);
        return "{\"status\": \"success\", \"message\": \"Corrupted " + altered + " rows.\"}";
    }

    @PostMapping("/reset")
    public String resetData() {
        legacyDataService.resetData();
        return "{\"status\": \"success\", \"message\": \"Data reset to clean CSV state.\"}";
    }

    @PostMapping("/chaos/targeted")
    public String chaosTargeted(@RequestParam String itemCode) {
        int altered = legacyDataService.chaosTargeted(itemCode);
        if (altered > 0) {
            return "{\"status\": \"success\", \"message\": \"Corrupted " + altered + " rows.\"}";
        }
        return "{\"status\": \"failure\", \"message\": \"Item Code " + itemCode + " not found.\"}";
    }

}