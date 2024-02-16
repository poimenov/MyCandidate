using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.DataAccess.Migrations;

namespace MyCandidate.DataAccess
{
    public class DatabaseCreator : IDatabaseCreator
    {
        private readonly ILogger<DatabaseCreator> _logger;
        private Database _database;
        public DatabaseCreator(ILogger<DatabaseCreator> logger)
        {
            _logger = logger;
        }

        public event EventHandler<DatabaseCreateEventArgs> DatabaseCreate;

        public void CreateDatabase()
        {
            var path = Path.Combine(AppSettings.AppDataPath, Database.DB_FILE_NAME);
            if (!File.Exists(path))
            {
                using (_database = new Database())
                {
                    _database.Database.Migrate();
                    var dictionaryCreator = new DictionaryCreator(_database);
                    dictionaryCreator.Create();
                    OnDatabaseCreate(new DatabaseCreateEventArgs());
                }

                if (_logger != null)
                {
                    _logger.LogInformation("Database created");
                }
            }
        }

        protected virtual void OnDatabaseCreate(DatabaseCreateEventArgs e)
        {
            EventHandler<DatabaseCreateEventArgs> handler = DatabaseCreate;
            if (handler != null)
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
                    catch (System.Exception ex)
                    {
                        if (_logger != null)
                        {
                            _logger.LogError(ex.Message);
                            if(ex.InnerException != null)
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
