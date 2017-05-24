using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "inventory_hilo",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "product_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Blocked = table.Column<double>(nullable: false),
                    BlockedReason = table.Column<string>(nullable: true),
                    ExpiredOn = table.Column<DateTime>(nullable: false),
                    LocM = table.Column<int>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Reserved = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Height = table.Column<double>(nullable: false),
                    InventoryRef = table.Column<int>(nullable: false),
                    Length = table.Column<double>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Price = table.Column<double>(nullable: false),
                    ProductGroup = table.Column<int>(nullable: false),
                    Supplier = table.Column<string>(nullable: true),
                    UnitMeasure = table.Column<string>(nullable: true),
                    Volume = table.Column<double>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    Width = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Inventory_InventoryRef",
                        column: x => x.InventoryRef,
                        principalTable: "Inventory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_InventoryRef",
                table: "Product",
                column: "InventoryRef",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropSequence(
                name: "inventory_hilo");

            migrationBuilder.DropSequence(
                name: "product_hilo");
        }
    }
}
