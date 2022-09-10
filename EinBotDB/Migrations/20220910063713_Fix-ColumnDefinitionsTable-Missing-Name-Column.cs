using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EinBotDB.Migrations
{
    public partial class FixColumnDefinitionsTableMissingNameColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ColumnDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ColumnDefinitions");
        }
    }
}
