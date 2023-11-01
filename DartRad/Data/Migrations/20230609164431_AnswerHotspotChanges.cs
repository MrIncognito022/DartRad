using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class AnswerHotspotChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HotspotQuestionImages_QuestionId",
                table: "HotspotQuestionImages");

            migrationBuilder.RenameColumn(
                name: "CorrectAreaY",
                table: "AnswerHotspot",
                newName: "Y");

            migrationBuilder.RenameColumn(
                name: "CorrectAreaX",
                table: "AnswerHotspot",
                newName: "X");

            migrationBuilder.RenameColumn(
                name: "CorrectAreaWidth",
                table: "AnswerHotspot",
                newName: "Width");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "AnswerHotspot",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 6, 9, 21, 44, 31, 564, DateTimeKind.Local).AddTicks(7022), "AQAAAAEAACcQAAAAEOUwfh/BOOxMARWZeK7dN0I/wyn4XGj+RdeajCre4O4QqBw986Frd5NPwg2/XYpQpw==" });

            migrationBuilder.CreateIndex(
                name: "IX_HotspotQuestionImages_QuestionId",
                table: "HotspotQuestionImages",
                column: "QuestionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HotspotQuestionImages_QuestionId",
                table: "HotspotQuestionImages");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "AnswerHotspot");

            migrationBuilder.RenameColumn(
                name: "Y",
                table: "AnswerHotspot",
                newName: "CorrectAreaY");

            migrationBuilder.RenameColumn(
                name: "X",
                table: "AnswerHotspot",
                newName: "CorrectAreaX");

            migrationBuilder.RenameColumn(
                name: "Width",
                table: "AnswerHotspot",
                newName: "CorrectAreaWidth");

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 6, 7, 0, 30, 19, 154, DateTimeKind.Local).AddTicks(5294), "AQAAAAEAACcQAAAAEOuQeE5SLsq7gAyF5GsLWz8OZDyixQ3sq5jcqcqs9NzQmYjQKwe5RaqiKa3PbVVS8g==" });

            migrationBuilder.CreateIndex(
                name: "IX_HotspotQuestionImages_QuestionId",
                table: "HotspotQuestionImages",
                column: "QuestionId");
        }
    }
}
