namespace EinBotDB.DataAccess;
using System.Linq;

public partial class EinDataAccess
{
    /// <summary>
    /// Returns the DataType id with the given name, or null if none is found.
    /// </summary>
    /// <param name="dataTypeName">The name of the data type.</param>
    /// <returns>The DataType id with that name, or null if none is found.</returns>
    public int? GetDataTypeId(string dataTypeName)
    {
        using var context = _factory.CreateDbContext();

        return context.DataTypes.FirstOrDefault(x =>
            x.Name.ToLower().Equals(dataTypeName.ToLower()))?.Id ?? null;
    }
}
