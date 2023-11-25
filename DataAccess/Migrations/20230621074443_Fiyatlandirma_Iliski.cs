using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class Fiyatlandirma_Iliski : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "rezervasyonId",
                table: "fiyatlandirmas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_fiyatlandirmas_rezervasyonId",
                table: "fiyatlandirmas",
                column: "rezervasyonId");

            migrationBuilder.AddForeignKey(
                name: "FK_fiyatlandirmas_rezervasyons_rezervasyonId",
                table: "fiyatlandirmas",
                column: "rezervasyonId",
                principalTable: "rezervasyons",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fiyatlandirmas_rezervasyons_rezervasyonId",
                table: "fiyatlandirmas");

            migrationBuilder.DropIndex(
                name: "IX_fiyatlandirmas_rezervasyonId",
                table: "fiyatlandirmas");

            migrationBuilder.DropColumn(
                name: "rezervasyonId",
                table: "fiyatlandirmas");
        }
    }



    //public partial class Fiyatlandirma_Iliski : Migration
    //{
    //    protected override void Up(MigrationBuilder migrationBuilder)
    //    {
    //        migrationBuilder.AddColumn<int>(
    //            name: "rezervasyonId",
    //            table: "fiyatlandirmas",
    //            type: "int",
    //            nullable: false,
    //            defaultValue: 0);

    //        migrationBuilder.CreateIndex(
    //            name: "IX_fiyatlandirmas_rezervasyonId",
    //            table: "fiyatlandirmas",
    //            column: "rezervasyonId");

    //        migrationBuilder.AddForeignKey(
    //            name: "FK_fiyatlandirmas_rezervasyons_rezervasyonId",
    //            table: "fiyatlandirmas",
    //            column: "rezervasyonId",
    //            principalTable: "rezervasyons",
    //            principalColumn: "Id",
    //            onDelete: ReferentialAction.Cascade);
    //    }

    //    protected override void Down(MigrationBuilder migrationBuilder)
    //    {
    //        migrationBuilder.DropForeignKey(
    //            name: "FK_fiyatlandirmas_rezervasyons_rezervasyonId",
    //            table: "fiyatlandirmas");

    //        migrationBuilder.DropIndex(
    //            name: "IX_fiyatlandirmas_rezervasyonId",
    //            table: "fiyatlandirmas");

    //        migrationBuilder.DropColumn(
    //            name: "rezervasyonId",
    //            table: "fiyatlandirmas");
    //    }
    //}
}
