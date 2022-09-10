using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EinBotDB.Migrations
{
    public partial class FixCellsModelAddMissingRowNumColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RowNum",
                table: "Cells",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowNum",
                table: "Cells");
        }
    }
}
