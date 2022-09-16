namespace EinBotDB.DataAccess;

using EinBotDB.Models;
using System.Collections.Generic;

public interface IEinDataAccess
{
    public int AddEmbedPart(EmbedPartsEnum embedPart,
        string Data01, string? Data02 = null, string? Data03 = null,
        int? tableId = null, ulong? roleId = null, string? tableName = null,
        int? sequence = null);

    /// <summary>
    /// Add a row to a table.
    /// </summary>
    /// <param name="tableId">The id of the table to add a row to.</param>
    /// <param name="key">The key of the role.</param>
    /// <param name="columnsDataDict">A dictionary with Key:Value of ColumnName:Data</param>
    /// <returns>The row number of the newly insterted row.</returns>
    /// <exception cref="KeyAlreadyPresentInTableException">If the given key is already present in the table.</exception>
    /// <exception cref="InvalidDataException">If one of the given data types doesn't match the data type defined by the column.</exception>
    /// <exception cref="TableDoesNotExistException">If a table with the given tableId does not exist.</exception>"
    int AddRow(int tableId, string key, Dictionary<string, string>? columnsDataDict = null);

    /// <summary>
    /// Changes the key of a row.
    /// </summary>
    /// <param name="tableId">The id of the table the row is in.</param>
    /// <param name="rowNum">The row number of the row.</param>
    /// <param name="key">The new key to assign.</param>
    /// <returns>True/False of success.</returns>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    bool ChangeKey(int tableId, int rowNum, string key);

    /// <summary>
    /// Create a new column for the given table.
    /// </summary>
    /// <param name="tableId">The id of the table this column should be placed in.</param>
    /// <param name="columnName">The name of the column.  Everything other than alpha-numerics and '-' chars are stripped out.</param>
    /// <param name="dataType">The type of data stored in this column</param>
    /// <returns>A ColumnDefinitionModel entity returned by the db context.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be added to does not exist.</exception>
    /// <exception cref="InvalidNameException">If the name of the column is invalid.</exception>
    /// <exception cref="ColumnAlreadyExistsException">If there is already a column with that name in the given table.</exception>
    ColumnDefinitionsModel? CreateColumn(int tableId, string columnName, DataTypesEnum dataType);

    /// <summary>
    /// Create a new column for the given table.
    /// </summary>
    /// <param name="tableName">The name of the table this column should be placed in.</param>
    /// <param name="columnName">The name of the column.  Everything other than alpha-numerics and '-' chars are stripped out.</param>
    /// <param name="dataType">The type of data stored in this column</param>
    /// <returns>A ColumnDefinitionModel entity returned by the db context.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be added to does not exist.</exception>
    /// <exception cref="InvalidNameException">If the name of the column is invalid.</exception>
    /// <exception cref="ColumnAlreadyExistsException">If there is already a column with that name in the given table.</exception>
    ColumnDefinitionsModel? CreateColumn(string tableName, string columnName, DataTypesEnum dataType);

    /// <summary>
    /// Create a new column for the given table.
    /// </summary>
    /// <param name="roleId">The role id of the table this column should be placed in.</param>
    /// <param name="columnName">The name of the column.  Everything other than alpha-numerics and '-' chars are stripped out.</param>
    /// <param name="dataType">The type of data stored in this column</param>
    /// <returns>A ColumnDefinitionModel entity returned by the db context.</returns>
    /// <exception cref="TableDoesNotExistException">If the table the column was to be added to does not exist.</exception>
    /// <exception cref="InvalidNameException">If the name of the column is invalid.</exception>
    /// <exception cref="ColumnAlreadyExistsException">If there is already a column with that name in the given table.</exception>
    ColumnDefinitionsModel? CreateColumn(ulong roleId, string columnName, DataTypesEnum dataType);

    /// <summary>
    /// Creates a new table.
    /// </summary>
    /// <param name="tableName">The name of the new table.  Must contain only alpha-numeric and '-' characters. </param>
    /// <param name="collectionType">The type of collection this table is.</param>
    /// <param name="roleId">The role id associated with this table.</param>
    /// <returns>The TableDefinitionsModel entity of the created table.</returns>
    /// <exception cref="InvalidNameException">If the table name is invalid.</exception>
    /// <exception cref="TableAlreadyExistsException">If a table already exists with the given name or role.</exception>"
    TableDefinitionsModel? CreateTable(string tableName, CollectionTypesEnum collectionType, ulong roleId);

