# AssetManagement
Managing corporate hardware (laptops, monitors, etc.) and their associated software licenses.

## Overview
This project provides a stateless ASP.NET Core API for managing corporate hardware assets, their warranty cards, assigned employees, software licenses, and asset statuses.

## Development Environment
- IDE: `Microsoft Visual Studio Enterprise 2026 (18.4.2)`
- Target framework: `.NET 10`

### Relationship Logic
- **Asset -> WarrantyCard**: One-to-one. Every asset must have exactly one warranty card.
- **Employee -> Assets**: One-to-many. One employee can be assigned multiple assets.
- **Asset -> SoftwareLicense**: Many-to-many via a join table.
- **Status -> Assets**: One-to-many. Each asset belongs to a status category (Available, Assigned, Retired).

## Run with Docker Compose
1. Start the environment:
   - `docker compose up --build`
2. The API will be available at `http://localhost:8080`.
3. Optional tools:
   - PgAdmin: `http://localhost:5050`
   - Angular UI: `http://localhost:4200`

## Apply EF Core Migrations
Run these commands from the repository root:
1. `dotnet tool install --global dotnet-ef` (once per machine)
2. `dotnet ef migrations add <MigrationName> --project AssetManagement --startup-project AssetManagement`
3. `dotnet ef database update --project AssetManagement --startup-project AssetManagement`

## API Endpoints
The API exposes CRUD endpoints for assets, employees, software licenses, and statuses. Assets include warranty card data and assigned licenses in the responses.

## Future Implementation
- Add logging and monitoring with Serilog and Application Insights.
- Add an authentication endpoint to issue JWTs.
- Rotate keys and move secrets to a managed vault (e.g., Azure Key Vault).
- Add refresh tokens and role management.
- Implement pagination and filtering for list endpoints.
- Implement caching for frequently accessed data (e.g., asset statuses).
