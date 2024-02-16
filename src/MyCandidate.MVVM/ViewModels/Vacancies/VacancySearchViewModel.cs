using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.PropertyGrid.Services;
using Dock.Model.Controls;
using Dock.Model.ReactiveUI.Controls;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.Models;
using MyCandidate.MVVM.Services;
using MyCandidate.MVVM.ViewModels.Candidates;
using MyCandidate.MVVM.ViewModels.Shared;
using MyCandidate.MVVM.ViewModels.Tools;
using MyCandidate.MVVM.ViewModels.Vacancies;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Vacancies;

public class VacancySearchViewModel : Document
{
    private readonly IVacancyService _vacancyService;
    private readonly IProperties _properties;
    private readonly IDataAccess<Company> _companiesData;
    private readonly IDataAccess<Office> _officesData;
    private readonly IDictionariesDataAccess _dictionariesDataAccess;
    private readonly CandidateViewModel _candidateViewModel;

    public VacancySearchViewModel(IVacancyService vacancyService, IDataAccess<Company> companiesData, IDataAccess<Office> officesData, IDictionariesDataAccess dictionariesDataAccess, IProperties properties)
    {
        _vacancyService = vacancyService;
        _companiesData = companiesData;
        _officesData = officesData;
        _dictionariesDataAccess = dictionariesDataAccess;        
        _properties = properties;

        Title = LocalizationService.Default["Vacancy_Search"];
        LocalizationService.Default.OnCultureChanged += CultureChanged;  

        var _offices = new List<Office>() { new Office() { Id = 0, CompanyId = 0, Name = string.Empty } };
        _offices.AddRange(_officesData.ItemsList.Where(x => x.Enabled == true));   

        OfficesSource = new ObservableCollectionExtended<Office>(_offices); 
        OfficesSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter)
            .Bind(out _officesList)
            .Subscribe(); 

        LoadVacancySearch();  

