using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;

namespace MyCandidate.DataAccess;

public class DynamicDatabaseFactory : IDatabaseFactory
{
    private readonly DatabaseType _databaseType;
    private readonly string _connectionString;

    public DynamicDatabaseFactory(DatabaseType databaseType, string connectionString)
    {
        _databaseType = databaseType;
        _connectionString = connectionString;
    }

    public Database CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<Database>();

        switch (_databaseType)
        {
            case DatabaseType.SqlServer:
                optionsBuilder.UseSqlServer(_connectionString, x => x.MigrationsAssembly("MyCandidate.Migrations.SqlServer"));
                break;
            case DatabaseType.PostgreSQL:
                optionsBuilder.UseNpgsql(_connectionString, x => x.MigrationsAssembly("MyCandidate.Migrations.PostgreSql"));
                break;
            case DatabaseType.SQLite:
                optionsBuilder.UseSqlite(_connectionString, x => x.MigrationsAssembly("MyCandidate.Migrations.Sqlite"));
                break;
            default:
                throw new ArgumentException("Unsupported database type");
        }

        return new Database(optionsBuilder.Options);
    }

    public DatabaseType GetDatabaseType() => _databaseType;
    public string GetConnectionString() => _connectionString;
}
