package com.assetsync.legacy;

import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/legacy")
public class LegacyDataController {

    private final LegacyDataService legacyDataService;

    // This is where we wire up the dependency injection
    // Spring Boot automatically hands this controller the active service
    public LegacyDataController(LegacyDataService legacyDataService) {
        this.legacyDataService = legacyDataService;
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

}