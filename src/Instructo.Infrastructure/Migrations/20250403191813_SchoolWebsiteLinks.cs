using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instructo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SchoolWebsiteLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebsiteLinks_Schools_SchoolId",
                table: "WebsiteLinks");

            migrationBuilder.DropIndex(
                name: "IX_WebsiteLinks_SchoolId",
                table: "WebsiteLinks");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "WebsiteLinks");

            migrationBuilder.CreateTable(
                name: "SchoolWebsiteLinks",
                columns: table => new
                {
                    SchoolsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebsiteLinksId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolWebsiteLinks", x => new { x.SchoolsId, x.WebsiteLinksId });
                    table.ForeignKey(
                        name: "FK_SchoolWebsiteLinks_Schools_SchoolsId",
                        column: x => x.SchoolsId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolWebsiteLinks_WebsiteLinks_WebsiteLinksId",
                        column: x => x.WebsiteLinksId,
                        principalTable: "WebsiteLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolWebsiteLinks_WebsiteLinksId",
                table: "SchoolWebsiteLinks",
                column: "WebsiteLinksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchoolWebsiteLinks");

            migrationBuilder.AddColumn<Guid>(
                name: "SchoolId",
                table: "WebsiteLinks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteLinks_SchoolId",
                table: "WebsiteLinks",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_WebsiteLinks_Schools_SchoolId",
                table: "WebsiteLinks",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");
        }
    }
}
