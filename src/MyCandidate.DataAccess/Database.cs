namespace MyCandidate.DataAccess;
using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;

internal class Database : DbContext
{
    public const string DB_FILE_NAME = "MyCandidate.db";

    public Database()
    {
        this.Database.Migrate();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DB_FILE_NAME}");
    }

    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<City> Cities { get; set; }
    public virtual DbSet<Location> Locations { get; set; }
    public virtual DbSet<ResourceType> ResourceTypes { get; set; }
    public virtual DbSet<SkillCategory> SkillCategories { get; set; }
    public virtual DbSet<Skill> Skills { get; set; }
    public virtual DbSet<Seniority> Seniorities { get; set; }
    public virtual DbSet<VacancyStatus> VacancyStatuses { get; set; }
    public virtual DbSet<Company> Companies { get; set; }
    public virtual DbSet<Office> Offices { get; set; }
    public virtual DbSet<SelectionStatus> SelectionStatuses { get; set; }
    public virtual DbSet<Candidate> Candidates { get; set; }
    public virtual DbSet<CandidateResource> CandidateResources { get; set; }
    public virtual DbSet<CandidateSkill> CandidateSkills { get; set; }
    public virtual DbSet<Vacancy> Vacancies { get; set; }
    public virtual DbSet<VacancySkill> VacancySkills { get; set; }
    public virtual DbSet<CandidateOnVacancy> CandidateOnVacancies { get; set; }
    public virtual DbSet<Comment> Comments { get; set; }
}
