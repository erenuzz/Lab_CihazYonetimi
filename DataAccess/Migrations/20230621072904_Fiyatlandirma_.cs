using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class Fiyatlandirma_ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cihazId",
                table: "fiyatlandirmas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_fiyatlandirmas_cihazId",
                table: "fiyatlandirmas",
                column: "cihazId");

            migrationBuilder.AddForeignKey(
                name: "FK_fiyatlandirmas_cihazlars_cihazId",
                table: "fiyatlandirmas",
                column: "cihazId",
                principalTable: "cihazlars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fiyatlandirmas_cihazlars_cihazId",
                table: "fiyatlandirmas");

            migrationBuilder.DropIndex(
                name: "IX_fiyatlandirmas_cihazId",
                table: "fiyatlandirmas");

            migrationBuilder.DropColumn(
                name: "cihazId",
                table: "fiyatlandirmas");
        }
    }
}
