using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class mig_MentorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MentorId",
                table: "mentorEgitimleris",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mentorEgitimleris_MentorId",
                table: "mentorEgitimleris",
                column: "MentorId");

            migrationBuilder.AddForeignKey(
                name: "FK_mentorEgitimleris_mentors_MentorId",
                table: "mentorEgitimleris",
                column: "MentorId",
                principalTable: "mentors",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mentorEgitimleris_mentors_MentorId",
                table: "mentorEgitimleris");

            migrationBuilder.DropIndex(
                name: "IX_mentorEgitimleris_MentorId",
                table: "mentorEgitimleris");

            migrationBuilder.DropColumn(
                name: "MentorId",
                table: "mentorEgitimleris");
        }
    }
}
