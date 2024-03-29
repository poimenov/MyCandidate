namespace MyCandidate.Common.Interfaces
{
    public interface IDatabaseMigrator
    {
        void MigrateDatabase();
        event EventHandler<DatabaseMigrateEventArgs> DatabaseMigrate;
    }

    public class DatabaseMigrateEventArgs : EventArgs
    {
        public DatabaseMigrateEventArgs()
        {
            Countries = new List<Country>();
            SkillCategories = new List<SkillCategory>();
            Companies = new List<Company>();
            Candidates = new List<Candidate>();
            Vacancies = new List<Vacancy>();
        }

        public List<Country> Countries { get; set; }
        public List<SkillCategory> SkillCategories { get; set; }
        public List<Company> Companies { get; set; }
        public List<Candidate> Candidates { get; set; }
        public List<Vacancy> Vacancies { get; set; }
    }
}
