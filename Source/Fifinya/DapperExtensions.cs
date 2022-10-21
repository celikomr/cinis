using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Dapper;
using System.Data;

namespace Fifinya;

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

    public static dynamic Create<T>(this IDbConnection dbConnection, T entity, IDbTransaction? dbTransaction = null, DbType dbType = DbType.Int32)
    {
        var stringOfColumns = string.Join(", ", GetColumns<T>());
        var stringOfParameters = string.Join(", ", GetColumnPropertyNames<T>().Select(e => ":" + e));
        var sql = $"insert into {GetTableSchema<T>()}.{GetTableName<T>()} ({stringOfColumns}) values ({stringOfParameters}) returning {GetPrimaryKey<T>()?.Name} into :lastcid";

        DynamicParameters parameters = new(entity);
        parameters.Add(name: "lastcid", dbType: dbType, direction: ParameterDirection.Output);

        var result = dbConnection.Execute(sql, parameters, dbTransaction);
        return parameters.Get<dynamic>("lastcid");
    }
}
