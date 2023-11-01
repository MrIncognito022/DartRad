using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class MatchingAnswerEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnswerMatching",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeftSide = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RightSide = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerMatching", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerMatching_Question_QuestionId",
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
                values: new object[] { new DateTime(2023, 5, 28, 13, 59, 58, 497, DateTimeKind.Local).AddTicks(6101), "AQAAAAEAACcQAAAAEMmJ76LYdQKEYI45K7YcJhuJgJUUt0ID1fGuk0qTG3N4UlyMxNoUWSRwKXb1Ixo5iQ==" });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerMatching_QuestionId",
                table: "AnswerMatching",
                column: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerMatching");

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 5, 24, 23, 41, 48, 427, DateTimeKind.Local).AddTicks(48), "AQAAAAEAACcQAAAAEHWWSAy+LtDjvzZN2ieAzXJyP/B/Y6YC0Gwjig6KpopcPhhMEPzylRvMwoybyLJR0g==" });
        }
    }
}
