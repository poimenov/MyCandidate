using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.PropertyGrid.Services;
using Dock.Model.ReactiveUI.Controls;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.Models;
using MyCandidate.MVVM.Services;
using MyCandidate.MVVM.ViewModels.Candidates;
using MyCandidate.MVVM.ViewModels.Shared;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Vacancies;

public class VacancySearchViewModel : Document
{
    private readonly IAppServiceProvider _provider;
    private readonly CandidateViewModel _candidateViewModel;
    public CandidateViewModel CandidateViewModel => _candidateViewModel;

    public VacancySearchViewModel(IAppServiceProvider appServiceProvider)
    {
        _provider = appServiceProvider;
        Title = LocalizationService.Default["Vacancy_Search"];
        LocalizationService.Default.OnCultureChanged += CultureChanged;

        var _offices = new List<Office>() { new Office() { Id = 0, CompanyId = 0, Name = string.Empty } };
        _offices.AddRange(_provider.OfficeService.ItemsList.Where(x => x.Enabled == true));

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

    public VacancySearchViewModel(CandidateViewModel candidateViewModel, IAppServiceProvider appServiceProvider)
    {
        _candidateViewModel = candidateViewModel;
        _provider = appServiceProvider;

        Title = $"{LocalizationService.Default["Vacancy_Search_Candidate"]} {CandidateViewModel.Candidate.Name}";
        LocalizationService.Default.OnCultureChanged += CultureChanged;

        var _offices = new List<Office>() { new Office() { Id = 0, CompanyId = 0, Name = string.Empty } };
        _offices.AddRange(_provider.OfficeService.ItemsList.Where(x => x.Enabled == true));

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
        AddToCandidateCmd = CreateAddToCandidateCmd();
    }

    private void LoadVacancySearch()
    {
        if (CandidateViewModel == null)
        {
            Skills = new SkillsViewModel(new List<SkillModel>(), _provider.Properties);
            VacancySearch = new VacancySearch();
        }
        else
        {
            Skills = new SkillsViewModel(CandidateViewModel.Candidate.CandidateSkills.Select(x => new SkillModel(x.Id, x.Skill, x.Seniority)), _provider.Properties);
            VacancySearch = new VacancySearch(CandidateViewModel.Candidate.CandidateSkills);
            var location = CandidateViewModel.Candidate.Location;
            Office = Offices.FirstOrDefault(x => x.Location.CityId == location.CityId);
            if (Office != null)
            {
                VacancySearch.OfficeId = Office.Id;
                Company = Companies.First(x => x.Id == Office.CompanyId);
            }
        }

        Enabled = true;
        VacancyStatus = VacancyStatuses.First();
        Pager = new PagerViewModel();
        Source = new ObservableCollectionExtended<VacancyModel>();
        Skills.WhenAnyValue(x => x.IsValid).Subscribe(x => { this.RaisePropertyChanged(nameof(IsValid)); });
        this.WhenAnyValue(x => x.Name).Subscribe(x => { VacancySearch.Name = x; });
        this.WhenAnyValue(x => x.VacancyStatus).Subscribe(x => { VacancySearch.VacancyStatusId = (VacancyStatus.Id == 0) ? null : VacancyStatus.Id; });
        this.WhenAnyValue(x => x.Enabled).Subscribe(x => { VacancySearch.Enabled = x; });

        this.WhenAnyValue(x => x.SelectedItem).Subscribe(
            x => 
            {
                if (_provider.Properties != null && x != null)
                {
                    _provider.Properties.SelectedItem = x;
                }
            }
        );

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
        if (CandidateViewModel == null)
        {
            Title = LocalizationService.Default["Vacancy_Search"];
        }
        else
        {
            Title = $"{LocalizationService.Default["Vacancy_Search_Candidate"]} {CandidateViewModel.Candidate.Name}";
        }
    }

    public IReactiveCommand OpenCmd { get; }
    private IReactiveCommand CreateOpenCmd()
    {
        return ReactiveCommand.Create(
            async () =>
                {
                    var existed = _provider.Documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(VacancyViewModel) && ((VacancyViewModel)x).VacancyId == SelectedItem.Vacancy.Id);
                    if (existed != null)
                    {
                        _provider.Factory.SetActiveDockable(existed);
                    }
                    else
                    {
                        _provider.OpenDock(_provider.GetVacancyViewModel(SelectedItem.Vacancy.Id));
                    }
                }, this.WhenAnyValue(x => x.SelectedItem, x => x.ItemList,
                    (obj, list) => obj != null && list.Count > 0)
            );
    }
    public IReactiveCommand AddToCandidateCmd { get; }
    private IReactiveCommand CreateAddToCandidateCmd()
    {
        return ReactiveCommand.Create(
            async () =>
                {
                    if (!_provider.Documents.VisibleDockables.Any(x => x.GetType() == typeof(CandidateViewModel) && ((CandidateViewModel)x).CandidateId == CandidateViewModel.CandidateId))
                    {
                        _provider.Factory.AddDockable(_provider.Documents, CandidateViewModel);
                    }

                    var status = new SelectionStatus { Id = 1, Name = SelectionStatusNames.SetContact, Enabled = true };
                    var newItem = new CandidateOnVacancy
                    {
                        Candidate = CandidateViewModel.Candidate,
                        CandidateId = CandidateViewModel.Candidate.Id,
                        Vacancy = SelectedItem.Vacancy,
                        VacancyId = SelectedItem.Vacancy.Id,
                        SelectionStatus = status,
                        SelectionStatusId = status.Id,
                        CreationDate = DateTime.Now,
                        LastModificationDate = DateTime.Now
                    };

                    _provider.Factory.SetActiveDockable(CandidateViewModel);
                    CandidateViewModel.CandidatesOnVacancy.Add(newItem);
                }, this.WhenAnyValue(x => x.SelectedItem, x => x.CandidateViewModel,
                    (obj, vm) => obj != null && vm != null && !vm.CandidatesOnVacancy.ItemList.Any(y => y.VacancyId == obj.Vacancy.Id))
            );
    }
    public IReactiveCommand SearchCmd { get; }
    private IReactiveCommand CreateSearchCmd()
    {
        return ReactiveCommand.Create(
            async () =>
            {
                VacancySearch.Skills = Skills.Skills.Select(x => x.ToSkillVaue());
                Source.Load(_provider.VacancyService.Search(VacancySearch).Select(x => new VacancyModel(x)));
                Pager.PagingUpdate(Source.Count());
            }, this.WhenAnyValue(x => x.IsValid, v => v == true)
        );
    }

    public bool AddToCandidateVisible => AddToCandidateCmd != null;

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
    public ObservableCollectionExtended<VacancyModel> Source;
    private readonly ReadOnlyObservableCollection<VacancyModel> _itemList;
    public ReadOnlyObservableCollection<VacancyModel> ItemList => _itemList;
    #endregion

    #region SelectedItem
    private VacancyModel? _selectedItem;
    public VacancyModel? SelectedItem
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
                retVal.AddRange(_provider.CompanyService.ItemsList.Where(x => x.Enabled == true));
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
                retVal.AddRange(_provider.DictionariesDataAccess.GetVacancyStatuses().Where(x => x.Enabled == true));
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
