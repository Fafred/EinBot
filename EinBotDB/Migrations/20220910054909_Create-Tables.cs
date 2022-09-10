using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EinBotDB.Migrations
{
    public partial class CreateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CollectionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TableDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CollectionTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableDefinitions_CollectionTypes_CollectionTypeId",
                        column: x => x.CollectionTypeId,
                        principalTable: "CollectionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColumnDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TableDefinitionsId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataTypesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColumnDefinitions_DataTypes_DataTypesId",
                        column: x => x.DataTypesId,
                        principalTable: "DataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColumnDefinitions_TableDefinitions_TableDefinitionsId",
                        column: x => x.TableDefinitionsId,
                        principalTable: "TableDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TableDefinitionsId = table.Column<int>(type: "INTEGER", nullable: false),
                    ColumnDefinitionsId = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    RowKey = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cells_ColumnDefinitions_ColumnDefinitionsId",
                        column: x => x.ColumnDefinitionsId,
                        principalTable: "ColumnDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cells_TableDefinitions_TableDefinitionsId",
                        column: x => x.TableDefinitionsId,
                        principalTable: "TableDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cells_ColumnDefinitionsId",
                table: "Cells",
                column: "ColumnDefinitionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Cells_TableDefinitionsId",
                table: "Cells",
                column: "TableDefinitionsId");

            migrationBuilder.CreateIndex(
                name: "IX_ColumnDefinitions_DataTypesId",
                table: "ColumnDefinitions",
                column: "DataTypesId");

            migrationBuilder.CreateIndex(
                name: "IX_ColumnDefinitions_TableDefinitionsId",
                table: "ColumnDefinitions",
                column: "TableDefinitionsId");

            migrationBuilder.CreateIndex(
                name: "IX_TableDefinitions_CollectionTypeId",
                table: "TableDefinitions",
                column: "CollectionTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cells");

            migrationBuilder.DropTable(
                name: "ColumnDefinitions");

            migrationBuilder.DropTable(
                name: "DataTypes");

            migrationBuilder.DropTable(
                name: "TableDefinitions");

            migrationBuilder.DropTable(
                name: "CollectionTypes");
        }
    }
}
