using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class Quizentityupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Admins_ApprovedBy",
                table: "Quizzes");

            migrationBuilder.AlterColumn<int>(
                name: "ApprovedBy",
                table: "Quizzes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "SuperAdmins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 5, 2, 13, 23, 27, 95, DateTimeKind.Local).AddTicks(7404), "AQAAAAEAACcQAAAAEL4+hsL2lQVx6ix6sCDq4BWDGBh0UnDf74adxx3mxmKl57oiDlaFtgv4Wc2gUKzzyg==" });

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Admins_ApprovedBy",
                table: "Quizzes",
                column: "ApprovedBy",
                principalTable: "Admins",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Admins_ApprovedBy",
                table: "Quizzes");

            migrationBuilder.AlterColumn<int>(
                name: "ApprovedBy",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "SuperAdmins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 5, 2, 12, 5, 21, 966, DateTimeKind.Local).AddTicks(5262), "AQAAAAEAACcQAAAAEK0boBt+1jnn/bOtyAwaBfLvZ+aBD80lqiGXsmAXkyHxYQauIwDGXc6DFbOkyVuogg==" });

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Admins_ApprovedBy",
                table: "Quizzes",
                column: "ApprovedBy",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
