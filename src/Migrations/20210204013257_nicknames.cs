using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InactiviteRoleRemover.Migrations
{
    public partial class nicknames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("727806cf-2679-4645-ae3d-d07a26d93354"));

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DiscordId", "LastActivity", "Nickname", "RoleIdsToRestore" },
                values: new object[] { new Guid("80c640b5-2080-4da0-8de3-b7f8c07d3271"), 123m, new DateTime(2021, 2, 3, 20, 32, 57, 238, DateTimeKind.Local).AddTicks(3703), null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("80c640b5-2080-4da0-8de3-b7f8c07d3271"));

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DiscordId", "LastActivity", "RoleIdsToRestore" },
                values: new object[] { new Guid("727806cf-2679-4645-ae3d-d07a26d93354"), 123m, new DateTime(2021, 2, 3, 19, 9, 26, 287, DateTimeKind.Local).AddTicks(9141), null });
        }
    }
}
