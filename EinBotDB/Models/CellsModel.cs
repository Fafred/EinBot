﻿namespace EinBotDB.Models;

using System.ComponentModel.DataAnnotations;

public class CellsModel
{
    [Key]
    public int Id { get; set; }
    
    public int TableDefinitionsId { get; set; }
    public TableDefinitionsModel TableDefinitions { get; set; }

    public int ColumnDefinitionsId { get; set; }
    public ColumnDefinitionsModel ColumnDefinitions { get; set; }

    public int RowNum { get; set; }

    public string? Data { get; set; }

    public string? RowKey { get; set; }
}