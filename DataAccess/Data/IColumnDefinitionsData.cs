namespace DataAccess.Data
{
    using DataAccess.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IColumnDefinitionsData
    {
        Task DeleteColumnDefinition(int id);
        Task<IEnumerable<ColumnDefinitionsModel>> GeColumnDefinitions();
        Task<ColumnDefinitionsModel?> GetColumnDefinition(int id);
        Task InsertColumnDefinition(ColumnDefinitionsModel columnDefinition);
        Task UpdateColumnDefinition(ColumnDefinitionsModel columnDefinition);
    }
}