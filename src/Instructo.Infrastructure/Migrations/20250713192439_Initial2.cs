using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Statistics",
                table: "Schools",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SchoolCategoryPricings",
                columns: table => new
                {
                    SchoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleCategoryId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    FullPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InstallmentPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Installments = table.Column<int>(type: "int", nullable: false),
                    Transmission = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolCategoryPricings", x => new { x.SchoolId, x.VehicleCategoryId });
                    table.ForeignKey(
                        name: "FK_SchoolCategoryPricings_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolCategoryPricings_VehicleCategories_VehicleCategoryId",
                        column: x => x.VehicleCategoryId,
                        principalTable: "VehicleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCategoryPricings_VehicleCategoryId",
                table: "SchoolCategoryPricings",
                column: "VehicleCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchoolCategoryPricings");

            migrationBuilder.DropColumn(
                name: "Statistics",
                table: "Schools");
        }
    }
}
