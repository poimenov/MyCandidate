using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Dock.Model.Controls;
using Dock.Model.Core;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.ViewModels;
using MyCandidate.MVVM.ViewModels.Candidates;
using MyCandidate.MVVM.ViewModels.Dictionary;
using MyCandidate.MVVM.ViewModels.Tools;
using MyCandidate.MVVM.ViewModels.Vacancies;

namespace MyCandidate.MVVM.Services;

public class AppServiceProvider : IAppServiceProvider
{
    private App CurrentApplication => (App)Application.Current!;
    private readonly DockFactory _factory;
    public IFactory Factory => _factory;

    public AppServiceProvider()
    {
        _factory = new DockFactory();
    }

    #region Documents
    private IDocumentDock? _documents;
    public IDocumentDock? Documents
    {
        get
        {
            if (_documents == null)
            {
                _documents = _factory?.GetDockable<IDocumentDock>("Documents");
            }

            return _documents;
        }
    }
    #endregion

    #region Properties
    private IProperties? _properties;
    public IProperties? Properties
    {
        get
        {
            if (_properties == null)
            {
                _properties = _factory?.GetDockable<PropertiesViewModel>("Properties") as IProperties;
            }

            return _properties;
        }
    }
    #endregion    

    #region CandidateService
    private ICandidateService? _candidateService;
    public ICandidateService CandidateService
    {
        get
        {
            if (_candidateService == null)
            {
                _candidateService = CurrentApplication.GetRequiredService<ICandidateService>();
            }

            return _candidateService;
        }
    }
    #endregion

    #region VacancyService
    private IVacancyService? _vacancyService;
    public IVacancyService VacancyService
    {
        get
        {
            if (_vacancyService == null)
            {
                _vacancyService = CurrentApplication.GetRequiredService<IVacancyService>();
            }

            return _vacancyService;
        }
    }
    #endregion

    #region CountryService
    private IDictionaryService<Country>? _countryService;
    public IDictionaryService<Country> CountryService
    {
        get
        {
            if (_countryService == null)
            {
                _countryService = CurrentApplication.GetRequiredService<IDictionaryService<Country>>();
            }

            return _countryService;
        }
    }
    #endregion

    #region CityService
    private IDictionaryService<City>? _cityService;
    public IDictionaryService<City> CityService
    {
        get
        {
            if (_cityService == null)
            {
                _cityService = CurrentApplication.GetRequiredService<IDictionaryService<City>>();
            }

            return _cityService;
        }
    }
    #endregion

    #region CompanyService
    private IDictionaryService<Company>? _companyService;
    public IDictionaryService<Company> CompanyService
    {
        get
        {
            if (_companyService == null)
            {
                _companyService = CurrentApplication.GetRequiredService<IDictionaryService<Company>>();
            }

            return _companyService;
        }
    }
    #endregion

    #region OfficeService
    private IDictionaryService<Office>? _officeService;
    public IDictionaryService<Office> OfficeService
    {
        get
        {
            if (_officeService == null)
            {
                _officeService = CurrentApplication.GetRequiredService<IDictionaryService<Office>>();
            }

            return _officeService;
        }
    }
    #endregion

    #region SkillCategoryService
    private IDictionaryService<SkillCategory>? _skillCategoryService;
    public IDictionaryService<SkillCategory> SkillCategoryService
    {
        get
        {
            if (_skillCategoryService == null)
            {
                _skillCategoryService = CurrentApplication.GetRequiredService<IDictionaryService<SkillCategory>>();
            }

            return _skillCategoryService;
        }
    }
    #endregion

    #region SkillService
    private IDictionaryService<Skill>? _skillService;
    public IDictionaryService<Skill> SkillService
    {
        get
        {
            if (_skillService == null)
            {
                _skillService = CurrentApplication.GetRequiredService<IDictionaryService<Skill>>();
            }

            return _skillService;
        }
    }

    #endregion

    #region DictionariesDataAccess
    private IDictionariesDataAccess? _dictionariesDataAccess;
    public IDictionariesDataAccess DictionariesDataAccess
    {
        get
        {
            if (_dictionariesDataAccess == null)
            {
                _dictionariesDataAccess = CurrentApplication.GetRequiredService<IDictionariesDataAccess>();
            }

            return _dictionariesDataAccess;
        }
    }
    #endregion

