# Supabase-Only Database Setup

This project is now configured to run only against Supabase PostgreSQL.

## Runtime expectation

The app will start only when `ConnectionStrings__DefaultConnection` is a PostgreSQL/Supabase connection string.

Supported shapes:

Recommended for this project, especially when your password contains `/`, `%`, `@`, or `:`:

```text
Host=db.<project-ref>.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=<password>;SSL Mode=Require
```

or

```text
postgresql://postgres:<password>@db.<project-ref>.supabase.co:5432/postgres
```

If you use the URI form and the password contains reserved URL characters, percent-encode the password first. Otherwise parsing will break before the app even reaches PostgreSQL.

If the connection string does not look like PostgreSQL, startup throws immediately.

## What was removed

- SQLite fallback logic
- SQL Server fallback logic
- SQLite connection defaults from Docker/Render config

## Current schema behavior

For a fresh Supabase database, startup uses:

```csharp
Database.EnsureCreatedAsync()
```

This is deliberate because the repository contains older SQL Server-specific and SQLite-specific migration history that is not safe to replay against PostgreSQL as-is.

## Render setup

In Render, set this environment variable manually:

```text
ConnectionStrings__DefaultConnection=Host=db.<project-ref>.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=<password>;SSL Mode=Require
```

Do not use the old SQLite path.

## IPv6 vs pooler

Supabase direct `db.<project-ref>.supabase.co:5432` hosts commonly resolve to IPv6. If your local machine or hosting environment does not support IPv6, use the Supabase `Session pooler` connection string from the dashboard instead of the direct host.

## Next step if you want a cleaner PostgreSQL future

The correct long-term move is to create a PostgreSQL-only migration baseline and use PostgreSQL-native migrations from that point forward.

## Importing your existing SQL Server data

This repo now includes a one-shot importer that copies the old local SQL Server `CarSalesSystem` database into the configured Supabase/PostgreSQL database.

Default source:

```text
Server=.;Database=CarSalesSystem;Trusted_Connection=True;TrustServerCertificate=True;
```

Run it with:

```powershell
dotnet run --project CarSalesSystem\CarSalesSystem.csproj -- --import-sqlserver
```

If your source SQL Server connection is different:

```powershell
dotnet run --project CarSalesSystem\CarSalesSystem.csproj -- --import-sqlserver --source-connection="Server=...;Database=CarSalesSystem;Trusted_Connection=True;TrustServerCertificate=True;"
```

What it imports:

- ASP.NET Identity users, roles, claims, logins, tokens, and user-role links
- Categories
- Dealers
- Cars
- Payments

Special case:

- Old SQL Server payments that still reference `DebitCards` are transformed into the current safe schema by storing only `CardLast4`, cardholder name, expiration month, and expiration year.
- The old `DebitCards` table is not copied.
