# Deployment

## Free-host setup

The repository now includes a Docker-based deployment path for Render:

- `CarSalesSystem/Dockerfile`
- `render.yaml`

The app can now boot with either:

- SQL Server connection strings
- SQLite connection strings such as `Data Source=/app/App_Data/carsales.db`

At startup the app applies EF Core migrations automatically, then runs role seeding.

## Recommended demo deployment

Use Render's free web service with the included `render.yaml`.

Environment values:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://0.0.0.0:10000`
- `ConnectionStrings__DefaultConnection=Data Source=/app/App_Data/carsales.db`

## Important limitation

The included free-host configuration uses a local SQLite file inside the container. That is good for a demo deployment, but data can be lost when the service is rebuilt or restarted because free stateless hosting does not guarantee persistent local storage.

For a durable production-style deployment, move to:

- a persistent disk on the host, or
- a managed external database

## Publish steps

1. Push this repository to GitHub.
2. Create a new Render Web Service from the repo.
3. Let Render detect `render.yaml`.
4. Deploy the service.
5. Open the app and create the first admin account using the configured `SeedData:AdminEmail`.

## Local database repair note

If your current local SQL Server database still has the old `Payments.DebitCardId` shape, run the latest migration against the same database your app actually uses. The new repair migration is:

- `20260409110000_MarketplaceUpgradeRepair`
