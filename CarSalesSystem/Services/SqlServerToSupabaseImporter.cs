using System.Data;
using System.Data.Common;
using CarSalesSystem.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CarSalesSystem.Services;

public class SqlServerToSupabaseImporter
{
	private const string DefaultSourceConnection =
		"Server=.;Database=CarSalesSystem;Trusted_Connection=True;TrustServerCertificate=True;";

	private static readonly string[] TargetCleanupTables =
	[
		"FavoriteCars",
		"Payments",
		"Cars",
		"Dealers",
		"Categories",
		"AspNetUserTokens",
		"AspNetUserLogins",
		"AspNetUserClaims",
		"AspNetUserRoles",
		"AspNetRoleClaims",
		"AspNetUsers",
		"AspNetRoles"
	];

	private static readonly string[] IdentitySequenceTables =
	[
		"Categories",
		"Dealers",
		"Cars",
		"Payments",
		"FavoriteCars",
		"AspNetRoleClaims",
		"AspNetUserClaims"
	];

	private readonly ApplicationDbContext targetContext;
	private readonly ILogger<SqlServerToSupabaseImporter> logger;

	public SqlServerToSupabaseImporter(
		ApplicationDbContext targetContext,
		ILogger<SqlServerToSupabaseImporter> logger)
	{
		this.targetContext = targetContext;
		this.logger = logger;
	}

	public async Task ImportAsync(string? sourceConnectionString, CancellationToken cancellationToken = default)
	{
		var sourceConnection = string.IsNullOrWhiteSpace(sourceConnectionString)
			? DefaultSourceConnection
			: sourceConnectionString;

		await EnsureTargetSchemaAsync(cancellationToken);

		await using var source = new SqlConnection(sourceConnection);
		await source.OpenAsync(cancellationToken);

		await using var transaction = await targetContext.Database.BeginTransactionAsync(cancellationToken);
		await ClearTargetAsync(cancellationToken);

		await CopyTableAsync(source, "AspNetRoles", cancellationToken);
		await CopyTableAsync(source, "AspNetUsers", cancellationToken);
		await CopyTableAsync(source, "AspNetRoleClaims", cancellationToken);
		await CopyTableAsync(source, "AspNetUserClaims", cancellationToken);
		await CopyTableAsync(source, "AspNetUserLogins", cancellationToken);
		await CopyTableAsync(source, "AspNetUserRoles", cancellationToken);
		await CopyTableAsync(source, "AspNetUserTokens", cancellationToken);
		await CopyTableAsync(source, "Categories", cancellationToken);
		await CopyTableAsync(source, "Dealers", cancellationToken);
		await CopyTableAsync(source, "Cars", cancellationToken);

		if (await SourceTableExistsAsync(source, "FavoriteCars", cancellationToken))
		{
			await CopyTableAsync(source, "FavoriteCars", cancellationToken);
		}

		await ImportPaymentsAsync(source, cancellationToken);
		await ResetSequencesAsync(cancellationToken);

		await transaction.CommitAsync(cancellationToken);
		logger.LogInformation("SQL Server to Supabase import completed successfully.");
	}

	private async Task ClearTargetAsync(CancellationToken cancellationToken)
	{
		var existingTables = new List<string>();
		foreach (var table in TargetCleanupTables)
		{
			if (await TargetTableExistsAsync(table, cancellationToken))
			{
				existingTables.Add(QuoteIdentifier(table));
			}
		}

		if (existingTables.Count == 0)
		{
			return;
		}

		var sql = $"TRUNCATE TABLE {string.Join(", ", existingTables)} RESTART IDENTITY CASCADE;";

		await targetContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
	}

	private async Task EnsureTargetSchemaAsync(CancellationToken cancellationToken)
	{
		if (await HasExpectedSchemaAsync(cancellationToken))
		{
			return;
		}

		logger.LogInformation("Target schema is missing required tables. Rebuilding public schema.");

		const string rebuildSchemaSql = """
			DROP SCHEMA IF EXISTS public CASCADE;
			CREATE SCHEMA public;
			GRANT ALL ON SCHEMA public TO postgres;
			GRANT ALL ON SCHEMA public TO public;
			""";

		await targetContext.Database.ExecuteSqlRawAsync(rebuildSchemaSql, cancellationToken);
		var createScript = targetContext.Database.GenerateCreateScript();
		await targetContext.Database.ExecuteSqlRawAsync(createScript, cancellationToken);
	}

