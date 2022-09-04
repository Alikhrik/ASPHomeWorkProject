using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserRegFormHWv01.Migrations
{
    public partial class ArticleDeleteMoment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("03dc041c-30fb-4de9-a956-14e5d2a7aba4"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteMoment",
                table: "Articles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("9564a2ca-e2f6-4813-8463-531630105296"), "", "", null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 9, 4, 17, 44, 33, 706, DateTimeKind.Local).AddTicks(7347) });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ReplyId",
                table: "Articles",
                column: "ReplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Articles_ReplyId",
                table: "Articles",
                column: "ReplyId",
                principalTable: "Articles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Articles_ReplyId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ReplyId",
                table: "Articles");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("9564a2ca-e2f6-4813-8463-531630105296"));

            migrationBuilder.DropColumn(
                name: "DeleteMoment",
                table: "Articles");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("03dc041c-30fb-4de9-a956-14e5d2a7aba4"), "", "", null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 8, 22, 18, 31, 38, 364, DateTimeKind.Local).AddTicks(4291) });
        }
    }
}
