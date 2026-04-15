using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarSalesSystem.Data.Migrations
{
	public partial class MarketplaceUpgradeRepair : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(
				"""
				IF OBJECT_ID(N'[FavoriteCars]', N'U') IS NULL
				BEGIN
					CREATE TABLE [FavoriteCars] (
						[Id] int NOT NULL IDENTITY,
						[UserId] nvarchar(max) NOT NULL,
						[CarId] int NOT NULL,
						[CreatedOnUtc] datetime2 NOT NULL,
						CONSTRAINT [PK_FavoriteCars] PRIMARY KEY ([Id]),
						CONSTRAINT [FK_FavoriteCars_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([Id]) ON DELETE CASCADE
					);
				END
				""");

			migrationBuilder.Sql(
				"""
				IF NOT EXISTS (
					SELECT 1
					FROM sys.indexes
					WHERE name = N'IX_FavoriteCars_UserId_CarId'
					  AND object_id = OBJECT_ID(N'[FavoriteCars]')
				)
				BEGIN
					CREATE UNIQUE INDEX [IX_FavoriteCars_UserId_CarId] ON [FavoriteCars] ([UserId], [CarId]);
				END
				""");

			migrationBuilder.Sql(
				"""
				IF COL_LENGTH('Payments', 'DebitCardId') IS NOT NULL
				BEGIN
					IF EXISTS (
						SELECT 1
						FROM sys.foreign_keys
						WHERE name = N'FK_Payments_DebitCards_DebitCardId'
					)
					BEGIN
						ALTER TABLE [Payments] DROP CONSTRAINT [FK_Payments_DebitCards_DebitCardId];
					END;

					IF EXISTS (
						SELECT 1
						FROM sys.indexes
						WHERE name = N'IX_Payments_DebitCardId'
						  AND object_id = OBJECT_ID(N'[Payments]')
					)
					BEGIN
						DROP INDEX [IX_Payments_DebitCardId] ON [Payments];
					END;

					ALTER TABLE [Payments] DROP COLUMN [DebitCardId];
				END
				""");

			migrationBuilder.Sql(
				"""
				IF COL_LENGTH('Payments', 'CardLast4') IS NULL
				BEGIN
					ALTER TABLE [Payments] ADD [CardLast4] nvarchar(4) NOT NULL CONSTRAINT [DF_Payments_CardLast4] DEFAULT N'0000';
				END
				""");

			migrationBuilder.Sql(
				"""
				IF COL_LENGTH('Payments', 'CardholderName') IS NULL
				BEGIN
					ALTER TABLE [Payments] ADD [CardholderName] nvarchar(max) NOT NULL CONSTRAINT [DF_Payments_CardholderName] DEFAULT N'Unknown Buyer';
				END
				""");

			migrationBuilder.Sql(
				"""
				IF COL_LENGTH('Payments', 'ExpirationMonth') IS NULL
				BEGIN
					ALTER TABLE [Payments] ADD [ExpirationMonth] nvarchar(2) NOT NULL CONSTRAINT [DF_Payments_ExpirationMonth] DEFAULT N'00';
				END
				""");

			migrationBuilder.Sql(
				"""
				IF COL_LENGTH('Payments', 'ExpirationYear') IS NULL
				BEGIN
					ALTER TABLE [Payments] ADD [ExpirationYear] int NOT NULL CONSTRAINT [DF_Payments_ExpirationYear] DEFAULT 0;
				END
				""");

			migrationBuilder.Sql(
				"""
				IF OBJECT_ID(N'[DebitCards]', N'U') IS NOT NULL
				BEGIN
					DROP TABLE [DebitCards];
				END
				""");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "FavoriteCars");

			migrationBuilder.DropColumn(
				name: "CardLast4",
				table: "Payments");

			migrationBuilder.DropColumn(
				name: "CardholderName",
				table: "Payments");

			migrationBuilder.DropColumn(
				name: "ExpirationMonth",
				table: "Payments");

			migrationBuilder.DropColumn(
				name: "ExpirationYear",
				table: "Payments");
		}
	}
}