	private async Task CopyTableAsync(SqlConnection source, string tableName, CancellationToken cancellationToken)
	{
		logger.LogInformation("Importing table {TableName}", tableName);

		await using var sourceCommand = source.CreateCommand();
		sourceCommand.CommandText = $"SELECT * FROM [{tableName}]";

		await using var reader = await sourceCommand.ExecuteReaderAsync(cancellationToken);
		var schema = reader.GetColumnSchema();

		if (schema.Count == 0)
		{
			return;
		}

		var columnNames = schema.Select(column => column.ColumnName!).ToArray();
		var insertSql = $"""
			INSERT INTO {QuoteIdentifier(tableName)} ({string.Join(", ", columnNames.Select(QuoteIdentifier))})
			VALUES ({string.Join(", ", columnNames.Select((_, index) => $"@p{index}"))});
			""";

		var targetConnection = targetContext.Database.GetDbConnection();
		if (targetConnection.State != ConnectionState.Open)
		{
			await targetConnection.OpenAsync(cancellationToken);
		}

		while (await reader.ReadAsync(cancellationToken))
		{
			await using var insertCommand = targetConnection.CreateCommand();
			insertCommand.Transaction = targetContext.Database.CurrentTransaction?.GetDbTransaction();
			insertCommand.CommandText = insertSql;

			for (var index = 0; index < columnNames.Length; index++)
			{
				var parameter = insertCommand.CreateParameter();
				parameter.ParameterName = $"@p{index}";
				parameter.Value = reader.IsDBNull(index) ? DBNull.Value : reader.GetValue(index);
				insertCommand.Parameters.Add(parameter);
			}

			await insertCommand.ExecuteNonQueryAsync(cancellationToken);
		}
	}

	private async Task ImportPaymentsAsync(SqlConnection source, CancellationToken cancellationToken)
	{
		logger.LogInformation("Importing table Payments with debit-card transformation");

		const string sql = """
			SELECT
				p.Id,
				p.PaymentTime,
				p.TotalAmount,
				p.UserId,
				p.CarId,
				p.IsSuccessful,
				d.CardNumber,
				d.FullName,
				d.ExpirationMonth,
				d.ExpirationYear
			FROM Payments p
			LEFT JOIN DebitCards d ON d.Id = p.DebitCardId
			ORDER BY p.Id
			""";

		await using var sourceCommand = source.CreateCommand();
		sourceCommand.CommandText = sql;

		await using var reader = await sourceCommand.ExecuteReaderAsync(cancellationToken);

		var targetConnection = targetContext.Database.GetDbConnection();
		if (targetConnection.State != ConnectionState.Open)
		{
			await targetConnection.OpenAsync(cancellationToken);
		}

		const string insertSql = """
			INSERT INTO "Payments"
				("Id", "PaymentTime", "TotalAmount", "UserId", "CarId", "CardLast4", "CardholderName", "ExpirationMonth", "ExpirationYear", "IsSuccessful")
			VALUES
				(@id, @paymentTime, @totalAmount, @userId, @carId, @cardLast4, @cardholderName, @expirationMonth, @expirationYear, @isSuccessful);
			""";

		while (await reader.ReadAsync(cancellationToken))
		{
			await using var insertCommand = targetConnection.CreateCommand();
			insertCommand.Transaction = targetContext.Database.CurrentTransaction?.GetDbTransaction();
			insertCommand.CommandText = insertSql;

			AddParameter(insertCommand, "@id", reader.GetInt32(0));
			AddParameter(insertCommand, "@paymentTime", reader.GetDateTime(1));
			AddParameter(insertCommand, "@totalAmount", reader.GetDecimal(2));
			AddParameter(insertCommand, "@userId", reader.IsDBNull(3) ? DBNull.Value : reader.GetString(3));
			AddParameter(insertCommand, "@carId", reader.GetInt32(4));
			AddParameter(insertCommand, "@cardLast4", GetLast4(reader.IsDBNull(6) ? null : reader.GetString(6)));
			AddParameter(insertCommand, "@cardholderName", reader.IsDBNull(7) ? "Unknown" : reader.GetString(7));
			AddParameter(insertCommand, "@expirationMonth", NormalizeMonth(reader.IsDBNull(8) ? null : reader.GetString(8)));
			AddParameter(insertCommand, "@expirationYear", reader.IsDBNull(9) ? 0 : reader.GetInt32(9));
			AddParameter(insertCommand, "@isSuccessful", reader.GetBoolean(5));

			await insertCommand.ExecuteNonQueryAsync(cancellationToken);
		}
	}

