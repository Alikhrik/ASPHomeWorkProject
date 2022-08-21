using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserRegFormHWv01.Migrations
{
    public partial class Relations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("81e6e2d8-34de-4596-ba82-e5e49477507a"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("b930d240-2eac-46d8-9783-dcfa2a2e0e9b"), "", "", null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 8, 17, 20, 43, 23, 810, DateTimeKind.Local).AddTicks(3115) });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Users_AuthorId",
                table: "Articles",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Users_AuthorId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b930d240-2eac-46d8-9783-dcfa2a2e0e9b"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("81e6e2d8-34de-4596-ba82-e5e49477507a"), "", "", null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 8, 1, 21, 55, 57, 94, DateTimeKind.Local).AddTicks(601) });
        }
    }
}
