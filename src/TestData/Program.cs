// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.DataAccess;
using TestData;
using System.Reflection;
using log4net;

string DATA_DIRECTORY = "DATA_DIRECTORY";

AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
{
    var ex = (Exception)eventArgs.ExceptionObject;
    var log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);
    log.Error("Unhandled exception", ex);
    Console.WriteLine($"Unhandled exception: {ex.Message}");
    Environment.Exit(1);
};
AppDomain.CurrentDomain.SetData("DataDirectory", AppSettings.AppDataPath);
var dataDirectory = Environment.GetEnvironmentVariable(DATA_DIRECTORY);
if (dataDirectory == null)
{
    Environment.SetEnvironmentVariable(DATA_DIRECTORY, AppSettings.AppDataPath);
}

var configurationManager = new ConfigurationManager();
configurationManager
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile(AppSettings.JSON_FILE_NAME, optional: true, reloadOnChange: false)
    .AddEnvironmentVariables();
var settings = new HostApplicationBuilderSettings
{
    ApplicationName = AppSettings.APPLICATION_NAME,
    Configuration = configurationManager
};
var builder = Host.CreateApplicationBuilder(settings);
builder.Logging.ClearProviders().AddLog4Net();
builder.Services
    .Configure<AppSettings>(builder.Configuration)
    .AddSingleton<ILog>(LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType))
    .AddDatabaseServices(builder.Configuration);

var host = builder.Build();

var dbMigrator = host.Services.GetRequiredService<IDatabaseMigrator>();
dbMigrator.DatabaseMigrate += OnDatabaseMigrate;
Console.WriteLine("Start database migration with fake data...");
dbMigrator.MigrateDatabase();
Console.WriteLine("Database migration with fake data completed.");

void OnDatabaseMigrate(object? sender, DatabaseMigrateEventArgs e)
{
    var countryCount = 5;
    var cityCount = 5;
    var companyCount = 5;
    var officeCount = 3;
    var categories = GetCategories();
    var countries = FakeData.GetCountries(countryCount, cityCount);
    var officeNames = countries.SelectMany(country => country.Cities, (country, city) => $"{country.Name}, {city.Name} Office").ToList();
    var companies = FakeData.GetCompanies(companyCount, officeCount, countryCount * cityCount);
    foreach (var company in companies)
    {
        foreach (var office in company.Officies)
        {
            if (office.Location != null)
            {
                office.Name = officeNames.ElementAt(office.Location.CityId - 1);
            }
        }
    }
    var skillCount = categories.Sum(x => x.Skills.Count());
    e.Countries.AddRange(countries);
    e.Companies.AddRange(companies);
    e.SkillCategories.AddRange(categories);
    e.Candidates.AddRange(FakeData.GetCandidates(1000, countryCount * cityCount, skillCount));
    e.Vacancies.AddRange(FakeData.GetVacancies(50, companyCount * officeCount, skillCount));
}



List<SkillCategory> GetCategories()
{
    var source = new Dictionary<string, string[]>();
    source.Add("Programming languages", new string[] { "C#", "Python", "Java", "JavaScript" });
    source.Add("Frameworks", new string[] { ".NET", ".NET WPF", "ASP.NET MVC" });
    source.Add("Web Frameworks", new string[] { "Angular", "React", "Vue.js" });
    source.Add("Databases", new string[] { "MS Sql Server", "Oracle", "Postgresql", "MySql" });
    source.Add("Languages spoken", new string[] { "English", "Spanish" });

    var retVal = new List<SkillCategory>();
    foreach (var item in source)
    {
        var skills = new List<Skill>();
        foreach (var skill in item.Value)
        {
            skills.Add(new Skill
            {
                Name = skill,
                Enabled = true
            });
        }

        retVal.Add(new SkillCategory
        {
            Name = item.Key,
            Enabled = true,
            Skills = skills
        });
    }

    return retVal;
}