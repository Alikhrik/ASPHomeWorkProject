using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserRegFormHWv01.Migrations
{
    public partial class TopicDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b930d240-2eac-46d8-9783-dcfa2a2e0e9b"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Topics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastArticleMoment",
                table: "Topics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("03dc041c-30fb-4de9-a956-14e5d2a7aba4"), "", "", null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 8, 22, 18, 31, 38, 364, DateTimeKind.Local).AddTicks(4291) });

            migrationBuilder.CreateIndex(
                name: "IX_Topics_AuthorId",
                table: "Topics",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_TopicId",
                table: "Articles",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Topics_TopicId",
                table: "Articles",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Users_AuthorId",
                table: "Topics",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Topics_TopicId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Users_AuthorId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_AuthorId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Articles_TopicId",
                table: "Articles");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("03dc041c-30fb-4de9-a956-14e5d2a7aba4"));

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "LastArticleMoment",
                table: "Topics");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("b930d240-2eac-46d8-9783-dcfa2a2e0e9b"), "", "", null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 8, 17, 20, 43, 23, 810, DateTimeKind.Local).AddTicks(3115) });
        }
    }
}
