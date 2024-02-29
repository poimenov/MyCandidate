using Dock.Model.Controls;
using Dock.Model.Core;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.ViewModels.Candidates;
using MyCandidate.MVVM.ViewModels.Dictionary;
using MyCandidate.MVVM.ViewModels.Tools;
using MyCandidate.MVVM.ViewModels.Vacancies;

namespace MyCandidate.MVVM.Services;

public interface IAppServiceProvider
{
    ICandidateService CandidateService { get; }
    IVacancyService VacancyService { get; }
    IDictionaryService<Country> CountryService { get; }
    IDictionaryService<City> CityService { get; }
    IDictionaryService<Company> CompanyService { get; }
    IDictionaryService<Office> OfficeService { get; }
    IDictionaryService<SkillCategory> SkillCategoryService { get; }
    IDictionaryService<Skill> SkillService { get; }
    IDictionariesDataAccess DictionariesDataAccess { get; }
    DictionaryViewModel<Country> GetCountriesViewModel();
    DictionaryViewModel<City> GetCitiesViewModel();
    DictionaryViewModel<SkillCategory> GetCategoriesViewModel();
    DictionaryViewModel<Skill> GetSkillsViewModel();
    DictionaryViewModel<Company> GetCompaniesViewModel();
    DictionaryViewModel<Office> GetOfficiesViewModel();
    VacancyViewModel GetVacancyViewModel();
    void OpenVacancyViewModel(int vacancyId);
    VacancySearchViewModel GetVacancySearchViewModel();
    VacancySearchViewModel GetVacancySearchViewModel(CandidateViewModel candidateViewModel);
    CandidateViewModel GetCandidateViewModel();
    void OpenCandidateViewModel(int candidateId);
    CandidateSearchViewModel GetCandidateSearchViewModel();
    CandidateSearchViewModel GetCandidateSearchViewModel(VacancyViewModel vacancyViewModel);    
    void OpenDock(IDockable dockable);
    void OpenSingleDock(IDockable dockable);
    void CloseDock(IDockable dockable);
    IDocumentDock? Documents { get; }
    IProperties? Properties { get; } 
    IFactory Factory { get; } 
}
