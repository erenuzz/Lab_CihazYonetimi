using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class mig_kullaniciveEgitim : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "kullaniciveMentorEgitimleris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MentorEgitimleriId = table.Column<int>(type: "int", nullable: false),
                    AppUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kullaniciveMentorEgitimleris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kullaniciveMentorEgitimleris_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_kullaniciveMentorEgitimleris_mentorEgitimleris_MentorEgitimleriId",
                        column: x => x.MentorEgitimleriId,
                        principalTable: "mentorEgitimleris",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kullaniciveMentorEgitimleris_AppUserId",
                table: "kullaniciveMentorEgitimleris",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_kullaniciveMentorEgitimleris_MentorEgitimleriId",
                table: "kullaniciveMentorEgitimleris",
                column: "MentorEgitimleriId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kullaniciveMentorEgitimleris");
        }
    }
}
