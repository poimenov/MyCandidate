using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.DataAccess.Migrations;

namespace MyCandidate.DataAccess;

public class DatabaseCreator : IDatabaseCreator
{
    private readonly ILogger<DatabaseCreator> _logger;
    public DatabaseCreator(ILogger<DatabaseCreator> logger)
    {
        _logger = logger;
    }
    public void CreateDatabase()
    {
        var path = Path.Combine(AppSettings.AppDataPath, Database.DB_FILE_NAME);
        if (!File.Exists(path))
        {
            using (var db = new Database())
            {
                db.Database.Migrate();
                var dictionaryCreator = new DictionaryCreator(db);
                dictionaryCreator.Create();
            }

            _logger.LogInformation("Database created");
        }
    }
}
