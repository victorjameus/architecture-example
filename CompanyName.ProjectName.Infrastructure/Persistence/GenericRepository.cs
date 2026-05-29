using CompanyName.ProjectName.Application.Common.Interfaces;

namespace CompanyName.ProjectName.Infrastructure.Persistence;

internal sealed class GenericRepository<T>(IDbConnection connection, IDbTransaction? transaction = null) : IGenericRepository<T> where T : class
{
    private readonly string _table = typeof(T).Name;

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await connection.QueryAsync<T>($"SELECT * FROM {_table}", transaction: transaction);
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await connection.QueryFirstOrDefaultAsync<T>($"SELECT * FROM {_table} WHERE Id = @Id", new { Id = id }, transaction: transaction);
    }

    public async Task<int> AddAsync(T entity)
    {
        var excludedProps = new[]
        {
            "Id",
            "CreatedAt",
            "UpdatedAt",
            "ConvertedAt"
        };
        
        var allowedTypes = new[]
        {
            typeof(string), typeof(bool), typeof(bool?),
            typeof(int), typeof(int?), typeof(decimal), typeof(decimal?),
            typeof(double), typeof(double?), typeof(float), typeof(float?),
            typeof(long), typeof(long?), typeof(DateTime), typeof(DateTime?)
        };

        var properties = typeof(T).GetProperties().Where(p => !excludedProps.Contains(p.Name) && allowedTypes.Contains(p.PropertyType));
        var columns = string.Join(", ", properties.Select(p => p.Name));
        var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

        var sql = $"INSERT INTO {_table} ({columns}) OUTPUT INSERTED.Id VALUES ({values})";

        return await connection.ExecuteScalarAsync<int>(sql, entity, transaction: transaction);
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        var properties = typeof(T).GetProperties().Where(p => p.Name != "Id");
        var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));

        var sql = $"UPDATE {_table} SET {setClause} WHERE Id = @Id";
        var rows = await connection.ExecuteAsync(sql, entity, transaction: transaction);

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var rows = await connection.ExecuteAsync($"DELETE FROM {_table} WHERE Id = @Id", new { Id = id }, transaction: transaction);

        return rows > 0;
    }
}