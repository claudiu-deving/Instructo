using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changes3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructorVehicleCategories");

            migrationBuilder.CreateTable(
                name: "[InstructorVehicleCategories]",
                columns: table => new
                {
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_[InstructorVehicleCategories]", x => new { x.InstructorId, x.VehicleCategoryId });
                    table.ForeignKey(
                        name: "FK_[InstructorVehicleCategories]_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_[InstructorVehicleCategories]_VehicleCategories_VehicleCategoryId",
                        column: x => x.VehicleCategoryId,
                        principalTable: "VehicleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_[InstructorVehicleCategories]_VehicleCategoryId",
                table: "[InstructorVehicleCategories]",
                column: "VehicleCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "[InstructorVehicleCategories]");

            migrationBuilder.CreateTable(
                name: "InstructorVehicleCategories",
                columns: table => new
                {
                    InstructorsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleCategoriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorVehicleCategories", x => new { x.InstructorsId, x.VehicleCategoriesId });
                    table.ForeignKey(
                        name: "FK_InstructorVehicleCategories_Instructors_InstructorsId",
                        column: x => x.InstructorsId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorVehicleCategories_VehicleCategories_VehicleCategoriesId",
                        column: x => x.VehicleCategoriesId,
                        principalTable: "VehicleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstructorVehicleCategories_VehicleCategoriesId",
                table: "InstructorVehicleCategories",
                column: "VehicleCategoriesId");
        }
    }
}