    /// <summary>
    /// Attempts to delete a column with the given id.
    /// </summary>
    /// <param name="columnId">The id of the column to delete</param>
    /// <returns>A ColumnDefinitionsModel entity of the deleted column.</returns>
    /// <exception cref="ColumnDoesNotExistException">If there is no column with the given id.</exception>
    ColumnDefinitionsModel DeleteColumn(int columnId);

    /// <summary>
    /// Attempts to delete a column with the given id.
    /// </summary>
    /// <param name="tableId">The id of the table the column is in.</param>
    /// <param name="columnName">The name of the column to delete.</param>
    /// <returns>A ColumnDefinitionsModel entity of the deleted column.</returns>
    /// <exception cref="ColumnDoesNotExistException">If there is no column with the given id.</exception>
    ColumnDefinitionsModel DeleteColumn(int tableId, string columnName);

    /// <summary>
    /// Attempts to delete a column with the given name from the table with the given name.
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="columnName">The name of the column to delete.</param>
    /// <returns>A ColumnDefinitionsModel entity of the deleted column.</returns>
    /// <exception cref="ColumnDoesNotExistException">If there is no column with the given column id.</exception>
    /// <exception cref="TableDoesNotExistException">If there is no table with the given table name.</exception>
    ColumnDefinitionsModel DeleteColumn(string tableName, string columnName);

    /// <summary>
    /// Attempts to delete a column with the given name from the table with the given name.
    /// </summary>
    /// <param name="tableRoleId">The role id of the table the column is on</param>
    /// <param name="columnName">The name of the column to delete.</param>
    /// <returns>A ColumnDefinitionsModel entity of the deleted column.</returns>
    /// <exception cref="ColumnDoesNotExistException">If there is no column with the given role id.</exception>
    /// <exception cref="TableDoesNotExistException">If there is no table with the given role id.</exception>
    ColumnDefinitionsModel DeleteColumn(ulong tableRoleId, string columnName);

    /// <summary>
    /// Removes a row from a table.
    /// </summary>
    /// <param name="tableId">The table to remove the row from.</param>
    /// <param name="rowNum">The number of the row to remove (if null, must provide a key).</param>
    /// <param name="key">The key of the row to remove (if null, must provide a rowNum).</param>
    /// <returns>True/False of success.</returns>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    bool DeleteRow(int tableId, int? rowNum = null, string? key = null);

    /// <summary>
    /// Removes the table with the given table id.
    /// </summary>
    /// <param name="tableId">The id of the table to delete.</param>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given table id.</exception>
    TableDefinitionsModel DeleteTable(int tableId);

    /// <summary>
    /// Removes the table with the given table name.
    /// </summary>
    /// <param name="tableName">The name of the table to delete.</param>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given name.</exception>
    TableDefinitionsModel DeleteTable(string tableName);

    /// <summary>
    /// Removes the table with the given role id.
    /// </summary>
    /// <param name="roleId">The role id of the table to delete.</param>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given role id.</exception>
    TableDefinitionsModel DeleteTable(ulong roleId);

    /// <summary>
    /// Retrieves the data of the cell in the given column of the given table.
    /// </summary>
    /// <param name="tableName">The name of the table the cell is in.</param>
    /// <param name="columnName"></param>
    /// <param name="dataType">OUT the data type associated with the cell.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <returns>The data in the given cell.</returns>
    /// <exception cref="InvalidDataException">If unable to determine the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public string? GetCellValue(string tableName, string columnName, out DataTypesEnum? dataType, int? rowNum = null, string? rowKey = null);

