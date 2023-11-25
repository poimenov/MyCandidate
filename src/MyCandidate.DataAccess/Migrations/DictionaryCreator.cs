using MyCandidate.Common;

namespace MyCandidate.DataAccess.Migrations;

internal class DictionaryCreator
{
    private readonly Database _database;
    public DictionaryCreator(Database database)
    {
        _database = database;
    }

    public void Create()
    {
        CreateCountries();
        CreateCities();
        CreateResourceTypes();
        CreateCompanies();
        CreateLocations();
        CreateOffices();
        CreateVacancyStatuses();
        CreateSelectionStatuses();
        CreateSkillCategories();
        CreateSkills();
        CreateSeniorities();
    }

    private void CreateCountries()
    {
        if (!_database.Countries.Any())
        {
            _database.Countries.AddRange(
                new Country { Name = "Ukraine", Enabled = true },
                new Country { Name = "Poland", Enabled = true },
                new Country { Name = "Hungary", Enabled = true },
                new Country { Name = "Bulgaria", Enabled = true },
                new Country { Name = "Romania", Enabled = true }
                );
            _database.SaveChanges();
        }
    }

    private void CreateCities()
    {
        if (!_database.Cities.Any())
        {
            var ukraine = _database.Countries.First(x => x.Name == "Ukraine");
            var poland = _database.Countries.First(x => x.Name == "Poland");
            _database.Cities.AddRange(
                new City { Name = "Kharkiv", Enabled = true, Country = ukraine },
                new City { Name = "Kyiv", Enabled = true, Country = ukraine },
                new City { Name = "Lviv", Enabled = true, Country = ukraine },
                new City { Name = "Odessa", Enabled = true, Country = ukraine },
                new City { Name = "Poltava", Enabled = true, Country = ukraine },
                new City { Name = "Warsava", Enabled = true, Country = poland },
                new City { Name = "Poznan", Enabled = true, Country = poland },
                new City { Name = "Wroclav", Enabled = true, Country = poland },
                new City { Name = "Krakow", Enabled = true, Country = poland },
                new City { Name = "Budapest", Enabled = true, Country = _database.Countries.First(x => x.Name == "Hungary") },
                new City { Name = "Sofia", Enabled = true, Country = _database.Countries.First(x => x.Name == "Bulgaria") },
                new City { Name = "Buharest", Enabled = true, Country = _database.Countries.First(x => x.Name == "Romania") }
                );
            _database.SaveChanges();
        }
    }

    private void CreateResourceTypes()
    {
        if (!_database.ResourceTypes.Any())
        {
            _database.ResourceTypes.AddRange(
                new ResourceType { Name = "Phone", Enabled = true },
                new ResourceType { Name = "Mobile", Enabled = true },
                new ResourceType { Name = "Email", Enabled = true },
                new ResourceType { Name = "Url", Enabled = true },
                new ResourceType { Name = "Skype", Enabled = true }
                );
            _database.SaveChanges();
        }
    }

    private void CreateCompanies()
    {
        if (!_database.Companies.Any())
        {
            _database.Companies.AddRange(
                new Company { Name = "EPAM Systems", Enabled = true },
                new Company { Name = "SoftServe", Enabled = true },
                new Company { Name = "GlobalLogic", Enabled = true },
                new Company { Name = "Luxoft", Enabled = true },
                new Company { Name = "Ciklum", Enabled = true }
                );
            _database.SaveChanges();
        }
    }

