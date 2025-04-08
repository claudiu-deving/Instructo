using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instructo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SerializedPhoneNumberGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhoneNumber");

            migrationBuilder.DropTable(
                name: "PhoneNumbersGroup");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumbersGroups",
                table: "Schools",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumbersGroups",
                table: "Schools");

            migrationBuilder.CreateTable(
                name: "PhoneNumbersGroup",
                columns: table => new
                {
                    SchoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneNumbersGroup", x => new { x.SchoolId, x.Id });
                    table.ForeignKey(
                        name: "FK_PhoneNumbersGroup_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneNumber",
                columns: table => new
                {
                    PhoneNumbersGroupSchoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumbersGroupId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneNumber", x => new { x.PhoneNumbersGroupSchoolId, x.PhoneNumbersGroupId, x.Id });
                    table.ForeignKey(
                        name: "FK_PhoneNumber_PhoneNumbersGroup_PhoneNumbersGroupSchoolId_PhoneNumbersGroupId",
                        columns: x => new { x.PhoneNumbersGroupSchoolId, x.PhoneNumbersGroupId },
                        principalTable: "PhoneNumbersGroup",
                        principalColumns: new[] { "SchoolId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
