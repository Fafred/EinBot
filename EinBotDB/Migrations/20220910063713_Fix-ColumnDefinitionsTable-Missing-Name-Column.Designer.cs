// <auto-generated />
using EinBotDB.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EinBotDB.Migrations
{
    [DbContext(typeof(EinDataContext))]
    [Migration("20220910063713_Fix-ColumnDefinitionsTable-Missing-Name-Column")]
    partial class FixColumnDefinitionsTableMissingNameColumn
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

            modelBuilder.Entity("EinBotDB.Models.CellsModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ColumnDefinitionsId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Data")
                        .HasColumnType("TEXT");

                    b.Property<string>("RowKey")
                        .HasColumnType("TEXT");

                    b.Property<int>("TableDefinitionsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ColumnDefinitionsId");

                    b.HasIndex("TableDefinitionsId");

                    b.ToTable("Cells");
                });

            modelBuilder.Entity("EinBotDB.Models.CollectionTypesModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CollectionTypes");
                });

            modelBuilder.Entity("EinBotDB.Models.ColumnDefinitionsModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DataTypesId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TableDefinitionsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DataTypesId");

                    b.HasIndex("TableDefinitionsId");

                    b.ToTable("ColumnDefinitions");
                });

            modelBuilder.Entity("EinBotDB.Models.DataTypesModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DataTypes");
                });

            modelBuilder.Entity("EinBotDB.Models.TableDefinitionsModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CollectionTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CollectionTypeId");

                    b.ToTable("TableDefinitions");
                });

            modelBuilder.Entity("EinBotDB.Models.CellsModel", b =>
                {
                    b.HasOne("EinBotDB.Models.ColumnDefinitionsModel", "ColumnDefinitions")
                        .WithMany("Cells")
                        .HasForeignKey("ColumnDefinitionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EinBotDB.Models.TableDefinitionsModel", "TableDefinitions")
                        .WithMany("Cells")
                        .HasForeignKey("TableDefinitionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ColumnDefinitions");

                    b.Navigation("TableDefinitions");
                });

            modelBuilder.Entity("EinBotDB.Models.ColumnDefinitionsModel", b =>
                {
                    b.HasOne("EinBotDB.Models.DataTypesModel", "DataTypes")
                        .WithMany("ColumnDefinitions")
                        .HasForeignKey("DataTypesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EinBotDB.Models.TableDefinitionsModel", "TableDefinitions")
                        .WithMany("ColumnDefinitions")
                        .HasForeignKey("TableDefinitionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DataTypes");

                    b.Navigation("TableDefinitions");
                });

            modelBuilder.Entity("EinBotDB.Models.TableDefinitionsModel", b =>
                {
                    b.HasOne("EinBotDB.Models.CollectionTypesModel", "CollectionType")
                        .WithMany("TableDefinitions")
                        .HasForeignKey("CollectionTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CollectionType");
                });

            modelBuilder.Entity("EinBotDB.Models.CollectionTypesModel", b =>
                {
                    b.Navigation("TableDefinitions");
                });

            modelBuilder.Entity("EinBotDB.Models.ColumnDefinitionsModel", b =>
                {
                    b.Navigation("Cells");
                });

            modelBuilder.Entity("EinBotDB.Models.DataTypesModel", b =>
                {
                    b.Navigation("ColumnDefinitions");
                });

            modelBuilder.Entity("EinBotDB.Models.TableDefinitionsModel", b =>
                {
                    b.Navigation("Cells");

                    b.Navigation("ColumnDefinitions");
                });
#pragma warning restore 612, 618
        }
    }
}
