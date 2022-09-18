namespace EinBotDB.Context;

using EinBotDB.Models;
using Microsoft.EntityFrameworkCore;

public class EinDataContext : DbContext
{
    public EinDataContext(DbContextOptions contextOptions) : base(contextOptions) { }

    public DbSet<DataTypesModel> DataTypes { get; set; }
    public DbSet<CollectionTypesModel> CollectionTypes { get; set; }

    public DbSet<TableDefinitionsModel> TableDefinitions { get; set; }
    public DbSet<ColumnDefinitionsModel> ColumnDefinitions { get; set; }
    public DbSet<CellsModel> Cells { get; set; }

    public DbSet<EinEmbedPartsModel> EinEmbedParts { get; set; }
    public DbSet<EmbedPartTypesModel> EmbedPartTypes { get; set; }
}