    /// <summary>
    /// Retrieves the data of the cell in the given column of the given table.
    /// </summary>
    /// <param name="roleId">The id of the role associated with the table the cell is in.</param>
    /// <param name="columnName"></param>
    /// <param name="dataType">OUT the data type associated with the cell.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <returns>The data in the given cell.</returns>
    /// <exception cref="InvalidDataException">If unable to determine the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public string? GetCellValue(ulong roleId, string columnName, out DataTypesEnum? dataType, int? rowNum = null, string? rowKey = null);

    /// <summary>
    /// Retrieves the data of the cell in the given column of the given table.
    /// </summary>
    /// <param name="tableId">The id of the table the cell is in.</param>
    /// <param name="columnName"></param>
    /// <param name="dataType">OUT the data type associated with the cell.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <returns>The data in the given cell.</returns>
    /// <exception cref="InvalidDataException">If unable to determine the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public string? GetCellValue(int tableId, string columnName, out DataTypesEnum? dataType, int? rowNum = null, string? rowKey = null);

    /// <summary>
    /// Retrieves the columns of a table.
    /// </summary>
    /// <param name="tableId">id of table the columns are in.</param>
    /// <returns>List of ColumnDefinitionsModel.</returns>
    /// <exception cref="TableDoesNotExistException">If no table exists with the given id.</exception>
    List<ColumnDefinitionsModel> GetColumns(int tableId);

    /// <summary>
    /// Retrieves the columns of a table.
    /// </summary>
    /// <param name="tableName">name of table the columns are in.</param>
    /// <returns>List of ColumnDefinitionsModel.</returns>
    /// <exception cref="TableDoesNotExistException">If no table exists with the given name.</exception>
    List<ColumnDefinitionsModel> GetColumns(string tableName);

    /// <summary>
    /// Retrieves the columns of a table.
    /// </summary>
    /// <param name="roleId">role id of table the columns are in.</param>
    /// <returns>List of ColumnDefinitionsModel.</returns>
    /// <exception cref="TableDoesNotExistException">If no table exists with the given role id.</exception>
    List<ColumnDefinitionsModel> GetColumns(ulong roleId);

    /// <summary>
    /// Returns the DataType id with the given name, or null if none is found.
    /// </summary>
    /// <param name="dataTypeName">The name of the data type.</param>
    /// <returns>The DataType id with that name, or null if none is found.</returns>
    int? GetDataTypeId(string dataTypeName);

    /// <summary>
    /// Creates and returns an EinTable object from the given table name.
    /// </summary>
    /// <param name="tableId">The id of the table.</param>
    /// <returns>An EinTable of the given table.</returns>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given table id.</exception>
    EinTable GetEinTable(int tableId);

    /// <summary>
    /// Creates and returns an EinTable object from the given table name.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <returns>An EinTable of the given table.</returns>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given table name.</exception>
    EinTable GetEinTable(string tableName);

    /// <summary>
    /// Creates and returns an EinTable object from the given table name.
    /// </summary>
    /// <param name="roleId">Role id of the table.</param>
    /// <returns>An EinTable of the given table.</returns>
    /// <exception cref="TableDoesNotExistException">If there's no table with the given role id.</exception>
    EinTable GetEinTable(ulong roleId);

    public int GetEmbedPartSequence(int embedPartId);

    public EinEmbedPartsModel? GetEmbedPart(int embedPartId, int? tableId = null, ulong? roleId = null, string? tableName = null);

    public List<EinEmbedPartsModel>? GetEmbedParts(int? tableId = null, ulong? roleId = null, string? tableName = null);

    /// <summary>
    /// Retrieves a table with the given name.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <returns>The TableDefinitionsModel of the table.</returns>
    /// <exception cref="TableDoesNotExistException">If a table with the given table name does not exist.</exception>
    public TableDefinitionsModel GetTable(string tableName);

    /// <summary>
    /// Retrieves a table with the given role id.
    /// </summary>
    /// <param name="tableId">Table id of the table.</param>
    /// <returns>The TableDefinitionsModel of the table.</returns>
    /// <exception cref="TableDoesNotExistException">If a table with the given table id does not exist.</exception>
    public TableDefinitionsModel GetTable(int tableId);

