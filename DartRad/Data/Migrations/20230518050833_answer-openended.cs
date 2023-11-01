using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class answeropenended : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnswerOpenEnded",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerOpenEnded", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerOpenEnded_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Email", "Password" },
                values: new object[] { new DateTime(2023, 5, 18, 10, 8, 33, 266, DateTimeKind.Local).AddTicks(640), "info@somuchheart.com", "AQAAAAEAACcQAAAAEAGeLGoZRSCrjAwqtSKngj5emtAYGGpRLcoEDpwiC10zbvbxxxOkzxKZwsWMDPvfzg==" });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOpenEnded_QuestionId",
                table: "AnswerOpenEnded",
                column: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerOpenEnded");

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Email", "Password" },
                values: new object[] { new DateTime(2023, 5, 13, 12, 12, 54, 516, DateTimeKind.Local).AddTicks(7716), "superadmin@dr.com", "AQAAAAEAACcQAAAAEE8D6mfvX/DpVgEWxqBepAV1Zthx6hK9N7sYsIZ/kcRw0vnLeYNovWH925frb/FDUg==" });
        }
    }
}
