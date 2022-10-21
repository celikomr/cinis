using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace Fifinya.Oracle;

public static partial class DapperExtensions
{
    public static dynamic Create<T>(this OracleConnection connection, T entity, OracleTransaction? transaction = null, DbType dbType = DbType.Int32)
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

        var result = connection.Execute(sql, parameters, transaction);
        return parameters.Get<dynamic>("lastcid");
    }

    public static List<T> Read<T>(this OracleConnection connection, string? whereClause = null, OracleTransaction? transaction = null)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        string sql;
        if (!string.IsNullOrEmpty(whereClause))
        {
            sql = $"select * from {GetTableSchema<T>()}.{GetTableName<T>()} where {whereClause}";
        }
        else
        {
            sql = $"select * from {GetTableSchema<T>()}.{GetTableName<T>()}";
        }

        List<T> result = connection.Query<T>(sql, null, transaction).ToList();
        return result;
    }
}
