using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class answersequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnswerSequence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerSequence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerSequence_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 7, 1, 19, 52, 32, 208, DateTimeKind.Local).AddTicks(4488), "AQAAAAEAACcQAAAAEIQQkIgJJX/ghi5+mHQot/GKoPY/OyQkPokPxTMNNv9+PCm/cBFMguyaCK2a6jQZmw==" });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSequence_QuestionId",
                table: "AnswerSequence",
                column: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerSequence");

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 6, 16, 17, 23, 52, 880, DateTimeKind.Local).AddTicks(558), "AQAAAAEAACcQAAAAEIOK6OeBMvSuHesE+SNVYZrEYzE+kRH2B4V0zD5DzrOTbH91wEEy8J9S+YyzjRxe7g==" });
        }
    }
}
