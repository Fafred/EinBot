namespace DataAccess.Data
{
    using DataAccess.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICollectionTypesData
    {
        Task<CollectionTypesModel?> GetCollectionType(int id);
        Task<IEnumerable<CollectionTypesModel>> GetCollectionTypes();
    }
}