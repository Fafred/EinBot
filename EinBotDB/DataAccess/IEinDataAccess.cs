namespace EinBotDB.DataAccess;

using EinBotDB.Models;
using System.Collections.Generic;

public interface IEinDataAccess
{
    /// <summary>
    /// Adds a part to the embed display.
    /// </summary>
    /// <param name="embedPart">The <see cref="EmbedPartsEnum">Embed Part ID</see> the part is.</param>
    /// <param name="Data01">The data in the first slot.</param>
    /// <param name="Data02">Null or the data in the first slot.</param>
    /// <param name="Data03">Null or the data in the first slot.</param>
    /// <param name="tableId">Null or the table id the display is attached to.  If null, then either role id or tablename cannot be.</param>
    /// <param name="roleId">Null or the role id the display is attached to.  If null, then either table id or tablename cannot be.</param>
    /// <param name="tableName">Null or the name the display is attached to.  If null, then either role id or table id cannot be.</param>
    /// <param name="sequence">The order # of the part.  If null will place it at the end.</param>
    /// <exception cref="TableDoesNotExistException">If no such table exists.</exception>"
    /// <returns>The id of the new <see cref="EinEmbedPartsModel"/></returns>
    int AddEmbedPart(EmbedPartsEnum embedPart, string Data01, string? Data02 = null, string? Data03 = null, int? tableId = null, ulong? roleId = null, string? tableName = null, int? sequence = null);

