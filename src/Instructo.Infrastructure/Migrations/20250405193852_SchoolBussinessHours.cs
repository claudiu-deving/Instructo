using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instructo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SchoolBussinessHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BussinessHours",
                table: "Schools",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BussinessHours",
                table: "Schools");
        }
    }
}
