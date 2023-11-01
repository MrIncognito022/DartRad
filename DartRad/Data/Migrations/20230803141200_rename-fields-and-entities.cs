using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class renamefieldsandentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Quiz",
                newName: "ClinicalScenario");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Quiz",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Quiz",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 8, 3, 19, 11, 59, 914, DateTimeKind.Local).AddTicks(5709), "AQAAAAEAACcQAAAAELmzX48Gae3v9CX3nibau2ysoLExVD2oMKKq2VdqzUbtITDqVOQY9aP38jIt8xtWSA==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Quiz");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Quiz");

            migrationBuilder.RenameColumn(
                name: "ClinicalScenario",
                table: "Quiz",
                newName: "Description");

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 7, 1, 19, 52, 32, 208, DateTimeKind.Local).AddTicks(4488), "AQAAAAEAACcQAAAAEIQQkIgJJX/ghi5+mHQot/GKoPY/OyQkPokPxTMNNv9+PCm/cBFMguyaCK2a6jQZmw==" });
        }
    }
}
