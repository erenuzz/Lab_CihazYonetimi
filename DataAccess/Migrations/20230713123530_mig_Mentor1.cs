using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class mig_Mentor1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mentors_AspNetUsers_AppUserId",
                table: "mentors");

            migrationBuilder.DropIndex(
                name: "IX_mentors_AppUserId",
                table: "mentors");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "mentors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "mentors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_mentors_AppUserId",
                table: "mentors",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_mentors_AspNetUsers_AppUserId",
                table: "mentors",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
