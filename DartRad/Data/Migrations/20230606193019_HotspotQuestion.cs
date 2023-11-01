using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class HotspotQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnswerExplanation",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AnswerHotspot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrectAreaX = table.Column<int>(type: "int", nullable: false),
                    CorrectAreaY = table.Column<int>(type: "int", nullable: false),
                    CorrectAreaWidth = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerHotspot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerHotspot_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotspotQuestionImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotspotQuestionImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotspotQuestionImages_Question_QuestionId",
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
                values: new object[] { new DateTime(2023, 6, 7, 0, 30, 19, 154, DateTimeKind.Local).AddTicks(5294), "AQAAAAEAACcQAAAAEOuQeE5SLsq7gAyF5GsLWz8OZDyixQ3sq5jcqcqs9NzQmYjQKwe5RaqiKa3PbVVS8g==" });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerHotspot_QuestionId",
                table: "AnswerHotspot",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_HotspotQuestionImages_QuestionId",
                table: "HotspotQuestionImages",
                column: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerHotspot");

            migrationBuilder.DropTable(
                name: "HotspotQuestionImages");

            migrationBuilder.DropColumn(
                name: "AnswerExplanation",
                table: "Question");

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 5, 28, 13, 59, 58, 497, DateTimeKind.Local).AddTicks(6101), "AQAAAAEAACcQAAAAEMmJ76LYdQKEYI45K7YcJhuJgJUUt0ID1fGuk0qTG3N4UlyMxNoUWSRwKXb1Ixo5iQ==" });
        }
    }
}
