using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InactiviteRoleRemover.Migrations
{
    public partial class seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DiscordId", "LastActivity", "RoleIdsToRestore" },
                values: new object[] { new Guid("727806cf-2679-4645-ae3d-d07a26d93354"), 123m, new DateTime(2021, 2, 3, 19, 9, 26, 287, DateTimeKind.Local).AddTicks(9141), null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("727806cf-2679-4645-ae3d-d07a26d93354"));
        }
    }
}
