using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class mig_migEgitim : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "egitimlers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    appUserId = table.Column<int>(type: "int", nullable: false),
                    cihazId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_egitimlers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_egitimlers_AspNetUsers_appUserId",
                        column: x => x.appUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_egitimlers_cihazlars_cihazId",
                        column: x => x.cihazId,
                        principalTable: "cihazlars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_egitimlers_appUserId",
                table: "egitimlers",
                column: "appUserId");

            migrationBuilder.CreateIndex(
                name: "IX_egitimlers_cihazId",
                table: "egitimlers",
                column: "cihazId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "egitimlers");
        }
    }
}
