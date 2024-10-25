using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVALORA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedFromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShippingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Carrier = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrackingNumber = table.Column<string>(type: "varchar(15)", nullable: true),
                    PaymentID = table.Column<string>(type: "varchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderHeaders_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderHeaderId = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderSummaries_OrderHeaders_OrderHeaderId",
                        column: x => x.OrderHeaderId,
                        principalTable: "OrderHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderSummaryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    OrderSummaryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSummaryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderSummaryItems_OrderSummaries_OrderSummaryId",
                        column: x => x.OrderSummaryId,
                        principalTable: "OrderSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderSummaryItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_ApplicationUserId",
                table: "OrderHeaders",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_TrackingNumber",
                table: "OrderHeaders",
                column: "TrackingNumber",
                unique: true,
                filter: "[TrackingNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSummaries_OrderHeaderId",
                table: "OrderSummaries",
                column: "OrderHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSummaryItems_OrderSummaryId",
                table: "OrderSummaryItems",
                column: "OrderSummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSummaryItems_ProductId",
                table: "OrderSummaryItems",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderSummaryItems");

            migrationBuilder.DropTable(
                name: "OrderSummaries");

            migrationBuilder.DropTable(
                name: "OrderHeaders");
        }
    }
}
