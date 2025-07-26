using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changes4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_[InstructorVehicleCategories]_Instructors_InstructorId",
                table: "[InstructorVehicleCategories]");

            migrationBuilder.DropForeignKey(
                name: "FK_[InstructorVehicleCategories]_VehicleCategories_VehicleCategoryId",
                table: "[InstructorVehicleCategories]");

            migrationBuilder.DropPrimaryKey(
                name: "PK_[InstructorVehicleCategories]",
                table: "[InstructorVehicleCategories]");

            migrationBuilder.RenameTable(
                name: "[InstructorVehicleCategories]",
                newName: "InstructorVehicleCategories");

            migrationBuilder.RenameIndex(
                name: "IX_[InstructorVehicleCategories]_VehicleCategoryId",
                table: "InstructorVehicleCategories",
                newName: "IX_InstructorVehicleCategories_VehicleCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstructorVehicleCategories",
                table: "InstructorVehicleCategories",
                columns: new[] { "InstructorId", "VehicleCategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorVehicleCategories_Instructors_InstructorId",
                table: "InstructorVehicleCategories",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorVehicleCategories_VehicleCategories_VehicleCategoryId",
                table: "InstructorVehicleCategories",
                column: "VehicleCategoryId",
                principalTable: "VehicleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstructorVehicleCategories_Instructors_InstructorId",
                table: "InstructorVehicleCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorVehicleCategories_VehicleCategories_VehicleCategoryId",
                table: "InstructorVehicleCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstructorVehicleCategories",
                table: "InstructorVehicleCategories");

            migrationBuilder.RenameTable(
                name: "InstructorVehicleCategories",
                newName: "[InstructorVehicleCategories]");

            migrationBuilder.RenameIndex(
                name: "IX_InstructorVehicleCategories_VehicleCategoryId",
                table: "[InstructorVehicleCategories]",
                newName: "IX_[InstructorVehicleCategories]_VehicleCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_[InstructorVehicleCategories]",
                table: "[InstructorVehicleCategories]",
                columns: new[] { "InstructorId", "VehicleCategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_[InstructorVehicleCategories]_Instructors_InstructorId",
                table: "[InstructorVehicleCategories]",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_[InstructorVehicleCategories]_VehicleCategories_VehicleCategoryId",
                table: "[InstructorVehicleCategories]",
                column: "VehicleCategoryId",
                principalTable: "VehicleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
