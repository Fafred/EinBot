using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EinBotDB.Migrations
{
    public partial class FixTableDefinitionsModelAddMissingRoleIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "RoleId",
                table: "TableDefinitions",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "TableDefinitions");
        }
    }
}
