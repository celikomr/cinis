using Dapper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Cinis.MsSql;

public static partial class DapperExtensions
{
    private static string GetTableName<T>()
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        => typeof(T).GetCustomAttribute<TableAttribute>().Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
    private static string GetTableSchema<T>() => typeof(T).GetCustomAttribute<TableAttribute>().Schema == null
            ? ""
            : typeof(T).GetCustomAttribute<TableAttribute>().Schema + ".";
#pragma warning restore CS8602 // Dereference of a possibly null reference.

    private static IEnumerable<PropertyInfo> GetProperties<T>()
        => typeof(T).GetProperties();

    private static PropertyInfo? GetPrimaryKey<T>()
        => typeof(T).GetProperties()
                    .Where(x => x.GetCustomAttributes().Any(y => y.GetType() == typeof(KeyAttribute)))
                    .FirstOrDefault();

    private static IEnumerable<string?> GetColumns<T>()
        => typeof(T).GetProperties()
                    .Where(e => e.Name != GetPrimaryKey<T>()?.Name && e.GetCustomAttribute<ColumnAttribute>() != null)
                    .Select(e => e.GetCustomAttribute<ColumnAttribute>()?.Name);

    private static IEnumerable<string> GetColumnPropertyNames<T>()
        => typeof(T).GetProperties()
                    .Where(e => e.Name != GetPrimaryKey<T>()?.Name && e.GetCustomAttribute<ColumnAttribute>() != null)
                    .Select(e => e.Name);

    public static dynamic Create<T>(this SqlConnection connection, T entity, SqlTransaction? transaction = null)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        var stringOfColumns = string.Join(", ", GetColumns<T>());
        var stringOfParameters = string.Join(", ", GetColumnPropertyNames<T>().Select(e => "@" + e));
        var sql = $"insert into {GetTableSchema<T>()}{GetTableName<T>()} ({stringOfColumns}) values ({stringOfParameters}); select SCOPE_IDENTITY();";

        var result = connection.ExecuteScalar(sql, entity, transaction);
        return result;
    }

    public static List<T> Read<T>(this SqlConnection connection, dynamic? id = null, string? whereClause = null, SqlTransaction? transaction = null)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        string sql;
        if (id != null)
        {
            sql = $"select * from {GetTableSchema<T>()}{GetTableName<T>()} where {GetPrimaryKey<T>()?.GetCustomAttribute<ColumnAttribute>()?.Name} = {id}";
        }
        else if (!string.IsNullOrEmpty(whereClause))
        {
            sql = $"select * from {GetTableSchema<T>()}{GetTableName<T>()} where {whereClause}";
        }
        else
        {
            sql = $"select * from {GetTableSchema<T>()}{GetTableName<T>()}";
        }

        List<T> result = connection.Query<T>(sql, transaction: transaction).ToList();
        return result;
    }

    public static dynamic Update<T>(this SqlConnection connection, T entity, bool nullable = false, string? whereClause = null, SqlTransaction? transaction = null)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        string stringOfSets;
        if (nullable)
        {
            stringOfSets = string.Join(", ", GetProperties<T>().Where(e => e.GetCustomAttribute<ColumnAttribute>() != null && e.GetCustomAttribute<ColumnAttribute>()?.Name != GetPrimaryKey<T>()?.Name).Select(e => $"{e?.GetCustomAttribute<ColumnAttribute>()?.Name} = @{e?.Name}"));
        }
        else
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            string[] propertyNames = entity.GetType().GetProperties()
                                           .Where(x => x?.GetCustomAttribute<ColumnAttribute>() != null && x?.GetCustomAttribute<ColumnAttribute>()?.Name != GetPrimaryKey<T>()?.Name && x.GetValue(entity) != null)
                                           .Select(x => x?.GetCustomAttribute<ColumnAttribute>().Name).ToArray();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            stringOfSets = string.Join(" , ", propertyNames.Select(propertyName => propertyName + " = @" + entity.GetType().GetProperties().Where(x => x?.GetCustomAttribute<ColumnAttribute>() != null && x?.GetCustomAttribute<ColumnAttribute>()?.Name == propertyName).Select(e => e?.Name).FirstOrDefault()));
        }

        string sql;
        if (!string.IsNullOrEmpty(whereClause) && !string.IsNullOrEmpty(stringOfSets))
        {
            sql = $"update {GetTableSchema<T>()}{GetTableName<T>()} set {stringOfSets} where {whereClause}";
        }
        else if (!string.IsNullOrEmpty(stringOfSets))
        {
            sql = $"update {GetTableSchema<T>()}{GetTableName<T>()} set {stringOfSets} where {GetPrimaryKey<T>()?.GetCustomAttribute<ColumnAttribute>()?.Name} = @{GetPrimaryKey<T>()?.Name}";
        }
        else
        {
            return 0;
        }

        var result = connection.Execute(sql, entity, transaction);
        return result;
    }

    public static dynamic Delete<T>(this SqlConnection connection, dynamic? id = null, string? whereClause = null, SqlTransaction? transaction = null)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        string sql;
        if (id != null)
        {
            sql = $"delete from {GetTableSchema<T>()}.{GetTableName<T>()} where {GetPrimaryKey<T>()?.GetCustomAttribute<ColumnAttribute>()?.Name} = '{id}'";
        }
        else if (!string.IsNullOrEmpty(whereClause))
        {
            sql = $"delete from {GetTableSchema<T>()}{GetTableName<T>()} where {whereClause}";
        }
        else
        {
            sql = $"delete from {GetTableSchema<T>()}{GetTableName<T>()}";
        }

        var result = connection.Execute(sql, transaction: transaction);
        return result;
    }
}