        Source.ToObservableChangeSet()
            .Page(Pager.Pager)
            .Do(x => Pager.PagingUpdate(x.Response.TotalSize, x.Response.Page, x.Response.Pages))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        SearchCmd = CreateSearchCmd();                     
    }

    public VacancySearchViewModel(CandidateViewModel candidateViewModel, IVacancyService vacancyService, IDataAccess<Company> companiesData, IDataAccess<Office> officesData, IDictionariesDataAccess dictionariesDataAccess, IProperties properties)
    {
        _candidateViewModel = candidateViewModel;
        _vacancyService = vacancyService;
        _companiesData = companiesData;
        _officesData = officesData;
        _dictionariesDataAccess = dictionariesDataAccess;
        _properties = properties;

        Title = $"{LocalizationService.Default["Vacancy_Search_Candidate"]} {_candidateViewModel.Candidate.Name}";
        LocalizationService.Default.OnCultureChanged += CultureChanged; 

        var _offices = new List<Office>() { new Office() { Id = 0, CompanyId = 0, Name = string.Empty } };
        _offices.AddRange(_officesData.ItemsList.Where(x => x.Enabled == true));   

        OfficesSource = new ObservableCollectionExtended<Office>(_offices); 
        OfficesSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter)
            .Bind(out _officesList)
            .Subscribe(); 

        LoadVacancySearch();  

        Source.ToObservableChangeSet()
            .Page(Pager.Pager)
            .Do(x => Pager.PagingUpdate(x.Response.TotalSize, x.Response.Page, x.Response.Pages))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        SearchCmd = CreateSearchCmd();                
    } 

    private void LoadVacancySearch()
    {
        if (_candidateViewModel == null)
        {
            Skills = new SkillsViewModel(new List<SkillModel>(), _properties);
            VacancySearch = new VacancySearch();
        }
        else
        {
            Skills = new SkillsViewModel(_candidateViewModel.Candidate.CandidateSkills.Select(x => new SkillModel(x.Id, x.Skill, x.Seniority)), _properties);            
            VacancySearch = new VacancySearch(_candidateViewModel.Candidate.CandidateSkills);
            var location = _candidateViewModel.Candidate.Location;
            Office = Offices.FirstOrDefault(x => x.Location.CityId == location.CityId);
            if(Office != null)
            {
                VacancySearch.OfficeId = Office.Id;
                Company = Companies.First(x => x.Id == Office.CompanyId);
            }
        }

        Enabled = true;
        VacancyStatus = VacancyStatuses.First();
        Pager = new PagerViewModel();
        Source = new ObservableCollectionExtended<Vacancy>();
        Skills.WhenAnyValue(x => x.IsValid).Subscribe(x => { this.RaisePropertyChanged(nameof(IsValid)); });
        this.WhenAnyValue(x => x.Name).Subscribe(x => { VacancySearch.Name = x; });
        this.WhenAnyValue(x => x.VacancyStatus).Subscribe(x => { VacancySearch.VacancyStatusId = (VacancyStatus.Id == 0)?null:VacancyStatus.Id; });
        this.WhenAnyValue(x => x.Enabled).Subscribe(x => { VacancySearch.Enabled = x; });

        this.WhenAnyValue(x => x.Office).Subscribe(
            x =>
            {
                if (x != null)
                {
                    VacancySearch.CompanyId = null;
                    VacancySearch.OfficeId = x.Id > 0 ? x.Id : null;
                }
            }
        );

        this.WhenAnyValue(x => x.Company).Subscribe(
            x =>
            {
                if (x != null)
                {
                    VacancySearch.OfficeId = null;
                    VacancySearch.CompanyId = x.Id > 0 ? x.Id : null;
                }
            }
        );
    }         

    private void CultureChanged(object? sender, EventArgs e)
    {
        if (_candidateViewModel == null)
        {
            Title = LocalizationService.Default["Vacancy_Search"];
        }
        else
        {
            Title = $"{LocalizationService.Default["Vacancy_Search_Candidate"]} {_candidateViewModel.Candidate.Name}";
        }
    }

    public IReactiveCommand OpenCmd { get; }
    private IReactiveCommand CreateOpenCmd()
    {
        return ReactiveCommand.Create(
                    async () =>
                        {
                            var doc = new VacancyViewModel(_vacancyService, _dictionariesDataAccess, _companiesData, _officesData, _properties, SelectedItem.Id)
                            {
                                Factory = this.Factory
                            };
                            this.Factory.AddDockable(this.Factory.GetDockable<IDocumentDock>("Documents"), doc);
                            this.Factory.SetActiveDockable(doc);
                        }, this.WhenAnyValue(x => x.SelectedItem, x => x.ItemList,
                            (obj, list) => obj != null && list.Count > 0)
                    );
    }
    public IReactiveCommand AddToCandidateCmd { get; }
    public IReactiveCommand SearchCmd { get; }
    private IReactiveCommand CreateSearchCmd()
    {
        return ReactiveCommand.Create(
            async () =>
            {
                VacancySearch.Skills = Skills.Skills.Select(x => x.ToSkillVaue());                
                Source.Load(_vacancyService.Search(VacancySearch));
                Pager.PagingUpdate(Source.Count());
            }, this.WhenAnyValue(x => x.IsValid, v => v == true)
        );
    }    

    public bool IsValid
    {
        get
        {
            var retVal = Validator.TryValidateObject(this, new ValidationContext(this), null, true)
                && Skills.IsValid;
            return retVal;
        }
    }    

    #region VacancySearch
    private VacancySearch _vacancySearch;
    public VacancySearch VacancySearch
    {
        get => _vacancySearch;
        set => this.RaiseAndSetIfChanged(ref _vacancySearch, value);
    }
    #endregion

    #region Filter
    private IObservable<Func<Office, bool>>? Filter =>
        this.WhenAnyValue(x => x.Company)
            .Select((x) => MakeFilter(x));

    private Func<Office, bool> MakeFilter(Company? company)
    {
        return item =>
        {
            var retVal = false;
            if (company != null)
            {
                retVal = item.CompanyId == company.Id;
            }

            return retVal;
        };
    }
    #endregion     

    #region Pager
    private PagerViewModel _pagerViewModel;
    public PagerViewModel Pager
    {
        get => _pagerViewModel;
        set => this.RaiseAndSetIfChanged(ref _pagerViewModel, value);
    }
    #endregion

    #region ItemList
    public ObservableCollectionExtended<Vacancy> Source;
    private readonly ReadOnlyObservableCollection<Vacancy> _itemList;
    public ReadOnlyObservableCollection<Vacancy> ItemList => _itemList;
    #endregion

    #region SelectedItem
    private Vacancy? _selectedItem;
    public Vacancy? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    #endregion    

    #region Name
    private string _name;
    [StringLength(250)]
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    #endregion    

    #region Enabled
    private bool _enabled;
    public bool Enabled
    {
        get => _enabled;
        set => this.RaiseAndSetIfChanged(ref _enabled, value);
    }
    #endregion

    #region Offices
    public ObservableCollectionExtended<Office> OfficesSource;
    private readonly ReadOnlyObservableCollection<Office> _officesList;
    public ReadOnlyObservableCollection<Office> Offices => _officesList;
    #endregion

    #region Companies
    private IEnumerable<Company> _companiesList;
    public IEnumerable<Company> Companies
    {
        get
        {
            if (_companiesList == null)
            {
                var retVal = new List<Company>() { new Company() { Id = 0, Name = string.Empty } };
                retVal.AddRange(_companiesData.ItemsList.Where(x => x.Enabled == true));
                _companiesList = retVal;
            }
            return _companiesList;
        }
    }
    #endregion

    #region Company
    private Company _company;
    public Company Company
    {
        get => _company;
        set => this.RaiseAndSetIfChanged(ref _company, value);
    }
    #endregion

    #region Office
    private Office _office;
    public Office Office
    {
        get => _office;
        set => this.RaiseAndSetIfChanged(ref _office, value);
    }
    #endregion

    #region VacancyStatuses
    private IEnumerable<VacancyStatus> _vacancyStatuses;
    public IEnumerable<VacancyStatus> VacancyStatuses
    {
        get
        {
            if (_vacancyStatuses == null)
            {
                var retVal = new List<VacancyStatus>() { new VacancyStatus() { Id = 0, Name = string.Empty } };
                retVal.AddRange(_dictionariesDataAccess.GetVacancyStatuses().Where(x => x.Enabled == true));
                _vacancyStatuses = retVal;
            }
            return _vacancyStatuses;
        }
    }
    #endregion  

    #region VacancyStatus
    private VacancyStatus _vacancyStatus;
    public VacancyStatus VacancyStatus
    {
        get => _vacancyStatus;
        set => this.RaiseAndSetIfChanged(ref _vacancyStatus, value);
    }
    #endregion      
    
    #region Skills
    private SkillsViewModel _Skills;
    public SkillsViewModel Skills
    {
        get => _Skills;
        set => this.RaiseAndSetIfChanged(ref _Skills, value);
    }
    #endregion  
}
