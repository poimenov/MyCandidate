using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess
{
    public class DatabaseMigrator : IDatabaseMigrator
    {
        private readonly IDatabaseFactory _databaseFactory;
        private readonly ILogger<DatabaseMigrator>? _logger;
        private Database? _database;
        public DatabaseMigrator(IDatabaseFactory databaseFactory, ILogger<DatabaseMigrator> logger)
        {
            _databaseFactory = databaseFactory;
            _logger = logger;
        }

        public event EventHandler<DatabaseMigrateEventArgs>? DatabaseMigrate;

        public void MigrateDatabase()
        {
            if (_databaseFactory.GetDatabaseType() == DatabaseType.SQLite)
            {
                var dbFileName = _databaseFactory.GetConnectionString().Split('=')[1];
                var path = Path.Combine(AppSettings.AppDataPath, dbFileName);
                if (!File.Exists(path))
                {
                    var directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory) && directory != null)
                    {
                        Directory.CreateDirectory(directory);
                    }
                }
            }
            using (_database = _databaseFactory.CreateDbContext())
            {
                _database.Database.Migrate();
                var dictionaryCreator = new DictionaryCreator(_database);
                dictionaryCreator.Create();
                OnDatabaseCreate(new DatabaseMigrateEventArgs());
            }

            if (_logger != null)
            {
                _logger.LogInformation("Database created");
            }
        }

        protected virtual void OnDatabaseCreate(DatabaseMigrateEventArgs e)
        {
            var handler = DatabaseMigrate;
            if (handler != null && _database != null)
            {
                handler(this, e);
                using (var transaction = _database.Database.BeginTransaction())
                {
                    try
                    {
                        _database.Countries.AddRange(e.Countries);
                        _database.SaveChanges();

                        _database.SkillCategories.AddRange(e.SkillCategories);
                        _database.SaveChanges();

                        _database.Companies.AddRange(e.Companies);
                        _database.SaveChanges();

                        _database.Candidates.AddRange(e.Candidates);
                        _database.SaveChanges();

                        _database.Vacancies.AddRange(e.Vacancies);
                        _database.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        if (_logger != null)
                        {
                            _logger.LogError(ex.Message);
                            if (ex.InnerException != null)
                            {
                                _logger.LogError(ex.InnerException.Message);
                            }
                        }
                        transaction.Rollback();
                    }
                }
            }
        }
    }
}
