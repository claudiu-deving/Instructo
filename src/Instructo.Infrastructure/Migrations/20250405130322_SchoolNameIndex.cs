using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instructo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SchoolNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "Schools",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_CompanyName",
                table: "Schools",
                column: "CompanyName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schools_CompanyName",
                table: "Schools");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "Schools",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
