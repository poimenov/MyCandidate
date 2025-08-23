using MyCandidate.Common;

namespace MyCandidate.DataAccess;

public interface IDatabaseFactory
{
    Database CreateDbContext();
    DatabaseType GetDatabaseType();
    string GetConnectionString();
}
