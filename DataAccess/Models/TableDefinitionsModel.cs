﻿namespace DataAccess.Models;

public class TableDefinitionsModel
{
    public int Id { get; set; }
    public string TableName { get; set; }
    public int CollectionTypesId { get; set; }
    public ulong RoleId { get; set; }
}