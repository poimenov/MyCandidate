using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        DatabaseType databaseType = DatabaseType.SQLite;
        string connectionString = $"Data Source={AppSettings.DB_FILE_NAME}";

        var databaseSettings = configuration.GetSection("DatabaseSettings");
        if (databaseSettings != null)
        {
            databaseType = databaseSettings.GetValue("DatabaseType", DatabaseType.SQLite);
            var connectionStrings = databaseSettings.GetSection("ConnectionStrings");
            if (connectionStrings != null)
            {
                var _connectionString = databaseType switch
                {
                    DatabaseType.SqlServer => connectionStrings["SqlServer"],
                    DatabaseType.PostgreSQL => connectionStrings["PostgreSQL"],
                    DatabaseType.SQLite => connectionStrings["SQLite"],
                    _ => throw new ArgumentException("Unsupported database type")
                };

                if (!string.IsNullOrEmpty(_connectionString))
                {
                    connectionString = _connectionString;
                }
                else if (databaseType != DatabaseType.SQLite)
                {
                    throw new ArgumentException($"Connection string for {databaseType} is not provided.");
                }
            }
        }

        services.AddSingleton<IDatabaseFactory>(provider =>
            new DynamicDatabaseFactory(databaseType, connectionString));

        services.AddScoped(provider =>
        {
            var factory = provider.GetRequiredService<IDatabaseFactory>();
            return factory.CreateDbContext();
        });

        services.AddTransient<IDatabaseMigrator, DatabaseMigrator>();

        return services;
    }
}
