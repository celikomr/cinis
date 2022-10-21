using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Transactions;

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

    public static dynamic Update<T>(this OracleConnection connection, T entity, bool nullable = false, string? whereClause = null, OracleTransaction? transaction = null)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        // Create stringOfSets with entity's not null attribute(s) if nullable is false
        string stringOfSets;

        // Create stringOfSets with entity's all attribute(s) if nullable is true
        if (nullable)
        {
            stringOfSets = string.Join(", ", GetProperties<T>().Where(e => e.GetCustomAttribute<ColumnAttribute>() != null).Select(e => $"{e.GetCustomAttribute<ColumnAttribute>().Name} = :{e.Name}"));
        }
        else
        {
            string[] propertyNames = entity.GetType().GetProperties().Where(x => x.GetCustomAttribute<ColumnAttribute>() != null && x.GetValue(entity) != null).Select(x => x.GetCustomAttribute<ColumnAttribute>().Name).ToArray();
            stringOfSets = string.Join(" , ", propertyNames.Select(propertyName => propertyName + " = :" + entity.GetType().GetProperties().Where(x => x.GetCustomAttribute<ColumnAttribute>() != null && x.GetCustomAttribute<ColumnAttribute>().Name == propertyName).Select(e => e.Name).FirstOrDefault()));
        }

        string sql;
        if (!string.IsNullOrEmpty(whereClause))
        {
            sql = $"update {GetTableSchema<T>()}.{GetTableName<T>()} set {stringOfSets} where {whereClause}";
        }
        else
        {
            sql = $"update {GetTableSchema<T>()}.{GetTableName<T>()} set {stringOfSets} where {GetPrimaryKey<T>()?.GetCustomAttribute<ColumnAttribute>()?.Name} = :{GetPrimaryKey<T>()?.Name}";
        }

        var result = connection.Execute(sql, entity, transaction);
        return result;
    }
}
