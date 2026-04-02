using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarSalesSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class ISboughtAddedInCarEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBought",
                table: "Cars",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBought",
                table: "Cars");
        }
    }
}
