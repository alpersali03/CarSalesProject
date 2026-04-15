using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarSalesSystem.Data.Migrations
{
	public partial class SecurePaymentStorage : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Payments_DebitCards_DebitCardId",
				table: "Payments");

			migrationBuilder.DropTable(
				name: "DebitCards");

			migrationBuilder.DropIndex(
				name: "IX_Payments_DebitCardId",
				table: "Payments");

			migrationBuilder.DropColumn(
				name: "DebitCardId",
				table: "Payments");

			migrationBuilder.AddColumn<string>(
				name: "CardLast4",
				table: "Payments",
				type: "nvarchar(4)",
				maxLength: 4,
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "CardholderName",
				table: "Payments",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "ExpirationMonth",
				table: "Payments",
				type: "nvarchar(2)",
				maxLength: 2,
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<int>(
				name: "ExpirationYear",
				table: "Payments",
				type: "int",
				nullable: false,
				defaultValue: 0);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
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

			migrationBuilder.AddColumn<int>(
				name: "DebitCardId",
				table: "Payments",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.CreateTable(
				name: "DebitCards",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CVV = table.Column<int>(type: "int", nullable: false),
					CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ExpirationMonth = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ExpirationYear = table.Column<int>(type: "int", nullable: false),
					FullName = table.Column<string>(type: "nvarchar(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DebitCards", x => x.Id);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Payments_DebitCardId",
				table: "Payments",
				column: "DebitCardId");

			migrationBuilder.AddForeignKey(
				name: "FK_Payments_DebitCards_DebitCardId",
				table: "Payments",
				column: "DebitCardId",
				principalTable: "DebitCards",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
