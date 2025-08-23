using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MyCandidate.Common;

namespace MyCandidate.DataAccess;

public class DesignTimeDatabaseFactory : IDesignTimeDbContextFactory<Database>
{
    public Database CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<Database>();

        //dotnet ef migrations add InitialCreate --context Database --output-dir ../MyCandidate.Migrations.Sqlite
        optionsBuilder.UseSqlite($"Data Source={AppSettings.DB_FILE_NAME}");

        //dotnet ef migrations add InitialCreate --context Database --output-dir ../MyCandidate.Migrations.SqlServer
        //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MyCandidate;Trusted_Connection=True;");

        //dotnet ef migrations add InitialCreate --context Database --output-dir ../MyCandidate.Migrations.PostgreSql
        //optionsBuilder.UseNpgsql("Server=localhost; Database=MyCandidate;Username=postgres;Password=password;");

        return new Database(optionsBuilder.Options);
    }
}
