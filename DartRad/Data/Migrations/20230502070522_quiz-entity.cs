using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class quizentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentCreatorId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_Admins_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quizzes_ContentCreators_ContentCreatorId",
                        column: x => x.ContentCreatorId,
                        principalTable: "ContentCreators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuizNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizNotes_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizNotes_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "SuperAdmins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 5, 2, 12, 5, 21, 966, DateTimeKind.Local).AddTicks(5262), "AQAAAAEAACcQAAAAEK0boBt+1jnn/bOtyAwaBfLvZ+aBD80lqiGXsmAXkyHxYQauIwDGXc6DFbOkyVuogg==" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizNotes_AdminId",
                table: "QuizNotes",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizNotes_QuizId",
                table: "QuizNotes",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_ApprovedBy",
                table: "Quizzes",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_ContentCreatorId",
                table: "Quizzes",
                column: "ContentCreatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizNotes");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.UpdateData(
                table: "SuperAdmins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 4, 10, 20, 49, 28, 25, DateTimeKind.Local).AddTicks(9489), "AQAAAAEAACcQAAAAED4M6WkyXNfjf/4ZjQPAqTdYdU5BgGHu1X3IaivoQss9+vakPYMNGVsr2Lt0rQyUvw==" });
        }
    }
}
