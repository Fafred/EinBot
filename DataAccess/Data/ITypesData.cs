namespace DataAccess.Data
{
    using DataAccess.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITypesData
    {
        Task<DataTypesModel?> GetDataType(int id);
        Task<IEnumerable<DataTypesModel>> GetDataTypes();
    }
}