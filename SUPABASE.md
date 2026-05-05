# Supabase Setup

This project now supports Supabase as a PostgreSQL database provider.

## What changed

- Added `Npgsql.EntityFrameworkCore.PostgreSQL`
- Added PostgreSQL provider detection in startup
- Fresh PostgreSQL/Supabase databases use `EnsureCreatedAsync()` instead of replaying the older mixed-provider migration history

## Connection string

Set the app connection string to the Supabase Postgres direct connection string.

Example environment variable:

```text
ConnectionStrings__DefaultConnection=Host=db.<project-ref>.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=<password>;SSL Mode=Require;Trust Server Certificate=true
```

## Recommended usage

- Use a fresh Supabase database for this app
- Let the app create the schema on first startup
- Keep ASP.NET Identity in the app; this change does not switch authentication to Supabase Auth

## Important limitation

The existing EF migration history in this repository includes SQL Server-specific and SQLite-specific fixes. It is not a clean cross-provider migration chain for PostgreSQL.

That is why PostgreSQL startup currently uses model-based schema creation for a fresh Supabase database.

If you later want full PostgreSQL-native migrations:

1. Create a dedicated PostgreSQL migration baseline
2. Apply that baseline to a fresh PostgreSQL database
3. Use PostgreSQL-only migrations from that point forward