    public DictionaryViewModel<Country> GetCountriesViewModel()
    {
        var retVal = CurrentApplication.GetRequiredService<DictionaryViewModel<Country>>();
        retVal.Properties = Properties;
        return retVal;
    }

    public DictionaryViewModel<City> GetCitiesViewModel()
    {
        var retVal = CurrentApplication.GetRequiredService<DictionaryViewModel<City>>();
        retVal.Properties = Properties;
        return retVal;
    }

    public DictionaryViewModel<SkillCategory> GetCategoriesViewModel()
    {
        var retVal = CurrentApplication.GetRequiredService<DictionaryViewModel<SkillCategory>>();
        retVal.Properties = Properties;
        return retVal;
    }

    public DictionaryViewModel<Skill> GetSkillsViewModel()
    {
        var retVal = CurrentApplication.GetRequiredService<DictionaryViewModel<Skill>>();
        retVal.Properties = Properties;
        return retVal;
    }

    public DictionaryViewModel<Company> GetCompaniesViewModel()
    {
        var retVal = CurrentApplication.GetRequiredService<DictionaryViewModel<Company>>();
        retVal.Properties = Properties;
        return retVal;
    }

    public DictionaryViewModel<Office> GetOfficiesViewModel()
    {
        var retVal = CurrentApplication.GetRequiredService<DictionaryViewModel<Office>>();
        retVal.Properties = Properties;
        return retVal;
    }

    public VacancyViewModel GetVacancyViewModel()
    {
        return new VacancyViewModel(this);
    }

    public VacancySearchViewModel GetVacancySearchViewModel()
    {
        return new VacancySearchViewModel(this);
    }

    public VacancySearchViewModel GetVacancySearchViewModel(CandidateViewModel candidateViewModel)
    {
        return new VacancySearchViewModel(candidateViewModel, this);
    }

    public CandidateViewModel GetCandidateViewModel()
    {
        return new CandidateViewModel(this);
    }

    public async Task OpenCandidateViewModelAsync(int candidateId)
    {
        if (Documents is { } && Documents?.VisibleDockables != null)
        {
            var existed = Documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(CandidateViewModel) && ((CandidateViewModel)x).CandidateId == candidateId);
            if (existed != null)
            {
                Factory.SetActiveDockable(existed);
            }
            else if (await CandidateService.ExistAsync(candidateId))
            {
                OpenDock(new CandidateViewModel(this, candidateId));
            }
        }
    }

    public async Task OpenVacancyViewModelAsync(int vacancyId)
    {
        if (Documents is { } && Documents?.VisibleDockables != null)
        {
            var existed = Documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(VacancyViewModel) && ((VacancyViewModel)x).VacancyId == vacancyId);
            if (existed != null)
            {
                Factory.SetActiveDockable(existed);
            }
            else if (await VacancyService.ExistAsync(vacancyId))
            {
                OpenDock(new VacancyViewModel(this, vacancyId));
            }
        }
    }

    public CandidateSearchViewModel GetCandidateSearchViewModel()
    {
        return new CandidateSearchViewModel(this);
    }

    public CandidateSearchViewModel GetCandidateSearchViewModel(VacancyViewModel vacancyViewModel)
    {
        return new CandidateSearchViewModel(this, vacancyViewModel);
    }

    public void OpenDock(IDockable dockable)
    {
        if (Documents is { } && Documents?.VisibleDockables != null)
        {
            dockable.CanFloat = false;
            _factory.AddDockable(Documents, dockable);
            _factory.SetActiveDockable(dockable);
        }
    }

    public void OpenSingleDock(IDockable dockable)
    {
        if (Documents is { } && Documents?.VisibleDockables != null)
        {
            var existed = Documents.VisibleDockables.FirstOrDefault(x => x.GetType() == dockable.GetType());
            if (existed != null)
            {
                Factory.SetActiveDockable(existed);
            }
            else
            {
                OpenDock(dockable);
            }
        }
    }

    public void CloseDock(IDockable dockable)
    {
        if (dockable != null && Documents is { } && Documents?.VisibleDockables != null)
        {
            _factory.CloseDockable(dockable);
        }
    }

}
