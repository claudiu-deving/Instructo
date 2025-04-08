using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instructo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CategoriesAndCertificatesMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ARRCertificates_Schools_SchoolId",
                table: "ARRCertificates");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleCategories_Schools_SchoolId",
                table: "VehicleCategories");

            migrationBuilder.DropIndex(
                name: "IX_VehicleCategories_SchoolId",
                table: "VehicleCategories");

            migrationBuilder.DropIndex(
                name: "IX_ARRCertificates_SchoolId",
                table: "ARRCertificates");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "VehicleCategories");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "ARRCertificates");

            migrationBuilder.CreateTable(
                name: "SchoolCategories",
                columns: table => new
                {
                    SchoolsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleCategoriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolCategories", x => new { x.SchoolsId, x.VehicleCategoriesId });
                    table.ForeignKey(
                        name: "FK_SchoolCategories_Schools_SchoolsId",
                        column: x => x.SchoolsId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolCategories_VehicleCategories_VehicleCategoriesId",
                        column: x => x.VehicleCategoriesId,
                        principalTable: "VehicleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolCertificates",
                columns: table => new
                {
                    CertificatesId = table.Column<int>(type: "int", nullable: false),
                    SchoolsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolCertificates", x => new { x.CertificatesId, x.SchoolsId });
                    table.ForeignKey(
                        name: "FK_SchoolCertificates_ARRCertificates_CertificatesId",
                        column: x => x.CertificatesId,
                        principalTable: "ARRCertificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolCertificates_Schools_SchoolsId",
                        column: x => x.SchoolsId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCategories_VehicleCategoriesId",
                table: "SchoolCategories",
                column: "VehicleCategoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCertificates_SchoolsId",
                table: "SchoolCertificates",
                column: "SchoolsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchoolCategories");

            migrationBuilder.DropTable(
                name: "SchoolCertificates");

            migrationBuilder.AddColumn<Guid>(
                name: "SchoolId",
                table: "VehicleCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SchoolId",
                table: "ARRCertificates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ARRCertificates",
                keyColumn: "Id",
                keyValue: 1,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ARRCertificates",
                keyColumn: "Id",
                keyValue: 2,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ARRCertificates",
                keyColumn: "Id",
                keyValue: 4,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ARRCertificates",
                keyColumn: "Id",
                keyValue: 8,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ARRCertificates",
                keyColumn: "Id",
                keyValue: 16,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ARRCertificates",
                keyColumn: "Id",
                keyValue: 32,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ARRCertificates",
                keyColumn: "Id",
                keyValue: 64,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ARRCertificates",
                keyColumn: "Id",
                keyValue: 128,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ARRCertificates",
                keyColumn: "Id",
                keyValue: 256,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 0,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 9,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 10,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 11,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 12,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 13,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 14,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 15,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 16,
                column: "SchoolId",
                value: null);

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 17,
                column: "SchoolId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleCategories_SchoolId",
                table: "VehicleCategories",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ARRCertificates_SchoolId",
                table: "ARRCertificates",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_ARRCertificates_Schools_SchoolId",
                table: "ARRCertificates",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleCategories_Schools_SchoolId",
                table: "VehicleCategories",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");
        }
    }
}