	private async Task ResetSequencesAsync(CancellationToken cancellationToken)
	{
		foreach (var table in IdentitySequenceTables)
		{
			var sql = $"""
				SELECT setval(
					pg_get_serial_sequence('public.{QuoteIdentifier(table)}', 'Id'),
					GREATEST(COALESCE((SELECT MAX("Id") FROM {QuoteIdentifier(table)}), 0), 1),
					true
				);
				""";

			await targetContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
		}
	}

	private static async Task<bool> SourceTableExistsAsync(SqlConnection source, string tableName, CancellationToken cancellationToken)
	{
		await using var command = source.CreateCommand();
		command.CommandText = """
			SELECT COUNT(*)
			FROM INFORMATION_SCHEMA.TABLES
			WHERE TABLE_NAME = @tableName AND TABLE_TYPE = 'BASE TABLE'
			""";
		command.Parameters.Add(new SqlParameter("@tableName", tableName));

		var result = (int)await command.ExecuteScalarAsync(cancellationToken);
		return result > 0;
	}

	private async Task<bool> TargetTableExistsAsync(string tableName, CancellationToken cancellationToken)
	{
		const string sql = """
			SELECT COUNT(*)
			FROM information_schema.tables
			WHERE table_schema = 'public' AND table_name = @tableName
			""";

		var connection = targetContext.Database.GetDbConnection();
		if (connection.State != ConnectionState.Open)
		{
			await connection.OpenAsync(cancellationToken);
		}

		await using var command = connection.CreateCommand();
		command.Transaction = targetContext.Database.CurrentTransaction?.GetDbTransaction();
		command.CommandText = sql;
		AddParameter(command, "@tableName", tableName);

		var result = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
		return result > 0;
	}

	private async Task<bool> HasExpectedSchemaAsync(CancellationToken cancellationToken)
	{
		var requiredTables = new[]
		{
			"AspNetUsers",
			"AspNetRoles",
			"Categories",
			"Dealers",
			"Cars",
			"Payments",
			"FavoriteCars"
		};

		foreach (var table in requiredTables)
		{
			if (!await TargetTableExistsAsync(table, cancellationToken))
			{
				return false;
			}
		}

		return true;
	}

	private static void AddParameter(DbCommand command, string name, object value)
	{
		var parameter = command.CreateParameter();
		parameter.ParameterName = name;
		parameter.Value = value;
		command.Parameters.Add(parameter);
	}

	private static string QuoteIdentifier(string identifier)
	{
		return $"\"{identifier.Replace("\"", "\"\"")}\"";
	}

	private static string GetLast4(string? cardNumber)
	{
		if (string.IsNullOrWhiteSpace(cardNumber))
		{
			return "0000";
		}

		var digitsOnly = new string(cardNumber.Where(char.IsDigit).ToArray());
		if (digitsOnly.Length >= 4)
		{
			return digitsOnly[^4..];
		}

		return digitsOnly.PadLeft(4, '0');
	}

	private static string NormalizeMonth(string? month)
	{
		if (string.IsNullOrWhiteSpace(month))
		{
			return "01";
		}

		var digitsOnly = new string(month.Where(char.IsDigit).ToArray());
		if (digitsOnly.Length == 0)
		{
			return "01";
		}

		if (!int.TryParse(digitsOnly, out var numericMonth))
		{
			return "01";
		}

		numericMonth = Math.Clamp(numericMonth, 1, 12);
		return numericMonth.ToString("00");
	}
}
