package com.assetsync.legacy;

import com.fasterxml.jackson.annotation.JsonProperty;

public record LegacyItem(
        @JsonProperty("year") int year,
        @JsonProperty("month") int month,
        @JsonProperty("itemCode") String itemCode,
        @JsonProperty("itemDescription") String itemDescription,
        @JsonProperty("retailSales") double retailSales,
        @JsonProperty("retailTransfers") double retailTransfers,
        @JsonProperty("warehouseSales") double warehouseSales
) {}