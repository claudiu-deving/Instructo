using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Instructo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CategoriesAndCertificates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ARRCertificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SchoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARRCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ARRCertificates_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VehicleCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SchoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleCategories_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ARRCertificates",
                columns: new[] { "Id", "Description", "Name", "SchoolId" },
                values: new object[,]
                {
                    { 1, "Certificate for general goods transportation", "Atestat pentru transport marfă", null },
                    { 2, "Certificate for passenger transportation", "Atestat pentru transport persoane", null },
                    { 4, "Certificate for dangerous goods transportation (ADR)", "Atestat ADR", null },
                    { 8, "Certificate for oversized load transportation", "Atestat pentru transport agabaritic", null },
                    { 16, "Certificate for taxi transportation", "Atestat pentru transport taxi", null },
                    { 32, "Certificate for transport managers (CPI)", "Atestat pentru manager de transport (CPI)", null },
                    { 64, "Certificate for driving instructors", "Atestat pentru instructor auto", null },
                    { 128, "Certificate for road legislation teachers", "Atestat pentru profesor de legislație rutieră", null },
                    { 256, "Certificate for safety advisors for the transport of dangerous goods", "Atestat pentru consilier de siguranță", null }
                });

            migrationBuilder.InsertData(
                table: "VehicleCategories",
                columns: new[] { "Id", "Description", "Name", "SchoolId" },
                values: new object[,]
                {
                    { 0, "Mopeds", "AM", null },
                    { 1, "Motorcycles with maximum 125cm³ cylinder capacity, maximum power of 11kW, and power-to-weight ratio not exceeding 0.1kW/kg; Motor tricycles with maximum power of 15kW", "A1", null },
                    { 2, "Motorcycles with maximum power of 35kW, power-to-weight ratio not exceeding 0.2kW/kg, and not derived from a vehicle with more than twice its power", "A2", null },
                    { 3, "Motorcycles with or without sidecar and motor tricycles with power over 15kW", "A", null },
                    { 4, "Quadricycles with unladen mass not exceeding 400kg (550kg for goods transport vehicles), excluding the mass of batteries for electric vehicles, equipped with internal combustion engine not exceeding 15kW net maximum power or electric motor not exceeding 15kW continuous rated power", "B1", null },
                    { 5, "Vehicles with maximum authorized mass not exceeding 3,500kg and with no more than 8 seats in addition to the driver's seat; Vehicle-trailer combinations where the trailer's maximum authorized mass doesn't exceed 750kg; Vehicle-trailer combinations not exceeding 4,250kg total, where the trailer's maximum authorized mass exceeds 750kg", "B", null },
                    { 6, "Vehicle-trailer combinations exceeding 4,250kg total, comprising a category B vehicle and a trailer or semi-trailer with maximum authorized mass not exceeding 3,500kg", "BE", null },
                    { 7, "Vehicles other than those in categories D or D1, with maximum authorized mass exceeding 3,500kg but not exceeding 7,500kg, designed to carry maximum 8 passengers in addition to the driver. These vehicles may be coupled with a trailer not exceeding 750kg maximum authorized mass", "C1", null },
                    { 8, "Vehicle-trailer combinations comprising a C1 vehicle and a trailer or semi-trailer with maximum authorized mass exceeding 750kg, provided the total doesn't exceed 12,000kg; Combinations where the towing vehicle is category B and the trailer or semi-trailer has a maximum authorized mass exceeding 3,500kg, provided the total doesn't exceed 12,000kg", "C1E", null },
                    { 9, "Vehicles other than those in categories D or D1, with maximum authorized mass exceeding 3,500kg, designed to carry maximum 8 passengers in addition to the driver; Combinations comprising a category C vehicle and a trailer with maximum authorized mass not exceeding 750kg", "C", null },
                    { 10, "Vehicle-trailer combinations comprising a category C vehicle and a trailer or semi-trailer with maximum authorized mass exceeding 750kg", "CE", null },
                    { 11, "Vehicles designed to carry maximum 16 passengers in addition to the driver, with maximum length not exceeding 8m; Combinations comprising a D1 vehicle and a trailer with maximum authorized mass not exceeding 750kg", "D1", null },
                    { 12, "Vehicle-trailer combinations comprising a D1 vehicle and a trailer with maximum authorized mass exceeding 750kg. The trailer must not be designed to carry passengers", "D1E", null },
                    { 13, "Vehicles designed to carry more than 8 passengers in addition to the driver. These vehicles may be coupled with a trailer not exceeding 750kg maximum authorized mass", "D", null },
                    { 14, "Vehicle-trailer combinations comprising a category D vehicle and a trailer with maximum authorized mass exceeding 750kg. The trailer must not be designed to carry passengers", "DE", null },
                    { 15, "Agricultural or forestry tractors", "Tr", null },
                    { 16, "Trolleybus", "Tb", null },
                    { 17, "Tram", "Tv", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ARRCertificates_SchoolId",
                table: "ARRCertificates",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleCategories_SchoolId",
                table: "VehicleCategories",
                column: "SchoolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ARRCertificates");

            migrationBuilder.DropTable(
                name: "VehicleCategories");
        }
    }
}
