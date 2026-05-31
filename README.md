# Asset Synchronizer

## The Problem
In large retail operations, data often lives in silos. We have modern, high-speed systems for daily operations, but we also rely on legacy databases for long-term audit and compliance records. Keeping these two systems in sync is a major challenge. When they drift, it creates significant operational risks and reporting errors.

## The Goal
I am building a reconciliation engine to bridge this gap. This tool will automatically detect discrepancies between our modern operational environment and our legacy archive, ensuring that both systems act as a single, consistent source of truth.

## Tech Stack
- Modern Backend: C# / .NET / ASP.NET Core
- Legacy/Archive Backend: Java / Jakarta EE
- Data Layer: Azure SQL (EF Core) & Oracle
- Deployment: IIS
- Version Control: Git
  
## Data Source
The data driving this simulation is derived from the **Warehouse and Retail Sales** public dataset provided by the government of **Montgomery County, Maryland** (available via Data Montgomery / Kaggle). It contains authentic, monthly inventory movement records—including direct retail sales, warehouse bulk sales, and physical retail transfers for a high volume beverage distribution operation. Utilizing a real world municipal ledger provides a highly realistic, production-grade schema for our reconciliation engine to process.

## Project Roadmap

| Phase | Title | Objective |
| :--- | :--- | :--- |
| **1** | **Foundation & Setup** | Establish the project structure and baseline C# API. |
| **2** | **Data Modeling** | Map the `WarehouseSales` schema to C# using EF Core. |
| **3** | **The Reconciliation Logic** | Write the "brain" that compares two tables and flags discrepancies. |
| **4** | **Legacy Integration** | Create the Java/Jakarta EE skeleton to simulate the Oracle database. |
| **5** | **Automation & API** | Expose the reconciliation logic via REST endpoints and schedule checks. |
| **6** | **Deployment & Polish** | Configure IIS hosting and finalize README. |