    private void CreateLocations()
    {
        if (!_database.Locations.Any())
        {
            var kharkiv = _database.Cities.First(x => x.Name == "Kharkiv");
            var kyiv = _database.Cities.First(x => x.Name == "Kyiv");
            var lviv = _database.Cities.First(x => x.Name == "Lviv");
            var odessa = _database.Cities.First(x => x.Name == "Odessa");
            var poltava = _database.Cities.First(x => x.Name == "Poltava");
            _database.Locations.AddRange(
                new Location { Address = "Epam Kharkiv", Enabled = true, City = kharkiv },
                new Location { Address = "SoftServe Kharkiv", Enabled = true, City = kharkiv },
                new Location { Address = "GlobalLogic Kharkiv", Enabled = true, City = kharkiv },
                new Location { Address = "Ciklum Kharkiv", Enabled = true, City = kharkiv },
                new Location { Address = "Epam Kyiv", Enabled = true, City = kyiv },
                new Location { Address = "SoftServe Kyiv", Enabled = true, City = kyiv },
                new Location { Address = "GlobalLogic Kyiv", Enabled = true, City = kyiv },
                new Location { Address = "Luxoft Kyiv", Enabled = true, City = kyiv },
                new Location { Address = "Ciklum Kyiv", Enabled = true, City = kyiv },
                new Location { Address = "Epam Lviv", Enabled = true, City = lviv },
                new Location { Address = "SoftServe Lviv", Enabled = true, City = lviv },
                new Location { Address = "GlobalLogic Lviv", Enabled = true, City = lviv },
                new Location { Address = "Ciklum Lviv", Enabled = true, City = lviv },
                new Location { Address = "Epam Odessa", Enabled = true, City = odessa },
                new Location { Address = "SoftServe Odessa", Enabled = true, City = odessa },
                new Location { Address = "Luxoft Odessa", Enabled = true, City = odessa },
                new Location { Address = "Ciklum Odessa", Enabled = true, City = odessa },
                new Location { Address = "Epam Poltava", Enabled = true, City = poltava },
                new Location { Address = "SoftServe Poltava", Enabled = true, City = poltava }
            );
            _database.SaveChanges();
        }
    }

    private void CreateOffices()
    {
        if (!_database.Offices.Any())
        {
            var epam = _database.Companies.First(x => x.Name == "EPAM Systems");
            var softServe = _database.Companies.First(x => x.Name == "SoftServe");
            var globalLogic = _database.Companies.First(x => x.Name == "GlobalLogic");
            var luxoft = _database.Companies.First(x => x.Name == "Luxoft");
            var ciklum = _database.Companies.First(x => x.Name == "Ciklum");
            _database.Offices.AddRange(
                new Office { Name = "Kharkiv", Enabled = true, Company = epam, Location = _database.Locations.First(x => x.Address == "Epam Kharkiv") },
                new Office { Name = "Kyiv", Enabled = true, Company = epam, Location = _database.Locations.First(x => x.Address == "Epam Kyiv") },
                new Office { Name = "Lviv", Enabled = true, Company = epam, Location = _database.Locations.First(x => x.Address == "Epam Lviv") },
                new Office { Name = "Odessa", Enabled = true, Company = epam, Location = _database.Locations.First(x => x.Address == "Epam Odessa") },
                new Office { Name = "Poltava", Enabled = true, Company = epam, Location = _database.Locations.First(x => x.Address == "Epam Poltava") },
                new Office { Name = "Kharkiv", Enabled = true, Company = softServe, Location = _database.Locations.First(x => x.Address == "SoftServe Kharkiv") },
                new Office { Name = "Kyiv", Enabled = true, Company = softServe, Location = _database.Locations.First(x => x.Address == "SoftServe Kyiv") },
                new Office { Name = "Lviv", Enabled = true, Company = softServe, Location = _database.Locations.First(x => x.Address == "SoftServe Lviv") },
                new Office { Name = "Odessa", Enabled = true, Company = softServe, Location = _database.Locations.First(x => x.Address == "SoftServe Odessa") },
                new Office { Name = "Poltava", Enabled = true, Company = softServe, Location = _database.Locations.First(x => x.Address == "SoftServe Poltava") },
                new Office { Name = "Kharkiv", Enabled = true, Company = globalLogic, Location = _database.Locations.First(x => x.Address == "GlobalLogic Kharkiv") },
                new Office { Name = "Kyiv", Enabled = true, Company = globalLogic, Location = _database.Locations.First(x => x.Address == "GlobalLogic Kyiv") },
                new Office { Name = "Lviv", Enabled = true, Company = globalLogic, Location = _database.Locations.First(x => x.Address == "GlobalLogic Lviv") },
                new Office { Name = "Kyiv", Enabled = true, Company = luxoft, Location = _database.Locations.First(x => x.Address == "Luxoft Kyiv") },
                new Office { Name = "Odessa", Enabled = true, Company = luxoft, Location = _database.Locations.First(x => x.Address == "Luxoft Odessa") },
                new Office { Name = "Kharkiv", Enabled = true, Company = ciklum, Location = _database.Locations.First(x => x.Address == "Ciklum Kharkiv") },
                new Office { Name = "Kyiv", Enabled = true, Company = ciklum, Location = _database.Locations.First(x => x.Address == "Ciklum Kyiv") },
                new Office { Name = "Lviv", Enabled = true, Company = ciklum, Location = _database.Locations.First(x => x.Address == "Ciklum Lviv") },
                new Office { Name = "Odessa", Enabled = true, Company = ciklum, Location = _database.Locations.First(x => x.Address == "Ciklum Odessa") }
                );
            _database.SaveChanges();
        }
    }

