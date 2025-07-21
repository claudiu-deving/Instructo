using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddsExtraLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SchoolId",
                table: "Addresses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_SchoolId",
                table: "Addresses",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Schools_SchoolId",
                table: "Addresses",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Schools_SchoolId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_SchoolId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Addresses");
        }
    }
}
