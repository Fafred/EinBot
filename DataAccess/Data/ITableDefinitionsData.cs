namespace DataAccess.Data
{
    using DataAccess.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITableDefinitionsData
    {
        Task DeleteTableDefinition(int id);
        Task<IEnumerable<TableDefinitionsModel>> GeTableDefinitions();
        Task<TableDefinitionsModel?> GetTableDefinition(int id);
        Task InsertTableDefinition(TableDefinitionsModel tableDefinition);
        Task UpdateTableDefinition(TableDefinitionsModel tableDefinition);
    }
}