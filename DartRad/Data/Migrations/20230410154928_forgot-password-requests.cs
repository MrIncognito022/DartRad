using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class forgotpasswordrequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResetPasswordRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetPasswordRequests", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "SuperAdmins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 4, 10, 20, 49, 28, 25, DateTimeKind.Local).AddTicks(9489), "AQAAAAEAACcQAAAAED4M6WkyXNfjf/4ZjQPAqTdYdU5BgGHu1X3IaivoQss9+vakPYMNGVsr2Lt0rQyUvw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResetPasswordRequests");

            migrationBuilder.UpdateData(
                table: "SuperAdmins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 4, 10, 19, 44, 7, 159, DateTimeKind.Local).AddTicks(2612), "AQAAAAEAACcQAAAAEBJoxWZpm0SqClOKrJbulB8zlxGRRLj7by1/tjjy4ZS+PzkFgVt4u6nlnggJnX+nig==" });
        }
    }
}
