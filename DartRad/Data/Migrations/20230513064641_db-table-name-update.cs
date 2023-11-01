using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DartRad.Migrations
{
    public partial class dbtablenameupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_SuperAdmins_CreatedBy",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentCreatorInvites_Admins_InvitedByAdminId",
                table: "ContentCreatorInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentCreators_Admins_InvitedBy",
                table: "ContentCreators");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizNotes_Admins_AdminId",
                table: "QuizNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizNotes_Quizzes_QuizId",
                table: "QuizNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Admins_ApprovedBy",
                table: "Quizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_ContentCreators_ContentCreatorId",
                table: "Quizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SuperAdmins",
                table: "SuperAdmins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResetPasswordRequests",
                table: "ResetPasswordRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quizzes",
                table: "Quizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizNotes",
                table: "QuizNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentCreators",
                table: "ContentCreators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentCreatorInvites",
                table: "ContentCreatorInvites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Admins",
                table: "Admins");

            migrationBuilder.RenameTable(
                name: "SuperAdmins",
                newName: "SuperAdmin");

            migrationBuilder.RenameTable(
                name: "ResetPasswordRequests",
                newName: "ResetPasswordRequest");

            migrationBuilder.RenameTable(
                name: "Quizzes",
                newName: "Quiz");

            migrationBuilder.RenameTable(
                name: "QuizNotes",
                newName: "QuizNote");

            migrationBuilder.RenameTable(
                name: "ContentCreators",
                newName: "ContentCreator");

            migrationBuilder.RenameTable(
                name: "ContentCreatorInvites",
                newName: "ContentCreatorInvite");

            migrationBuilder.RenameTable(
                name: "Admins",
                newName: "Editor");

            migrationBuilder.RenameIndex(
                name: "IX_Quizzes_ContentCreatorId",
                table: "Quiz",
                newName: "IX_Quiz_ContentCreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Quizzes_ApprovedBy",
                table: "Quiz",
                newName: "IX_Quiz_ApprovedBy");

            migrationBuilder.RenameIndex(
                name: "IX_QuizNotes_QuizId",
                table: "QuizNote",
                newName: "IX_QuizNote_QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizNotes_AdminId",
                table: "QuizNote",
                newName: "IX_QuizNote_AdminId");

            migrationBuilder.RenameIndex(
                name: "IX_ContentCreators_InvitedBy",
                table: "ContentCreator",
                newName: "IX_ContentCreator_InvitedBy");

            migrationBuilder.RenameIndex(
                name: "IX_ContentCreatorInvites_InvitedByAdminId",
                table: "ContentCreatorInvite",
                newName: "IX_ContentCreatorInvite_InvitedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Admins_CreatedBy",
                table: "Editor",
                newName: "IX_Admin_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SuperAdmin",
                table: "SuperAdmin",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResetPasswordRequest",
                table: "ResetPasswordRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quiz",
                table: "Quiz",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizNote",
                table: "QuizNote",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentCreator",
                table: "ContentCreator",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentCreatorInvite",
                table: "ContentCreatorInvite",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Admin",
                table: "Editor",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "SuperAdmin",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 5, 13, 11, 46, 41, 621, DateTimeKind.Local).AddTicks(6558), "AQAAAAEAACcQAAAAECu/zJo9i8033V5jnFV7Q1y8BKDLUsUNVPIh24oxXDIm+LVGb/cVQnhUhBABKjzs7w==" });

            migrationBuilder.AddForeignKey(
                name: "FK_Admin_SuperAdmin_CreatedBy",
                table: "Editor",
                column: "CreatedBy",
                principalTable: "SuperAdmin",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentCreator_Admin_InvitedBy",
                table: "ContentCreator",
                column: "InvitedBy",
                principalTable: "Editor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentCreatorInvite_Admin_InvitedByAdminId",
                table: "ContentCreatorInvite",
                column: "InvitedByAdminId",
                principalTable: "Editor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_Admin_ApprovedBy",
                table: "Quiz",
                column: "ApprovedBy",
                principalTable: "Editor",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_ContentCreator_ContentCreatorId",
                table: "Quiz",
                column: "ContentCreatorId",
                principalTable: "ContentCreator",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizNote_Admin_AdminId",
                table: "QuizNote",
                column: "AdminId",
                principalTable: "Editor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizNote_Quiz_QuizId",
                table: "QuizNote",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admin_SuperAdmin_CreatedBy",
                table: "Editor");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentCreator_Admin_InvitedBy",
                table: "ContentCreator");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentCreatorInvite_Admin_InvitedByAdminId",
                table: "ContentCreatorInvite");

            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_Admin_ApprovedBy",
                table: "Quiz");

            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_ContentCreator_ContentCreatorId",
                table: "Quiz");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizNote_Admin_AdminId",
                table: "QuizNote");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizNote_Quiz_QuizId",
                table: "QuizNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SuperAdmin",
                table: "SuperAdmin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResetPasswordRequest",
                table: "ResetPasswordRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizNote",
                table: "QuizNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quiz",
                table: "Quiz");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentCreatorInvite",
                table: "ContentCreatorInvite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentCreator",
                table: "ContentCreator");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Admin",
                table: "Editor");

            migrationBuilder.RenameTable(
                name: "SuperAdmin",
                newName: "SuperAdmins");

            migrationBuilder.RenameTable(
                name: "ResetPasswordRequest",
                newName: "ResetPasswordRequests");

            migrationBuilder.RenameTable(
                name: "QuizNote",
                newName: "QuizNotes");

            migrationBuilder.RenameTable(
                name: "Quiz",
                newName: "Quizzes");

            migrationBuilder.RenameTable(
                name: "ContentCreatorInvite",
                newName: "ContentCreatorInvites");

            migrationBuilder.RenameTable(
                name: "ContentCreator",
                newName: "ContentCreators");

            migrationBuilder.RenameTable(
                name: "Editor",
                newName: "Admins");

            migrationBuilder.RenameIndex(
                name: "IX_QuizNote_QuizId",
                table: "QuizNotes",
                newName: "IX_QuizNotes_QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizNote_AdminId",
                table: "QuizNotes",
                newName: "IX_QuizNotes_AdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Quiz_ContentCreatorId",
                table: "Quizzes",
                newName: "IX_Quizzes_ContentCreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Quiz_ApprovedBy",
                table: "Quizzes",
                newName: "IX_Quizzes_ApprovedBy");

            migrationBuilder.RenameIndex(
                name: "IX_ContentCreatorInvite_InvitedByAdminId",
                table: "ContentCreatorInvites",
                newName: "IX_ContentCreatorInvites_InvitedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_ContentCreator_InvitedBy",
                table: "ContentCreators",
                newName: "IX_ContentCreators_InvitedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Admin_CreatedBy",
                table: "Admins",
                newName: "IX_Admins_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SuperAdmins",
                table: "SuperAdmins",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResetPasswordRequests",
                table: "ResetPasswordRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizNotes",
                table: "QuizNotes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quizzes",
                table: "Quizzes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentCreatorInvites",
                table: "ContentCreatorInvites",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentCreators",
                table: "ContentCreators",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Admins",
                table: "Admins",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "SuperAdmins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2023, 5, 2, 13, 23, 27, 95, DateTimeKind.Local).AddTicks(7404), "AQAAAAEAACcQAAAAEL4+hsL2lQVx6ix6sCDq4BWDGBh0UnDf74adxx3mxmKl57oiDlaFtgv4Wc2gUKzzyg==" });

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_SuperAdmins_CreatedBy",
                table: "Admins",
                column: "CreatedBy",
                principalTable: "SuperAdmins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentCreatorInvites_Admins_InvitedByAdminId",
                table: "ContentCreatorInvites",
                column: "InvitedByAdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentCreators_Admins_InvitedBy",
                table: "ContentCreators",
                column: "InvitedBy",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizNotes_Admins_AdminId",
                table: "QuizNotes",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizNotes_Quizzes_QuizId",
                table: "QuizNotes",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Admins_ApprovedBy",
                table: "Quizzes",
                column: "ApprovedBy",
                principalTable: "Admins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_ContentCreators_ContentCreatorId",
                table: "Quizzes",
                column: "ContentCreatorId",
                principalTable: "ContentCreators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