    /// <summary>
    /// Retrieves a table with the given role id.
    /// </summary>
    /// <param name="roleId">Role id associated with the table.</param>
    /// <returns>The TableDefinitionsModel of the table.</returns>
    /// <exception cref="TableDoesNotExistException">If a table with the given role id does not exist.</exception>
    public TableDefinitionsModel GetTable(ulong roleId);


    /// <summary>
    /// Modifies the value of the cell.  If numeric type, it will add or subtract the given modifier.  If text, it will simply set the data to the modifier.
    /// </summary>
    /// <param name="roleId">The id of the role associated with the table the cell is in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="modifier">The modifier to apply to the data.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public bool ModifyCellValue(ulong roleId, string columnName, string modifier, int? rowNum = null, string? rowKey = null);

    /// <summary>
    /// Modifies the value of the cell.  If numeric type, it will add or subtract the given modifier.  If text, it will simply set the data to the modifier.
    /// </summary>
    /// <param name="tableId">The id of the table the cell is in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="modifier">The modifier to apply to the data.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public bool ModifyCellValue(int tableId, string columnName, string modifier, int? rowNum = null, string? rowKey = null);
    
    /// <summary>
    /// Modifies the value of the cell.  If numeric type, it will add or subtract the given modifier.  If text, it will simply set the data to the modifier.
    /// </summary>
    /// <param name="tableName">The name of the table the cell is in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="modifier">The modifier to apply to the data.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public bool ModifyCellValue(string tableName, string columnName, string modifier, int? rowNum = null, string? rowKey = null);

    /// <summary>
    /// Renames a column.
    /// </summary>
    /// <param name="columnId">The id of the column to rename.</param>
    /// <param name="newColumnName">The new name to set the column to.</param>
    /// <returns>A ColumnDefinitionsModel of the column which was renamed.</returns>
    /// <exception cref="ColumnDoesNotExistException">If a column with the given id doesn't exist.</exception>
    /// <exception cref="InvalidNameException">If the newColumnName is invalid (only alpha-numeric and '-' allowed.</exception>
    ColumnDefinitionsModel RenameColumn(int columnId, string newColumnName);

    /// <summary>
    /// Renames a column.
    /// </summary>
    /// <param name="tableId">The id associated with the table.</param>
    /// <param name="oldColumnName">The current name of the column to rename.</param>
    /// <param name="newColumnName">The new name to set the column to.  Only '-' and alpha-numeric allowed.</param>
    /// <returns>A ColumnDefinitionsModel of the column which was renamed.</returns>
    /// <exception cref="TableDoesNotExistException">If no table exists with the given id.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the column with the given oldColumnName doesn't exist.</exception>
    /// <exception cref="InvalidNameException">If the new column name is invalid.</exception>"
    ColumnDefinitionsModel RenameColumn(int tableId, string oldColumnName, string newColumnName);

    /// <summary>
    /// Renames a column.
    /// </summary>
    /// <param name="tableName">The name associated with the table.</param>
    /// <param name="oldColumnName">The current name of the column to rename.</param>
    /// <param name="newColumnName">The new name to set the column to.  Only '-' and alpha-numeric allowed.</param>
    /// <returns>A ColumnDefinitionsModel of the column which was renamed.</returns>
    /// <exception cref="TableDoesNotExistException">If no table exists with the given name.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the column with the given oldColumnName doesn't exist.</exception>
    /// <exception cref="InvalidNameException">If the new column name is invalid.</exception>"    
    ColumnDefinitionsModel RenameColumn(string tableName, string oldColumnName, string newColumnName);

    /// <summary>
    /// Renames a column.
    /// </summary>
    /// <param name="tableRoleId">The role id associated with the table.</param>
    /// <param name="oldColumnName">The current name of the column to rename.</param>
    /// <param name="newColumnName">The new name to set the column to.  Only '-' and alpha-numeric allowed.</param>
    /// <returns>A ColumnDefinitionsModel of the column which was renamed.</returns>
    /// <exception cref="TableDoesNotExistException">If not able exists with the given id.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the column with the given oldColumnName doesn't exist.</exception>
    /// <exception cref="InvalidNameException">If the new column name is invalid.</exception>"
    ColumnDefinitionsModel RenameColumn(ulong tableRoleId, string oldColumnName, string newColumnName);

