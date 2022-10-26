using Dapper;
using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace Cinis.PostgreSql;

public static partial class DapperExtensions
{
    public static async Task<dynamic> CreateAsync<T>(this NpgsqlConnection connection, T entity, NpgsqlTransaction? transaction = null)
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

    public static async Task<List<T>> ReadAsync<T>(this NpgsqlConnection connection, dynamic? id = null, string ? whereClause = null, NpgsqlTransaction? transaction = null)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        string sql;
        if (id != null)
        {
            sql = $"select * from {GetTableSchema<T>()}.{GetTableName<T>()} where {GetPrimaryKey<T>()?.GetCustomAttribute<ColumnAttribute>()?.Name} = @{GetPrimaryKey<T>()?.Name}";
        }
        else if (!string.IsNullOrEmpty(whereClause))
        {
            sql = $"select * from {GetTableSchema<T>()}.{GetTableName<T>()} where {whereClause}";
        }
        else
        {
            sql = $"select * from {GetTableSchema<T>()}.{GetTableName<T>()}";
        }

        var result = await connection.QueryAsync<T>(sql, null, transaction);
        return result.ToList();
    }

    public static async Task<dynamic> UpdateAsync<T>(this NpgsqlConnection connection, T entity, bool nullable = false, string? whereClause = null, NpgsqlTransaction? transaction = null)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        string stringOfSets;
        if (nullable)
        {
            stringOfSets = string.Join(", ", GetProperties<T>().Where(e => e.GetCustomAttribute<ColumnAttribute>() != null).Select(e => $"{e?.GetCustomAttribute<ColumnAttribute>()?.Name} = @{e?.Name}"));
        }
        else
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            string[] propertyNames = entity.GetType().GetProperties()
                                           .Where(x => x.GetCustomAttribute<ColumnAttribute>() != null && x.GetValue(entity) != null)
                                           .Select(x => x.GetCustomAttribute<ColumnAttribute>().Name).ToArray();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            stringOfSets = string.Join(" , ", propertyNames.Select(propertyName => propertyName + " = @" + entity.GetType().GetProperties().Where(x => x.GetCustomAttribute<ColumnAttribute>() != null && x?.GetCustomAttribute<ColumnAttribute>()?.Name == propertyName).Select(e => e.Name).FirstOrDefault()));
        }

        string sql;
        if (!string.IsNullOrEmpty(whereClause))
        {
            sql = $"update {GetTableSchema<T>()}.{GetTableName<T>()} set {stringOfSets} where {whereClause}";
        }
        else
        {
            sql = $"update {GetTableSchema<T>()}.{GetTableName<T>()} set {stringOfSets} where {GetPrimaryKey<T>()?.GetCustomAttribute<ColumnAttribute>()?.Name} = @{GetPrimaryKey<T>()?.Name}";
        }

        var result = await connection.ExecuteAsync(sql, entity, transaction);
        return result;
    }

    public static async Task<dynamic> DeleteAsync<T>(this NpgsqlConnection connection, string? whereClause = null, NpgsqlTransaction? transaction = null)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        string sql;
        if (string.IsNullOrEmpty(whereClause))
        {
            sql = $"delete from {GetTableSchema<T>()}.{GetTableName<T>()}";
        }
        else
        {
            sql = $"delete from {GetTableSchema<T>()}.{GetTableName<T>()} where {whereClause}";
        }

        var result = await connection.ExecuteAsync(sql, null, transaction);
        return result;
    }
}
