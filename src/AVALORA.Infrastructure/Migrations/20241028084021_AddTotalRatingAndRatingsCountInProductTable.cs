using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVALORA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalRatingAndRatingsCountInProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RatingsCount",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRating",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "RatingsCount", "TotalRating" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "RatingsCount", "TotalRating" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "RatingsCount", "TotalRating" },
                values: new object[] { 0m, 0m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingsCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TotalRating",
                table: "Products");
        }
    }
}