    /// <summary>
    /// Adds or updates a "row" to the given table.  If no dataDict is supplied, or a column isn't present in the dataDict, the default value for the column will be an empty string.
    /// </summary>
    /// <param name="rowKey">The key of the row.  This must be unique key for the table.</param>
    /// <param name="dataDict">Optional Dictionary<string, string>, where the key is the name of the column and the value is the data to put into it.</param>
    /// <param name="tableId">NULL or the id of the table to add/update a row in.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table to add/update a row in.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table to add/update a row in.  If null, then either roleId or tableId cannot be null.</param>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="InvalidKeyException">If the key is null.</exception>
    /// <exception cref="KeyAlreadyPresentInTableException">If the key is already present in the table.</exception>"
    /// <exception cref="InvalidDataException">If the data given in one of the values in the dataDict does not match the column data type.</exception>
    /// <exception cref="InvalidKeyException"></exception>
    public void AddOrUpdateRow(
        string rowKey,
        Dictionary<string, string>? dataDict = null,
        int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Adds a "row" to the given table.  If no dataDict is supplied, or a column isn't present in the dataDict, the default value for the column will be an empty string.
    /// </summary>
    /// <param name="rowKey">The key to give to the row.  This must be unique key for the table.</param>
    /// <param name="dataDict">Optional Dictionary<string, string>, where the key is the name of the column and the value is the data to put into it.</param>
    /// <param name="tableId">NULL or the id of the table to make a row in.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table to make a row in.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table to make a row in.  If null, then either roleId or tableId cannot be null.</param>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="InvalidKeyException">If the key is null.</exception>
    /// <exception cref="KeyAlreadyPresentInTableException">If the key is already present in the table.</exception>"
    /// <exception cref="InvalidDataException">If the data given in one of the values in the dataDict does not match the column data type.</exception>
    void AddRow(string rowKey, Dictionary<string, string>? dataDict = null, int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Changes the key of a given row in a table.
    /// </summary>
    /// <param name="newKey">The new key for the given row in the table.</param>
    /// <param name="tableId">NULL or the id of the table to change the row key in.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table to change the row key in.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table to change the row key in.  If null, then either roleId or tableId cannot be null.</param>
    /// <param name="rowNum"></param>
    /// <param name="rowKey"></param>
    /// <returns></returns>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="InvalidKeyException">If the new key is null, or if the rowKey and rowNum are both null.</exception>
    /// <exception cref="CellDoesNotExistException">If there are no cells with the given rowKey and rowNum.</exception>
    /// <exception cref="KeyAlreadyPresentInTableException">If the table already has cells with the newKey key.</exception>    
    string ChangeKey(string newKey, int? tableId = null, ulong? roleId = null, string? tableName = null, int? rowNum = null, string? rowKey = null);

    /// <summary>
    /// Create a new column for the given table.
    /// </summary>
    /// <param name="tableId">Null or the id of the table this column should be placed in.  If null, then either roleId or tableName must not be null.</param>
    /// <param name="roleId">Null or the role id of the table this column should be placed in.  If null, then either tableId or tableName must not be null.</param>
    /// <param name="tableName">Null or the name of the table this column should be placed in.  If null, then either tableId or roleId must not be null.</param>
    /// <param name="columnName">The name of the column.  Everything other than alpha-numerics and '-' chars are stripped out.</param>
    /// <param name="dataType">The type of data stored in this column</param>
    /// <returns>A ColumnDefinitionModel entity returned by the db context.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be added to does not exist.</exception>
    /// <exception cref="InvalidNameException">If the name of the column is invalid.</exception>
    /// <exception cref="ColumnAlreadyExistsException">If there is already a column with that name in the given table.</exception>
    ColumnDefinitionsModel CreateColumn(string columnName, DataTypesEnum dataType, int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Creates a new table.
    /// </summary>
    /// <param name="tableName">The name of the new table.  Must contain only alpha-numeric and '-' characters. </param>
    /// <param name="collectionType">The type of collection this table is.</param>
    /// <param name="roleId">The role id associated with this table.</param>
    /// <returns>The TableDefinitionsModel entity of the created table.</returns>
    /// <exception cref="InvalidNameException"></exception>
    /// <exception cref="TableAlreadyExistsException"></exception>"
    /// /// <exception cref="TableAlreadyExistsWithRoleException"></exception>"
    /// /// <exception cref="TableAlreadyExistsWithNameException"></exception>"
    TableDefinitionsModel CreateTable(string tableName, CollectionTypesEnum collectionType, ulong roleId);

    /// <summary>
    /// Deletes a column.  May either supply a column id, or else a column name + a table identifier (id, role id, table name).
    /// </summary>
    /// <param name="columnName">The name of the column. If null, then columnId must not be null.</param>
    /// <param name="columnId">Null or the id of the column.  If null, then columnName cannot be NULL and one of the table identifiers must also not be null.</param>
    /// <param name="tableId">Null or the id of the table this column should be placed in.  If null, then either roleId or tableName must not be null if columnId is null.</param>
    /// <param name="roleId">Null or the role id of the table this column should be placed in.  If null, then either tableId or tableName must not be null if columnId is null.</param>
    /// <param name="tableName">Null or the name of the table this column should be placed in.  If null, then either tableId or roleId must not be null if columnId is null.</param>
    /// <returns>A ColumnDefinitionModel entity returned by the db context after it removes the column.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be removed from does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If there is no column that matches the given id or name + tableid.</exception>    
    ColumnDefinitionsModel DeleteColumn(string? columnName = null, int? columnId = null, int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Deletes a given row from the table.
    /// </summary>
    /// <param name="tableId">NULL or the id of the table to delete the row in.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table to delete the row in.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table to delete the row in.  If null, then either roleId or tableId cannot be null.</param>
    /// <param name="rowNum">NULL or the row num of the row to delete the row in.  If null, rowKEY cannot be null.</param>
    /// <param name="rowKey">NULL or the key of the row to delete the row in.  If null, rowNum cannot be null.</param>
    /// <exception cref="InvalidKeyException"></exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>    
    void DeleteRow(int? tableId = null, ulong? roleId = null, string? tableName = null, int? rowNum = null, string? rowKey = null);

    /// <summary>
    /// Deletes a table.
    /// </summary>
    /// <param name="tableId">NULL or the id of the table to delete.  If null, then either role id or tablename cannot be.</param>
    /// <param name="roleId">NULL or the role id of the table to delete.  If null, then either table id or tablename cannot be.</param>
    /// <param name="tableName">NULL or the name of the table to delete.  If null, then either role id or table id cannot be.</param>
    /// <returns>A <class cref="TableDefinitionsModel">TableDefinitionsModel</class> of the deleted table.</returns>
    TableDefinitionsModel DeleteTable(int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Checks if a table with the given identifiers exists.
    /// </summary>
    /// <param name="tableId">NULL or the id of the table.  If null, then either role id or table name must not be null.</param>
    /// <param name="roleId">NULL or the role id of the table.  If null, then either table id or table name must not be null.</param>
    /// <param name="tableName">NULL or the name of thet able.  If nole then either table id or role id must not be null.</param>
    /// <returns>True if table exists, otherwise false.</returns>
    bool DoesTableExist(int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Retrieves the value of a cell from the given table, column, and row.
    /// </summary>
    /// <param name="tableId">NULL or the id of the table to get the cell value of.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table get the cell value of.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table get the cell value of.  If null, then either roleId or tableId cannot be null.</param>
    /// <param name="columnId">NULL or the column id of the cell to get the value of.  If null, then columnName cannot be null.</param>
    /// <param name="columnName">NULL or the column name of the cell to get the value of.  If null, then columnId cannot be null.</param>
    /// <param name="rowNum">NULL or the row num of the row to get the cell value of.  If null, rowKEY cannot be null.</param>
    /// <param name="rowKey">NULL or the key of the row to get the cell value of.  If null, rowNum cannot be null.</param>
    /// <returns>A string of the cell's value.  If the value is null, then it will return an empty string.</returns>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given column does not exist.</exception>
    /// <exception cref="CellDoesNotExistException">If the given cell does not exist.</exception>
    string GetCellValue(int? tableId = null, ulong? roleId = null, string? tableName = null, int? columnId = null, string? columnName = null, int? rowNum = null, string? rowKey = null);

    /// <summary>
    /// Retrieves a column of the given table..
    /// </summary>
    /// <param name="columnName">Null or the name of the column to retrieve. If null then columnId cannot be null.</param>
    /// <param name="columnId">Null or the id of the column to retrieve.  If null then columnName cannot be null</param>
    /// <param name="tableId">Null or the id of the table this column is in.  If null, then either roleId or tableName must not be null if columnId is null.</param>
    /// <param name="roleId">Null or the role id of the table this column is in.  If null, then either tableId or tableName must not be null if columnId is null.</param>
    /// <param name="tableName">Null or the name of the table this column is in.  If null, then either tableId or roleId must not be null if columnId is null.</param>
    /// <returns>The ColumnDefinitionModel of the column.</returns>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the column does not exist.</exception>"    
    public ColumnDefinitionsModel GetColumn(string? columnName = null, int? columnId = null, int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Retrieves all the columns of the given table..
    /// </summary>
    /// <param name="tableId">Null or the id of the table to get the columns from.  If null, then either roleId or tableName must not be null if columnId is null.</param>
    /// <param name="roleId">Null or the role id of the table to get the columns from.  If null, then either tableId or tableName must not be null if columnId is null.</param>
    /// <param name="tableName">Null or the name of the table to get the columns from.  If null, then either tableId or roleId must not be null if columnId is null.</param>
    /// <returns>A list of all the columns in the table.</returns>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    List<ColumnDefinitionsModel> GetColumns(int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Returns the DataType id with the given name, or null if none is found.
    /// </summary>
    /// <param name="dataTypeName">The name of the data type.</param>
    /// <returns>The DataType id with that name, or null if none is found.</returns>
    public int? GetDataTypeId(string dataTypeName);

    /// <summary>
    /// Retrieves an EinTable from the given table.
    /// </summary>
    /// <param name="tableId">The id of the table.</param>
    /// <param name="roleId">The role id of the table.</param>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>An EinTable from the given table.</returns>
    EinTable GetEinTable(int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Retrieves the embed part with the given Id which is attached to the given table.
    /// </summary>
    /// <param name="embedPartId">The id of the part.</param>
    /// <param name="tableId">Null or the table id the embed is attached to.  If null, then either role id or tablename cannot be.</param>
    /// <param name="roleId">Null or the role id the embed is attached to.  If null, then either table id or tablename cannot be.</param>
    /// <param name="tableName">Null or the name the embed is attached to.  If null, then either role id or table id cannot be.</param>
    /// <returns>The <see cref="EinEmbedPartsModel"/> or null if none exists which matches the criteria.</returns>    
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    EinEmbedPartsModel? GetEmbedPart(int embedPartId, int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Retrieves a list of the embed parts which are attached to the given table.
    /// </summary>
    /// <param name="embedPartId">The id of the part.</param>
    /// <param name="tableId">Null or the table id the embeds are attached to.  If null, then either role id or tablename cannot be.</param>
    /// <param name="roleId">Null or the role id the embeds are attached to.  If null, then either table id or tablename cannot be.</param>
    /// <param name="tableName">Null or the name the embeds are attached to.  If null, then either role id or table id cannot be.</param>
    /// <returns>A List of <see cref="EinEmbedPartsModel"/> or null if none exist.</returns>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    List<EinEmbedPartsModel>? GetEmbedParts(int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Retrieves the sequence # of the given part.
    /// </summary>
    /// <param name="embedPartId">The id of the part to get the sequence of.</param>
    /// <returns>The sequence # of the part.</returns>
    /// <exception cref="EinEmbedPartDoesNotExistException">If there's no part with the given id./exception>   
    int GetEmbedPartSequence(int embedPartId);

    /// <summary>
    /// Retrieves a table with the given tableId, roleId, or tableName.
    /// </summary>
    /// <param name="tableId">Null or the id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">Null or the role id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="tableName">Null or the name of the table.  If null, then either roleId or tableId cannot be null.</param>
    /// <returns>A TableDefinitionsModel of the table.</returns>
    /// <exception cref="TableDoesNotExistException">If all arguments are null, or if a table cannot be found with the given argument.</exception>
    TableDefinitionsModel GetTable(int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Checks the given data against the given data type.
    /// </summary>
    /// <param name="dataType">The data type to check against.</param>
    /// <param name="data">The data to check against the data type.</param>
    /// <returns>True if matches, false otherwise.</returns>
    bool IsValidValue(DataTypesEnum dataType, string data);

    /// <summary>
    /// Modifies the value in a cell.  If the cell data type is a string, it will replace it.  If numeric, it will add or subtract.
    /// </summary>
    /// <param name="modifier">Value to modify the cell data by.</param>
    /// <param name="tableId">NULL or the id of the table to modify the cell value of.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table modify the cell value of.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table modify the cell value of.  If null, then either roleId or tableId cannot be null.</param>
    /// <param name="columnId">NULL or the column id of the cell to modify the value of.  If null, then columnName cannot be null.</param>
    /// <param name="columnName">NULL or the column name of the modify to get the value of.  If null, then columnId cannot be null.</param>
    /// <param name="rowNum">NULL or the row num of the row to get the modify value of.  If null, rowKEY cannot be null.</param>
    /// <param name="rowKey">NULL or the key of the row to get the modify value of.  If null, rowNum cannot be null.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">If the given data type does not match the cell's data type (ie, text when the cell is a numeric).</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given column does not exist.</exception>
    /// <exception cref="InvalidKeyException">If rowNum and rowKey are null</exception>"
    /// <exception cref="CellDoesNotExistException">If there are no cells with the given rowNum or rowKey.</exception>"
    bool ModifyCellValue(string modifier, int? tableId = null, ulong? roleId = null, string? tableName = null, int? columnId = null, string? columnName = null, int? rowNum = null, string? rowKey = null);

    /// <summary>
    /// Removes all the embed parts attached to the given table.
    /// </summary>
    /// <param name="tableId">Null or the table id the embeds are attached to.  If null, then either role id or tablename cannot be.</param>
    /// <param name="roleId">Null or the role id the embeds are attached to.  If null, then either table id or tablename cannot be.</param>
    /// <param name="tableName">Null or the name the embeds are attached to.  If null, then either role id or table id cannot be.</param>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    void RemoveAllEmbedParts(int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Removes the embed part with the given id.
    /// </summary>
    /// <param name="embedPartId">The id of the part to remove.</param>
    /// <exception cref="EinEmbedPartDoesNotExistException">If no embed exists with the given id.</exception>   
    void RemoveEmbedPart(int embedPartId);

    /// <summary>
    /// Renames a column.
    /// </summary>
    /// <param name="newColumnName">The new column name.  Cannot be null or empty, and will be stripped of non-alpha-numerics and non-dashes.</param>
    /// <param name="oldColumnName">NULL or the name of the column to rename.  If null, columnID may not be null.</param>
    /// <param name="columnId">NULL or the id of the column.  Cannot be null if columnName is null.  If given, then the table identifiers may all be null.</param>
    /// <param name="tableId">NULL or the id of the table the column is on.  Only necessary if columnID is null.</param>
    /// <param name="roleId">NULL or the role id of the table the column is on.  Only necessary if columnID is null.</param>
    /// <param name="tableName">NULL or the name of the table the column is on.  Only necessary if columnID is null.</param>
    /// <returns></returns>
    /// <exception cref="InvalidNameException">If the name, after stripping all unallowed chars, is null or empty.</exception>
    /// <exception cref="TableDoesNotExistException">If the table the column is on does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If there is no column that matches the given id or name + table(id/role id/name).</exception>
    string RenameColumn(string newColumnName, string? oldColumnName = null, int? columnId = null, int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Sets a table's name.  The name must be unique to this table.
    /// </summary>
    /// <param name="newTableName">The new name of the table.  Must be unique.</param>
    /// <param name="tableId">Null or the id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">Null or the role id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="tableName">Null or the name of the table.  If null, then either roleId or tableId cannot be null.</param>
    /// <returns>[string] The old name of the table.</returns>
    /// <exception cref="InvalidNameException">If the given name is invalid.</exception>
    /// <exception cref="TableAlreadyExistsWithNameException">If there is already a table with that name in existance.</exception>
    /// <exception cref="TableDoesNotExistException">If all arguments are null, or if a table cannot be found with the given argument.</exception>    
    string RenameTable(string newTableName, int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Sets the value in a cell.
    /// </summary>
    /// <param name="newValue">Value to set the cell data to.</param>
    /// <param name="tableId">NULL or the id of the table to set the cell value of.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table set the cell value of.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table set the cell value of.  If null, then either roleId or tableId cannot be null.</param>
    /// <param name="columnId">NULL or the column id of the cell to set the value of.  If null, then columnName cannot be null.</param>
    /// <param name="columnName">NULL or the column name of the set to get the value of.  If null, then columnId cannot be null.</param>
    /// <param name="rowNum">NULL or the row num of the row to get the set value of.  If null, rowKEY cannot be null.</param>
    /// <param name="rowKey">NULL or the key of the row to get the set value of.  If null, rowNum cannot be null.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">If the given data type does not match the cell's data type (ie, text when the cell is a numeric).</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given column does not exist.</exception>
    /// <exception cref="InvalidKeyException">If rowNum and rowKey are null</exception>"
    /// <exception cref="CellDoesNotExistException">If there are no cells with the given rowNum or rowKey.</exception>"
    string SetCellValue(string newValue, int? tableId = null, ulong? roleId = null, string? tableName = null, int? columnId = null, string? columnName = null, int? rowNum = null, string? rowKey = null);

    /// <summary>
    /// Sets the data in the given slot for the given part.
    /// </summary>
    /// <param name="embedPartId">The id to set the data for.</param>
    /// <param name="dataSeq">1, 2, or 3.  If > 3, then 3.  If < 1, then 1.</param>
    /// <param name="data">The new data to set.</param>
    /// <exception cref="EinEmbedPartDoesNotExistException">If there's no embed part with the given id.</exception>   
    void SetEmbedPartData(int embedPartId, int dataSeq, string data);

    /// <summary>
    /// Changes the sequence (order) # the part is applied to the display.
    /// </summary>
    /// <param name="embedPartId">The id of the part to change the sequence of.</param>
    /// <param name="newSequenceNum">The new sequence number.</param>
    /// <exception cref="EinEmbedPartDoesNotExistException">If the part does not exist.</exception>    
    void SetEmbedPartSequence(int embedPartId, int newSequenceNum);

    /// <summary>
    /// Sets a new role id for the table.  No other table can have this role id.
    /// </summary>
    /// <param name="newRoleId"></param>
    /// <param name="tableId">Null or the id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">Null or the role id of the table.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="tableName">Null or the name of the table.  If null, then either roleId or tableId cannot be null.</param>
    /// <exception cref="TableDoesNotExistException">If all arguments are null, or if a table cannot be found with the given argument.</exception>
    /// <exception cref="TableAlreadyExistsWithRoleException">If there's a table which already has this role id.</exception>
    void SetTableRole(ulong newRoleId, int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Updates multiple cells in a row based off the provided updateData dict.  Key:Value should be ColumnName:Data.
    /// </summary>
    /// <param name="updateData"></param>
    /// <param name="tableId">NULL or the id of the table to update the cells of.  If null, then either roleId or tableName cannot be null.</param>
    /// <param name="roleId">NULL or the role id of the table to update the cells of.  If null, then either tableId or tableName cannot be null.</param>
    /// <param name="tableName">NULL or the name of the table to update the cells.  If null, then either roleId or tableId cannot be null.</param>
    /// <param name="rowNum">NULL or the row num of the row to update the cells of.  If null, rowKEY cannot be null.</param>
    /// <param name="rowKey">NULL or the key of the row to update the cells of.  If null, rowNum cannot be null.</param>
    /// <exception cref="InvalidDataException">If the given data type does not match the cell's data type (ie, text when the cell is a numeric).</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given column does not exist.</exception>
    /// <exception cref="InvalidKeyException">If rowNum and rowKey are null</exception>"
    /// <exception cref="CellDoesNotExistException">If there are no cells with the given rowNum or rowKey.</exception>"
    /// <exception cref="KeyNotFoundException">If the given keys do not exist in any rows of the table.</exception>
    void UpdateRow(Dictionary<string, string> updateData, int? tableId = null, ulong? roleId = null, string? tableName = null, int? rowNum = null, string? rowKey = null);
}