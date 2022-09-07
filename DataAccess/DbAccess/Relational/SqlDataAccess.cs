namespace DataAccess.DbAccess;

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;

public class SqlDataAccess : IRelationalDataAccess
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
		using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));

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
