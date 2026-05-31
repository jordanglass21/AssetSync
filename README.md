# AssetSync
Jordan Glass

## The Problem
In large operations, data lives in silos. Modern systems handle daily operations while legacy databases maintain long-term audit and compliance records. When these two drift apart, it creates real operational and reporting problems.

This repo contains three services:
1. **AssetSync.Api** — A C# .NET 8 Web API backed by Entity Framework Core and SQLite.
2. **AssetSync.Legacy** — A Java 21 Spring Boot app that simulates an isolated legacy database.
3. **AssetSync.Client** — A React + TypeScript frontend dashboard.

The C# engine calls the Java server, pulls its data, and runs a row-by-row comparison against SQLite to find discrepancies.

## Data Source
Montgomery County, Maryland's public Warehouse and Retail Sales dataset (via Data Montgomery / Kaggle). Real monthly inventory records covering retail sales, retail transfers, and warehouse sales for a beverage distribution operation.

## Running Locally

From the repo root:

```bash
./start.sh
```

This starts all three services and clears any occupied ports automatically.

- Legacy simulator: http://localhost:8080
- Modern engine: http://localhost:5289
- Frontend: http://localhost:5173

## Endpoints

**AssetSync.Api**
- `GET /api/reconciliation/run-audit` — Runs the audit and returns all detected discrepancies.
- `GET /api/testconnection/check-data` — Returns 5 rows from SQLite to verify the database connection.

**AssetSync.Legacy**
- `GET /api/legacy/sales` — Returns the full legacy dataset.
- `POST /api/legacy/chaos?count=10` — Corrupts n rows in memory to simulate data drift.
- `POST /api/legacy/reset` — Resets legacy data back to the clean CSV state.

## Test Script
`test-reconciliation.sh` runs a full end-to-end audit cycle — baseline check, chaos injection, detection verification, and reset. Both backend servers need to be running first.

```bash
./test-reconciliation.sh
```

## Roadmap

| Phase | Title | Objective |
| :--- | :--- | :--- |
| 1 | Foundation & Setup | Project structure and baseline C# API. |
| 2 | Data Modeling | Map the WarehouseSales schema using EF Core. |
| 3 | Reconciliation Logic | Row-by-row comparison engine and discrepancy reporting. |
| 4 | Legacy Integration | Java Spring Boot legacy simulator with chaos and reset controls. |
| 5 | Frontend | Dashboard UI to run audits, visualize discrepancies, and trigger chaos. |
| 6 | Deployment | Host and deploy all three services. |