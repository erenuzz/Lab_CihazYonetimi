using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class mig_mentoregitim4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mentorEgitimleris_AspNetUsers_AppUserId",
                table: "mentorEgitimleris");

            migrationBuilder.DropForeignKey(
                name: "FK_mentorEgitimleris_mentors_MentorId",
                table: "mentorEgitimleris");

            migrationBuilder.DropIndex(
                name: "IX_mentorEgitimleris_AppUserId",
                table: "mentorEgitimleris");

            migrationBuilder.DropIndex(
                name: "IX_mentorEgitimleris_MentorId",
                table: "mentorEgitimleris");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "mentorEgitimleris");

            migrationBuilder.DropColumn(
                name: "MentorId",
                table: "mentorEgitimleris");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "mentorEgitimleris",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MentorId",
                table: "mentorEgitimleris",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mentorEgitimleris_AppUserId",
                table: "mentorEgitimleris",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_mentorEgitimleris_MentorId",
                table: "mentorEgitimleris",
                column: "MentorId");

            migrationBuilder.AddForeignKey(
                name: "FK_mentorEgitimleris_AspNetUsers_AppUserId",
                table: "mentorEgitimleris",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_mentorEgitimleris_mentors_MentorId",
                table: "mentorEgitimleris",
                column: "MentorId",
                principalTable: "mentors",
                principalColumn: "Id");
        }
    }
}
