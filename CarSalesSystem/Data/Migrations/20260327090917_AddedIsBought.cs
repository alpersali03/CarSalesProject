using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarSalesSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsBought : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBought",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBought",
                table: "Payments");
        }
    }
}