    /// <summary>
    /// Rename a table.
    /// </summary>
    /// <param name="tableId">Table id of the table to rename</param>
    /// <param name="newTableName">Name to set the table to.</param>
    /// <returns>The old table name.</returns>
    /// <exception cref="InvalidNameException">Thrown if the the table name is invalid.</exception>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    string RenameTable(int tableId, string newTableName);

    /// <summary>
    /// Rename a table.
    /// </summary>
    /// <param name="tableName">Old name of the table</param>
    /// <param name="newTableName">Name to set the table to.</param>
    /// <returns>The old table name.</returns>
    /// <exception cref="InvalidNameException">Thrown if the the table name is invalid.</exception>\
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    string RenameTable(string tableName, string newTableName);

    /// <summary>
    /// Rename a table.
    /// </summary>
    /// <param name="roleId">Role id of the table to rename</param>
    /// <param name="newTableName">Name to set the table to.</param>
    /// <returns>The old table name.</returns>
    /// <exception cref="InvalidNameException">Thrown if the the table name is invalid.</exception>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    string RenameTable(ulong roleId, string newTableName);

    public void RemoveAllEmbedParts(int? tableId = null, ulong? roleId = null, string? tableName = null);

    public void RemoveEmbedPart(int embedPartId);

    public void SetEmbedPartData(int embedPartId, int dataSeq, string data);

    public void SetEmbedPartSequence(int embedPartId, int newSequenceNum);

    /// <summary>
    /// Changes the role associated with a table.
    /// </summary>
    /// <param name="oldRoleId">Role id of the table to re-role</param>
    /// <param name="newRoleId">New role of the table.</param>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    void SetTableRole(ulong oldRoleId, ulong newRoleId);

    /// <summary>
    /// Sets the value of the cell in the given table and column.
    /// </summary>
    /// <param name="tableName">The name of the table the cell is in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="data">The data to set the cell to.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public void SetCellValue(string tableName, string columnName, string data, int? rowNum = null, string? rowKey = null);
    
    /// <summary>
    /// Sets the value of the cell in the given table and column.
    /// </summary>
    /// <param name="roleId">The id of the role associated with the table the cell is in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="data">The data to set the cell to.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public void SetCellValue(ulong roleId, string columnName, string data, int? rowNum = null, string? rowKey = null);

    /// <summary>
    /// Sets the value of the cell in the given table and column.
    /// </summary>
    /// <param name="tableId">The id of the table the cell is in.</param>
    /// <param name="columnName">The name of the column the cell is in.</param>
    /// <param name="data">The data to set the cell to.</param>
    /// <param name="rowNum">Null or the row number of the cell.  Cannot be null if rowKey is null.</param>
    /// <param name="rowKey">Null or the key associated with the row of the cell.  Cannot be null of rowNum is null.</param>
    /// <exception cref="InvalidDataException">If the given modifier does not match the cell's data type.</exception>
    /// <exception cref="TableDoesNotExistException">If the given table does not exist.</exception>
    /// <exception cref="ColumnDoesNotExistException">If the given table has no column with the given name.</exception>
    /// <exception cref="CellDoesNotExistException">If there's no cells with the given table, column name, row num, or row key.</exception>
    public void SetCellValue(int tableId, string columnName, string data, int? rowNum = null, string? rowKey = null);

    /// <summary>
    /// Updates a row with the given data.
    /// </summary>
    /// <param name="tableId">Id of the table to update the row in.</param>
    /// <param name="columnsDataDict">A dictionary with Key:Value of ColumnName:Data</param>
    /// <param name="rowNum">The row number of the row to update (if null, must provide a key).</param>
    /// <param name="key">The key of the row to update (if null, must provide a rollNum).</param>
    /// <returns>True/False of success.</returns>
    /// <exception cref="TableDoesNotExistException">If the table does not exist.</exception>
    /// <exception cref="InvalidDataException">If the given data type does not match the defined data type for the column.</exception>
    bool UpdateRow(int tableId, Dictionary<string, string> columnsDataDict, int? rowNum = null, string? key = null);
}