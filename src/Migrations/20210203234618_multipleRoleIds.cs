using Microsoft.EntityFrameworkCore.Migrations;

namespace InactiviteRoleRemover.Migrations
{
    public partial class multipleRoleIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleIdToRestore",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "RoleIdsToRestore",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleIdsToRestore",
                table: "Users");

            migrationBuilder.AddColumn<decimal>(
                name: "RoleIdToRestore",
                table: "Users",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