    private void CreateVacancyStatuses()
    {
        if (!_database.VacancyStatuses.Any())
        {
            _database.VacancyStatuses.AddRange(
                new VacancyStatus { Name = "New", Enabled = true },
                new VacancyStatus { Name = "InProgress", Enabled = true },
                new VacancyStatus { Name = "Closed", Enabled = true }
                );
            _database.SaveChanges();
        }
    }

    private void CreateSelectionStatuses()
    {
        if (!_database.SelectionStatuses.Any())
        {
            _database.SelectionStatuses.AddRange(
                new SelectionStatus { Name = "SetContact", Enabled = true },
                new SelectionStatus { Name = "PreScreen", Enabled = true },
                new SelectionStatus { Name = "Technical interview", Enabled = true },
                new SelectionStatus { Name = "Final inerview", Enabled = true },
                new SelectionStatus { Name = "Rejected", Enabled = true },
                new SelectionStatus { Name = "Accepted", Enabled = true }
                );
            _database.SaveChanges();
        }
    }

    private void CreateSkillCategories()
    {
        if (!_database.SkillCategories.Any())
        {
            _database.SkillCategories.AddRange(
                new SkillCategory { Name = "Programming languages", Enabled = true },
                new SkillCategory { Name = "Frameworks", Enabled = true },
                new SkillCategory { Name = "Web Frameworks", Enabled = true },
                new SkillCategory { Name = "Languages spoken", Enabled = true },
                new SkillCategory { Name = "Databases", Enabled = true }
            );
            _database.SaveChanges();
        }
    }

    private void CreateSkills()
    {
        if (!_database.Skills.Any())
        {
            _database.Skills.AddRange(
                new Skill { Name = "C#", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Programming languages") },
                new Skill { Name = "Python", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Programming languages") },
                new Skill { Name = "Java", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Programming languages") },
                new Skill { Name = "JavaScript", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Programming languages") },
                new Skill { Name = ".NET", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Frameworks") },
                new Skill { Name = ".NET WPF", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Frameworks") },
                new Skill { Name = "ASP.NET MVC", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Frameworks") },
                new Skill { Name = "Angular", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Web Frameworks") },
                new Skill { Name = "React", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Web Frameworks") },
                new Skill { Name = "Vue.js", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Web Frameworks") },
                new Skill { Name = "MS Sql Server", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Databases") },
                new Skill { Name = "Oracle", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Databases") },
                new Skill { Name = "Postgresql", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Databases") },
                new Skill { Name = "MySql", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Databases") },
                new Skill { Name = "English", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Languages spoken") },
                new Skill { Name = "Spanish", Enabled = true, SkillCategory = _database.SkillCategories.First(x => x.Name == "Languages spoken") }
                );
            _database.SaveChanges();
        }
    }

    private void CreateSeniorities()
    {
        if (!_database.Seniorities.Any())
        {
            _database.Seniorities.AddRange(
                new Seniority { Name = "Intern", Enabled = true },
                new Seniority { Name = "Junior", Enabled = true },
                new Seniority { Name = "Middle", Enabled = true },
                new Seniority { Name = "Senior", Enabled = true }
                );
            _database.SaveChanges();
        }
    }
}
