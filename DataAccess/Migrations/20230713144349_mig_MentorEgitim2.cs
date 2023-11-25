using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class mig_MentorEgitim2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mentorEgitimleris_AspNetUsers_AppUserId",
                table: "mentorEgitimleris");

            migrationBuilder.DropForeignKey(
                name: "FK_mentorEgitimleris_mentors_MentorId",
                table: "mentorEgitimleris");

            migrationBuilder.AlterColumn<int>(
                name: "MentorId",
                table: "mentorEgitimleris",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "mentorEgitimleris",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mentorEgitimleris_AspNetUsers_AppUserId",
                table: "mentorEgitimleris");

            migrationBuilder.DropForeignKey(
                name: "FK_mentorEgitimleris_mentors_MentorId",
                table: "mentorEgitimleris");

            migrationBuilder.AlterColumn<int>(
                name: "MentorId",
                table: "mentorEgitimleris",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "mentorEgitimleris",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_mentorEgitimleris_AspNetUsers_AppUserId",
                table: "mentorEgitimleris",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mentorEgitimleris_mentors_MentorId",
                table: "mentorEgitimleris",
                column: "MentorId",
                principalTable: "mentors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
