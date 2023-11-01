using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class superadminpasswordchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 6, 16, 17, 23, 52, 880, DateTimeKind.Local).AddTicks(558), "AQAAAAEAACcQAAAAEIOK6OeBMvSuHesE+SNVYZrEYzE+kRH2B4V0zD5DzrOTbH91wEEy8J9S+YyzjRxe7g==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 6, 9, 21, 44, 31, 564, DateTimeKind.Local).AddTicks(7022), "AQAAAAEAACcQAAAAEOUwfh/BOOxMARWZeK7dN0I/wyn4XGj+RdeajCre4O4QqBw986Frd5NPwg2/XYpQpw==" });
        }
    }
}
