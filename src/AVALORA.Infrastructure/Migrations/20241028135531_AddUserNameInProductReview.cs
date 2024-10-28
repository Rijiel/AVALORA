using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVALORA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameInProductReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ProductReviews",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ProductReviews");
        }
    }
}
