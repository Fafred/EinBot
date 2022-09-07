namespace DataAccess.DbAccess;

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;

public class SqlDataAccess : ISqlDataAccess
{
	private readonly IConfiguration _configuration;

	public SqlDataAccess(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public async Task<IEnumerable<T>> LoadData<T, U>(
		string storedProcedure,
		U parameters,
		string connectionId = "Default")
	{

		Console.WriteLine(_configuration.GetConnectionString(connectionId));
		using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));

		/*SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();

		csb.DataSource = "EinBotDB.mssql.somee.com";
		csb.InitialCatalog = "EinBotDB";
		csb.UserID = "EinBot";
		csb.Password = "!.E1nb0t";
		csb.WorkstationID = "EinBotDB.mssql.somee.com";
		csb.PacketSize = 4096;
		csb.PersistSecurityInfo = false;
		Console.WriteLine(csb.ToString());
        Console.WriteLine(csb.ConnectionString);
        using IDbConnection connection = new SqlConnection(csb.ToString());*/

        return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
	}

	public async Task SaveData<T>(
		string storedProcedure,
		T parameters,
		string connectionId = "Default")
	{
		using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));

		await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
	}
}
