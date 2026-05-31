# AssetSync
Jordan Glass

## The Problem
In large operations, data lives in silos. Modern systems handle daily operations while legacy databases maintain long-term audit and compliance records. When these two drift apart, it creates real operational and reporting problems.

This repo contains two services:
1. **AssetSync.Api** — A C# .NET 8 Web API backed by Entity Framework Core and SQLite.
2. **AssetSync.Legacy** — A Java 21 Spring Boot app that simulates an isolated legacy database.

The C# engine calls the Java server, pulls its data, and runs a row-by-row comparison against SQLite to surface discrepancies.

## Data Source
Montgomery County, Maryland's public Warehouse and Retail Sales dataset (via Data Montgomery / Kaggle). Real monthly inventory records covering retail sales, retail transfers, and warehouse sales for a beverage distribution operation.

## Running Locally

**1. Start the legacy simulator**
```bash
cd AssetSync.Legacy
./mvnw spring-boot:run
```
Runs on http://localhost:8080

**2. Start the modern engine**
```bash
cd AssetSync.Api
dotnet run
```
Runs on http://localhost:5289

## Endpoints

**AssetSync.Api**
- `GET /api/reconciliation/run-audit` — Runs the audit and returns all detected discrepancies.
- `GET /api/testconnection/check-data` — Returns 5 rows from SQLite to verify the database connection.

**AssetSync.Legacy**
- `GET /api/legacy/sales` — Returns the full legacy dataset.
- `POST /api/legacy/chaos?count=200` — Corrupts n rows in memory to simulate data drift.
- `POST /api/legacy/reset` — Resets legacy data back to the clean CSV state.

## Test Script
`test-reconciliation.sh` in the repo root runs a full end-to-end audit cycle — baseline check, chaos injection, detection verification, and reset.

```bash
./test-reconciliation.sh
```

Both servers need to be running first.

## Roadmap

| Phase | Title | Objective |
| :--- | :--- | :--- |
| 1 | Foundation & Setup | Project structure and baseline C# API. |
| 2 | Data Modeling | Map the WarehouseSales schema using EF Core. |
| 3 | Reconciliation Logic | Row-by-row comparison engine and discrepancy reporting. |
| 4 | Legacy Integration | Java Spring Boot legacy simulator with chaos and reset controls. |
| 5 | Frontend | Dashboard UI to run audits, visualize discrepancies, and trigger chaos. |
| 6 | Deployment | Host and deploy both services. |