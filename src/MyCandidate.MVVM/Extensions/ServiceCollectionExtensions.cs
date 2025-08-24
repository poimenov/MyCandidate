using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.DataAccess;
using MyCandidate.MVVM.Services;
using MyCandidate.MVVM.ViewModels;
using MyCandidate.MVVM.ViewModels.Dictionary;
using MyCandidate.MVVM.Views;

namespace MyCandidate.MVVM.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<MainWindow>(service => new MainWindow
            {
                DataContext = service.GetRequiredService<MainWindowViewModel>()
            })
            //data access 
            .AddDatabaseServices(configuration)
            .AddTransient<IDataAccess<Country>, Countries>()
            .AddTransient<IDataAccess<City>, Cities>()
            .AddTransient<IDataAccess<Skill>, Skills>()
            .AddTransient<IDataAccess<SkillCategory>, SkillCategories>()
            .AddTransient<IDataAccess<Company>, Companies>()
            .AddTransient<IDataAccess<Office>, Officies>()
            .AddTransient<ICandidates, Candidates>()
            .AddTransient<IDictionariesDataAccess, Dictionaries>()
            .AddTransient<IVacancies, Vacancies>()
            .AddTransient<ICandidateSkills, CandidateSkills>()
            .AddTransient<IVacancySkills, VacancySkills>()
            .AddTransient<ICandidateOnVacancies, CandidateOnVacancies>()
            .AddTransient<IComments, Comments>()
            //services
            .AddTransient<IDictionaryService<Country>, CountryService>()
            .AddTransient<IDictionaryService<City>, CityService>()
            .AddTransient<IDictionaryService<Skill>, SkillService>()
            .AddTransient<IDictionaryService<SkillCategory>, SkillCategoryService>()
            .AddTransient<IDictionaryService<Company>, CompanyService>()
            .AddTransient<IDictionaryService<Office>, OfficeService>()
            .AddTransient<ICandidateService, CandidateService>()
            .AddTransient<IVacancyService, VacancyService>()
            .AddTransient<IAppServiceProvider, AppServiceProvider>()
            //ViewModels        
            .AddTransient<DictionaryViewModel<Country>, CountriesViewModel>()
            .AddTransient<DictionaryViewModel<City>, CitiesViewModel>()
            .AddTransient<DictionaryViewModel<Skill>, SkillsViewModel>()
            .AddTransient<DictionaryViewModel<SkillCategory>, CategoriesViewModel>()
            .AddTransient<DictionaryViewModel<Company>, CompaniesViewModel>()
            .AddTransient<DictionaryViewModel<Office>, OfficiesViewModel>();

        return services;
    }
}
