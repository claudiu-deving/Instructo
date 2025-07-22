using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Schools_SchoolId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Schools_Addresses_AddressId",
                table: "Schools");

            migrationBuilder.DropForeignKey(
                name: "FK_WebsiteLinks_Schools_SchoolId",
                table: "WebsiteLinks");

            migrationBuilder.DropIndex(
                name: "IX_Schools_AddressId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Schools");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Schools_SchoolId",
                table: "Addresses",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebsiteLinks_Schools_SchoolId",
                table: "WebsiteLinks",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Schools_SchoolId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_WebsiteLinks_Schools_SchoolId",
                table: "WebsiteLinks");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Schools",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Schools_AddressId",
                table: "Schools",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Schools_SchoolId",
                table: "Addresses",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_Addresses_AddressId",
                table: "Schools",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebsiteLinks_Schools_SchoolId",
                table: "WebsiteLinks",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");
        }
    }
}
