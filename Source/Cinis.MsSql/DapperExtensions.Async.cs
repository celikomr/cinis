using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Cinis.MsSql;

public static partial class DapperExtensions
{
    public static async Task<dynamic> CreateAsync<T>(this SqlConnection connection, T entity, SqlTransaction? transaction = null, DbType dbType = DbType.Int32)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        var stringOfColumns = string.Join(", ", GetColumns<T>());
        var stringOfParameters = string.Join(", ", GetColumnPropertyNames<T>().Select(e => "@" + e));
        var sql = $"insert into {GetTableSchema<T>()}.{GetTableName<T>()} ({stringOfColumns}) values ({stringOfParameters}) returning {GetPrimaryKey<T>()?.Name}";

        var result = await connection.ExecuteAsync(sql, entity, transaction);
        return result;
    }
}
