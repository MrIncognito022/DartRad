using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class contentcreatorinvites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvitedBy",
                table: "ContentCreators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ContentCreatorInvites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvitationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvitedByAdminId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentCreatorInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentCreatorInvites_Admins_InvitedByAdminId",
                        column: x => x.InvitedByAdminId,
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "SuperAdmins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Name", "Password" },
                values: new object[] { new DateTime(2023, 4, 10, 19, 44, 7, 159, DateTimeKind.Local).AddTicks(2612), "Test Super Editor", "AQAAAAEAACcQAAAAEBJoxWZpm0SqClOKrJbulB8zlxGRRLj7by1/tjjy4ZS+PzkFgVt4u6nlnggJnX+nig==" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentCreators_InvitedBy",
                table: "ContentCreators",
                column: "InvitedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_CreatedBy",
                table: "Admins",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentCreatorInvites_InvitedByAdminId",
                table: "ContentCreatorInvites",
                column: "InvitedByAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_SuperAdmins_CreatedBy",
                table: "Admins",
                column: "CreatedBy",
                principalTable: "SuperAdmins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentCreators_Admins_InvitedBy",
                table: "ContentCreators",
                column: "InvitedBy",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_SuperAdmins_CreatedBy",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentCreators_Admins_InvitedBy",
                table: "ContentCreators");

            migrationBuilder.DropTable(
                name: "ContentCreatorInvites");

            migrationBuilder.DropIndex(
                name: "IX_ContentCreators_InvitedBy",
                table: "ContentCreators");

            migrationBuilder.DropIndex(
                name: "IX_Admins_CreatedBy",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "InvitedBy",
                table: "ContentCreators");

            migrationBuilder.UpdateData(
                table: "SuperAdmins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Name", "Password" },
                values: new object[] { new DateTime(2023, 4, 10, 12, 41, 52, 528, DateTimeKind.Local).AddTicks(3465), "Test Editor", "AQAAAAEAACcQAAAAEHkZCeUT3/OOcLPfiwJlsUHXv2KoTl2dFr31QMQ6K2KO+XDo8sPm4rvzly9OKdLCug==" });
        }
    }
}
