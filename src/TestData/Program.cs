// See https://aka.ms/new-console-template for more information
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.DataAccess;
using TestData;

AppDomain.CurrentDomain.SetData("DataDirectory", AppSettings.AppDataPath);

var dbCreator = new DatabaseCreator(null);
dbCreator.DatabaseCreate += OnDatabaseCreate;
dbCreator.CreateDatabase();

void OnDatabaseCreate(object? sender, DatabaseCreateEventArgs e)
{
    var countryCount = 5;
    var cityCount = 5;
    var categories = GetCategories();
    var countries = FakeData.GetCountries(countryCount, cityCount);
    var cities = countries.SelectMany(country => country.Cities, (country, city) => new { CountryName = country.Name, CityName = city.Name }).ToList();
    var companies = FakeData.GetCompanies(5, 3, countryCount * cityCount);
    foreach (var company in companies)
    {
        foreach (var office in company.Officies)
        {
            var city = cities.ElementAt(office.Location.CityId - 1);
            office.Name = $"{city.CountryName}, {city.CityName} Office";
        }
    }
    
    e.Countries.AddRange(countries);
    e.Companies.AddRange(companies);
    e.SkillCategories.AddRange(categories);
    e.Candidates.AddRange(FakeData.GetCandidates(1000, countryCount * cityCount, categories.Count));
}

List<SkillCategory> GetCategories()
{
    var retVal = new List<SkillCategory>();
    retVal.Add(new SkillCategory
    {
        Name = "Programming languages",
        Enabled = true,
        Skills = new List<Skill>
        {
            new Skill { Name = "C#", Enabled = true},
            new Skill { Name = "Python", Enabled = true},
            new Skill { Name = "Java", Enabled = true},
            new Skill { Name = "JavaScript", Enabled = true }
        }
    });
    retVal.Add(new SkillCategory
    {
        Name = "Frameworks",
        Enabled = true,
        Skills = new List<Skill>
        {
            new Skill { Name = ".NET", Enabled = true },
            new Skill { Name = ".NET WPF", Enabled = true },
            new Skill { Name = "ASP.NET MVC", Enabled = true }
        }
    });
    retVal.Add(new SkillCategory
    {
        Name = "Web Frameworks",
        Enabled = true,
        Skills = new List<Skill>
        {
            new Skill { Name = "Angular", Enabled = true },
            new Skill { Name = "React", Enabled = true },
            new Skill { Name = "Vue.js", Enabled = true }
        }
    });
    retVal.Add(new SkillCategory
    {
        Name = "Languages spoken",
        Enabled = true,
        Skills = new List<Skill>
        {
            new Skill { Name = "English", Enabled = true },
            new Skill { Name = "Spanish", Enabled = true }
        }
    });
    retVal.Add(new SkillCategory
    {
        Name = "Databases",
        Enabled = true,
        Skills = new List<Skill>
        {
            new Skill { Name = "MS Sql Server", Enabled = true },
            new Skill { Name = "Oracle", Enabled = true },
            new Skill { Name = "Postgresql", Enabled = true },
            new Skill { Name = "MySql", Enabled = true }
        }
    });
    return retVal;
}