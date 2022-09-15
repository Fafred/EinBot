using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EinBotDB.Migrations
{
    public partial class AddEmbedPartsAndEmbedPartTypesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "RoleId",
                table: "TableDefinitions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateTable(
                name: "EmbedPartTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbedPartTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EinEmbedParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TableDefinitionsId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmbedPartTypesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    Data01 = table.Column<string>(type: "TEXT", nullable: true),
                    Data02 = table.Column<string>(type: "TEXT", nullable: true),
                    Data03 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EinEmbedParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EinEmbedParts_EmbedPartTypes_EmbedPartTypesId",
                        column: x => x.EmbedPartTypesId,
                        principalTable: "EmbedPartTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EinEmbedParts_TableDefinitions_TableDefinitionsId",
                        column: x => x.TableDefinitionsId,
                        principalTable: "TableDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TableDefinitions_Name",
                table: "TableDefinitions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableDefinitions_RoleId",
                table: "TableDefinitions",
                column: "RoleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EinEmbedParts_EmbedPartTypesId",
                table: "EinEmbedParts",
                column: "EmbedPartTypesId");

            migrationBuilder.CreateIndex(
                name: "IX_EinEmbedParts_TableDefinitionsId",
                table: "EinEmbedParts",
                column: "TableDefinitionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EinEmbedParts");

            migrationBuilder.DropTable(
                name: "EmbedPartTypes");

            migrationBuilder.DropIndex(
                name: "IX_TableDefinitions_Name",
                table: "TableDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_TableDefinitions_RoleId",
                table: "TableDefinitions");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "TableDefinitions");
        }
    }
}
