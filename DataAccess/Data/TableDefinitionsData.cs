﻿namespace DataAccess.Data;

using DataAccess.DbAccess;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TableDefinitionsData: ITableDefinitionsData
{
    private readonly IRelationalDataAccess _db;

    public TableDefinitionsData(IRelationalDataAccess db)
    {
        _db = db;
    }


    public Task<IEnumerable<TableDefinitionsModel>> GetTableDefinitions() =>
        _db.LoadData<TableDefinitionsModel, dynamic>(storedProcedure: "dbo.spTableDefinitions_GetAll", new { });

    public async Task<TableDefinitionsModel?> GetTableDefinition(int id)
    {
        var results = await _db.LoadData<TableDefinitionsModel, dynamic>(
            storedProcedure: "dbo.spTableDefinitions_Get",
            new { Id = id });

        return results.FirstOrDefault();
    }

    public async Task<TableDefinitionsModel?> GetTableDefinitionByName(string tableName)
    {
        var results = await _db.LoadData<TableDefinitionsModel, dynamic>(
            storedProcedure: "dbo.spTableDefinitions_GetByName",
            new { TableName = tableName });

        return results.FirstOrDefault();
    }

    public Task InsertTableDefinition(TableDefinitionsModel tableDefinition) =>
        _db.SaveData(
            storedProcedure: "dbo.spTableDefinitions_Insert",
            new
            {
                tableDefinition.TableName,
                tableDefinition.CollectionTypesId,
                tableDefinition.RoleId,
            });

    public Task UpdateTableDefinition(TableDefinitionsModel tableDefinition) =>
        _db.SaveData(storedProcedure: "dbo.spTableDefinitions_Update", tableDefinition);

    public Task DeleteTableDefinition(int id) =>
        _db.SaveData(storedProcedure: "dbo.spTableDefinitions_Delete", new { Id = id });

}
