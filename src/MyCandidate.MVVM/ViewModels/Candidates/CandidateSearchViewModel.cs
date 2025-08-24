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
using MyCandidate.MVVM.ViewModels.Shared;
using MyCandidate.MVVM.ViewModels.Vacancies;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Candidates;

public class CandidateSearchViewModel : Document
{
    private readonly IAppServiceProvider _provider;
    private readonly VacancyViewModel? _vacancyViewModel;
    public VacancyViewModel? VacancyViewModel => _vacancyViewModel;

    public CandidateSearchViewModel(IAppServiceProvider appServiceProvider)
    {
        _provider = appServiceProvider;

        Title = LocalizationService.Default["Candidate_Search"];
        LocalizationService.Default.OnCultureChanged += CultureChanged;

        var _cities = new List<City>() { new City() { Id = 0, CountryId = 0, Name = string.Empty } };
        _cities.AddRange(_provider.CityService.GetItemsListAsync().Result.Where(x => x.Enabled == true));

        CitiesSource = new ObservableCollectionExtended<City>(_cities);
        CitiesSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter!)
            .Bind(out _citiesList)
            .Subscribe();

        Source = new ObservableCollectionExtended<CandidateModel>();
        LoadCandidateSearch();

        Source.ToObservableChangeSet()
            .Page(Pager!.Pager)
            .Do(x => Pager.PagingUpdate(x.Response.TotalSize, x.Response.Page, x.Response.Pages))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        SearchCmd = CreateSearchCmd();
        AddToVacancyCmd = CreateAddToVacancyCmd();
    }

    public CandidateSearchViewModel(VacancyViewModel vacancyViewModel, IAppServiceProvider appServiceProvider)
    {
        _vacancyViewModel = vacancyViewModel;
        _provider = appServiceProvider;

        Title = $"{LocalizationService.Default["Candidate_Search_Vacancy"]} {VacancyViewModel?.Name}";
        LocalizationService.Default.OnCultureChanged += CultureChanged;

        var _cities = new List<City>() { new City() { Id = 0, CountryId = 0, Name = string.Empty } };
        _cities.AddRange(_provider.CityService.GetItemsListAsync().Result.Where(x => x.Enabled == true));

        CitiesSource = new ObservableCollectionExtended<City>(_cities);
        CitiesSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter!)
            .Bind(out _citiesList)
            .Subscribe();

        Source = new ObservableCollectionExtended<CandidateModel>();
        LoadCandidateSearch();

        Source.ToObservableChangeSet()
            .Page(Pager!.Pager)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(x => Pager.PagingUpdate(x.Response.TotalSize, x.Response.Page, x.Response.Pages))
            .Bind(out _itemList)
            .DisposeMany()
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        SearchCmd = CreateSearchCmd();
        AddToVacancyCmd = CreateAddToVacancyCmd();
    }

    private void LoadCandidateSearch()
    {
        if (VacancyViewModel == null)
        {
            Skills = new SkillsViewModel(new List<SkillModel>(), _provider.Properties!);
            CandidateSearch = new CandidateSearch();
        }
        else
        {
            Skills = new SkillsViewModel(VacancyViewModel.Vacancy.VacancySkills.Select(x => new SkillModel(x.Id, x.Skill!, x.Seniority!)), _provider.Properties!);
            var location = VacancyViewModel.Vacancy.Office?.Location;
            if (location != null)
            {
                Country = Countries.First(x => x.Id == location.City!.CountryId);
                City = Cities.First(x => x.Id == location.CityId);
                CandidateSearch = new CandidateSearch(VacancyViewModel.Vacancy.VacancySkills)
                {
                    CountryId = location.City!.CountryId,
                    CityId = location.CityId
                };
            }
        }

        Enabled = true;
        SearchStrictBySeniority = true;
        Pager = new PagerViewModel();
        Skills!.WhenAnyValue(x => x.IsValid).Subscribe(x => { this.RaisePropertyChanged(nameof(IsValid)); });
        this.WhenAnyValue(x => x.FirstName).Subscribe(x => { CandidateSearch!.FirstName = x ?? string.Empty; });
        this.WhenAnyValue(x => x.LastName).Subscribe(x => { CandidateSearch!.LastName = x ?? string.Empty; });
        this.WhenAnyValue(x => x.CandidateTitle).Subscribe(x => { CandidateSearch!.Title = x ?? string.Empty; });
        this.WhenAnyValue(x => x.Enabled).Subscribe(x => { CandidateSearch!.Enabled = x; });
        this.WhenAnyValue(x => x.SearchStrictBySeniority).Subscribe(x => { CandidateSearch!.SearchStrictBySeniority = x; });

        this.WhenAnyValue(x => x.SelectedItem).Subscribe(
            x =>
            {
                if (_provider.Properties != null && x != null)
                {
                    _provider.Properties.SelectedItem = x;
                }
            }
        );

        this.WhenAnyValue(x => x.City).Subscribe(
            x =>
            {
                if (x != null)
                {
                    CandidateSearch!.CountryId = null;
                    CandidateSearch.CityId = x.Id > 0 ? x.Id : null;
                }
            }
        );

        this.WhenAnyValue(x => x.Country).Subscribe(
            x =>
            {
                if (x != null)
                {
                    CandidateSearch!.CityId = null;
                    CandidateSearch.CountryId = x.Id > 0 ? x.Id : null;
                }
            }
        );
    }
    private void CultureChanged(object? sender, EventArgs e)
    {
        if (VacancyViewModel == null)
        {
            Title = LocalizationService.Default["Candidate_Search"];
        }
        else
        {
            Title = $"{LocalizationService.Default["Candidate_Search_Vacancy"]} {VacancyViewModel.Name}";
        }
    }

    #region CandidateSearch
    private CandidateSearch? _candidateSearch;
    public CandidateSearch? CandidateSearch
    {
        get => _candidateSearch;
        set => this.RaiseAndSetIfChanged(ref _candidateSearch, value);
    }
    #endregion

    #region Filter
    private IObservable<Func<City, bool>>? Filter =>
        this.WhenAnyValue(x => x.Country)
            .Select((x) => MakeFilter(x));

    private Func<City, bool> MakeFilter(Country? country)
    {
        return item =>
        {
            var retVal = false;
            if (country != null)
            {
                retVal = item.CountryId == country.Id;
            }

            return retVal;
        };
    }
    #endregion    

    public bool IsValid
    {
        get
        {
            var retVal = Validator.TryValidateObject(this, new ValidationContext(this), null, true)
                && Skills!.IsValid;
            return retVal;
        }
    }

    #region Commands
    public IReactiveCommand OpenCmd { get; }
    private IReactiveCommand CreateOpenCmd()
    {
        return ReactiveCommand.Create(
            () =>
                {
                    _provider.OpenCandidateViewModel(SelectedItem!.Candidate.Id);
                }, this.WhenAnyValue(x => x.SelectedItem, x => x.ItemList,
                    (obj, list) => obj != null && list.Count > 0)
            );
    }
    public IReactiveCommand AddToVacancyCmd { get; }
    private IReactiveCommand CreateAddToVacancyCmd()
    {
        return ReactiveCommand.Create(
            () =>
                {
                    var visibleDockables = _provider.Documents?.VisibleDockables;
                    if (visibleDockables != null &&
                        !visibleDockables.Any(x => x != null && x.GetType() == typeof(VacancyViewModel) &&
                        ((VacancyViewModel)x).VacancyId == VacancyViewModel?.VacancyId))
                    {
                        _provider.OpenDock(VacancyViewModel!);
                    }

                    var status = new SelectionStatus { Id = 1, Name = SelectionStatusNames.SetContact, Enabled = true };
                    var newItem = new CandidateOnVacancy
                    {
                        Candidate = SelectedItem!.Candidate,
                        CandidateId = SelectedItem.Candidate.Id,
                        Vacancy = VacancyViewModel!.Vacancy,
                        VacancyId = VacancyViewModel.Vacancy.Id,
                        SelectionStatus = status,
                        SelectionStatusId = status.Id,
                        CreationDate = DateTime.Now,
                        LastModificationDate = DateTime.Now
                    };

                    _provider.Factory.SetActiveDockable(VacancyViewModel);
                    VacancyViewModel!.CandidatesOnVacancy!.Add(newItem);
                }, this.WhenAnyValue(x => x.SelectedItem, x => x.VacancyViewModel,
                    (obj, vm) => obj != null && vm != null && !vm.CandidatesOnVacancy!.ItemList.Any(y => y.CandidateId == obj.Candidate.Id))
            );
    }
    public IReactiveCommand SearchCmd { get; }
    private IReactiveCommand CreateSearchCmd()
    {
        return ReactiveCommand.Create(
            () =>
            {
                CandidateSearch!.Skills = Skills!.Skills.Select(x => x.ToSkillVaue());
                Source.Load(_provider.CandidateService.Search(CandidateSearch).Select(x => new CandidateModel(x)));
                Pager!.PagingUpdate(Source.Count());
            }, this.WhenAnyValue(x => x.IsValid, v => v == true)
        );
    }
    #endregion

    public bool AddToVacancyVisible => AddToVacancyCmd != null;

    #region Pager
    private PagerViewModel? _pagerViewModel;
    public PagerViewModel? Pager
    {
        get => _pagerViewModel;
        set => this.RaiseAndSetIfChanged(ref _pagerViewModel, value);
    }
    #endregion

    #region ItemList
    public ObservableCollectionExtended<CandidateModel> Source;
    private readonly ReadOnlyObservableCollection<CandidateModel> _itemList;
    public ReadOnlyObservableCollection<CandidateModel> ItemList => _itemList;
    #endregion

    #region SelectedItem
    private CandidateModel? _selectedItem;
    public CandidateModel? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    #endregion

    #region FirstName
    private string? _firstName;
    [StringLength(250)]
    public string? FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);

    }
    #endregion

    #region LastName
    private string? _lastName;
    [StringLength(250)]
    public string? LastName
    {
        get => _lastName;
        set => this.RaiseAndSetIfChanged(ref _lastName, value);
    }
    #endregion    

    #region CandidateTitle
    private string? _candidateTitle;
    [StringLength(250)]
    public string? CandidateTitle
    {
        get => _candidateTitle;
        set => this.RaiseAndSetIfChanged(ref _candidateTitle, value);
    }
    #endregion     

    #region Enabled
    private bool? _enabled;
    public bool? Enabled
    {
        get => _enabled;
        set => this.RaiseAndSetIfChanged(ref _enabled, value);
    }
    #endregion

    #region Cities
    public ObservableCollectionExtended<City> CitiesSource;
    private readonly ReadOnlyObservableCollection<City> _citiesList;
    public ReadOnlyObservableCollection<City> Cities => _citiesList;
    #endregion

    #region Countries
    private IEnumerable<Country>? _countriesList;
    public IEnumerable<Country> Countries
    {
        get
        {
            if (_countriesList == null)
            {
                var retVal = new List<Country>() { new Country() { Id = 0, Name = string.Empty } };
                retVal.AddRange(_provider.CountryService.GetItemsListAsync().Result.Where(x => x.Enabled == true));
                _countriesList = retVal;
            }
            return _countriesList;
        }
    }
    #endregion

    #region Country
    private Country? _country;
    public Country? Country
    {
        get => _country;
        set => this.RaiseAndSetIfChanged(ref _country, value);
    }
    #endregion

    #region City
    private City? _city;
    public City? City
    {
        get => _city;
        set => this.RaiseAndSetIfChanged(ref _city, value);
    }
    #endregion

    #region Skills
    private SkillsViewModel? _skills;
    public SkillsViewModel? Skills
    {
        get => _skills;
        set => this.RaiseAndSetIfChanged(ref _skills, value);
    }
    #endregion

    #region SearchStrictBySeniority
    private bool _searchStrictBySeniority;
    public bool SearchStrictBySeniority
    {
        get => _searchStrictBySeniority;
        set => this.RaiseAndSetIfChanged(ref _searchStrictBySeniority, value);
    }
    #endregion
}
