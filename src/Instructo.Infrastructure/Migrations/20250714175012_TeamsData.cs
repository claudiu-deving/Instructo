using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TeamsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instructors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthYear = table.Column<int>(type: "int", nullable: false),
                    YearsExperience = table.Column<int>(type: "int", nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instructors_Images_ProfileImageId",
                        column: x => x.ProfileImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Instructors_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Instructors_ProfileImageId",
                table: "Instructors",
                column: "ProfileImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_TeamId",
                table: "Instructors",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorVehicleCategories_VehicleCategoriesId",
                table: "InstructorVehicleCategories",
                column: "VehicleCategoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SchoolId",
                table: "Teams",
                column: "SchoolId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructorVehicleCategories");

            migrationBuilder.DropTable(
                name: "Instructors");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
