using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace Cinis.MsSql;

public static partial class DapperExtensions
{
    private static string GetTableName<T>()
        => typeof(T).GetCustomAttribute<TableAttribute>().Name;

    private static string GetTableSchema<T>()
        => typeof(T).GetCustomAttribute<TableAttribute>().Schema;

    private static IEnumerable<PropertyInfo> GetProperties<T>()
        => typeof(T).GetProperties();

    private static PropertyInfo? GetPrimaryKey<T>()
        => typeof(T).GetProperties().Where(x => x.GetCustomAttributes().Any(y => y.GetType() == typeof(KeyAttribute))).FirstOrDefault();

    private static IEnumerable<string?> GetColumns<T>()
        => typeof(T).GetProperties().Where(e => e.Name != GetPrimaryKey<T>()?.Name && e.GetCustomAttribute<ColumnAttribute>() != null).Select(e => e.GetCustomAttribute<ColumnAttribute>()?.Name);

    private static IEnumerable<string> GetColumnPropertyNames<T>()
        => typeof(T).GetProperties().Where(e => e.Name != GetPrimaryKey<T>()?.Name && e.GetCustomAttribute<ColumnAttribute>() != null).Select(e => e.Name);

    public static dynamic Create<T>(this SqlConnection connection, T entity, SqlTransaction? transaction = null, DbType dbType = DbType.Int32)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        var stringOfColumns = string.Join(", ", GetColumns<T>());
        var stringOfParameters = string.Join(", ", GetColumnPropertyNames<T>().Select(e => "@" + e));
        var sql = $"insert into {GetTableSchema<T>()}.{GetTableName<T>()} ({stringOfColumns}) values ({stringOfParameters}) returning {GetPrimaryKey<T>()?.Name}";

        var result = connection.Execute(sql, entity, transaction);
        return result;
    }

    public static List<T> Read<T>(this SqlConnection connection, string? whereClause = null, SqlTransaction? transaction = null)
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
