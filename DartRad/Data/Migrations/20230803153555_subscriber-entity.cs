using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class subscriberentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subscriber",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriber_Editor_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Editor",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 8, 3, 20, 35, 55, 210, DateTimeKind.Local).AddTicks(4058), "AQAAAAEAACcQAAAAEITNiIQtfRBKjkwtkKRok2xa0o5M68ExLEf775YHx2pKzdhrKwsG8ZhAJFRaPn2j3A==" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriber_CreatedBy",
                table: "Subscriber",
                column: "CreatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscriber");

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 8, 3, 19, 11, 59, 914, DateTimeKind.Local).AddTicks(5709), "AQAAAAEAACcQAAAAELmzX48Gae3v9CX3nibau2ysoLExVD2oMKKq2VdqzUbtITDqVOQY9aP38jIt8xtWSA==" });
        }
    }
}
