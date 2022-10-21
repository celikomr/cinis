using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Fifinya.Oracle;

public static partial class DapperExtensions
{
    public static async Task<dynamic> CreateAsync<T>(this OracleConnection connection, T entity, OracleTransaction? transaction = null, DbType dbType = DbType.Int32)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        var stringOfColumns = string.Join(", ", GetColumns<T>());
        var stringOfParameters = string.Join(", ", GetColumnPropertyNames<T>().Select(e => ":" + e));
        var sql = $"insert into {GetTableSchema<T>()}.{GetTableName<T>()} ({stringOfColumns}) values ({stringOfParameters}) returning {GetPrimaryKey<T>()?.Name} into :lastcid";

        DynamicParameters parameters = new(entity);
        parameters.Add(name: "lastcid", dbType: dbType, direction: ParameterDirection.Output);

        var result = await connection.ExecuteAsync(sql, parameters, transaction);
        return parameters.Get<dynamic>("lastcid");
    }
}
