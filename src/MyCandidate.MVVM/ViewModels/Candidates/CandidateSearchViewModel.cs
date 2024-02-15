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
using MyCandidate.MVVM.ViewModels.Shared;
using MyCandidate.MVVM.ViewModels.Tools;
using MyCandidate.MVVM.ViewModels.Vacancies;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Candidates;

public class CandidateSearchViewModel : Document
{
    private readonly ICandidateService _candidateService;
    private readonly IProperties _properties;
    private readonly IDataAccess<Country> _countriesData;
    private readonly IDataAccess<City> _citiesData;
    private readonly VacancyViewModel _vacancyViewModel;

    public CandidateSearchViewModel(ICandidateService candidateService, IDataAccess<Country> countries, IDataAccess<City> cities, IProperties properties)
    {
        _candidateService = candidateService;
        _countriesData = countries;
        _citiesData = cities;
        _properties = properties;

        Title = LocalizationService.Default["Candidate_Search"];
        LocalizationService.Default.OnCultureChanged += CultureChanged;

        var _cities = new List<City>() { new City() { Id = 0, CountryId = 0, Name = string.Empty } };
        _cities.AddRange(_citiesData.ItemsList.Where(x => x.Enabled == true));

        CitiesSource = new ObservableCollectionExtended<City>(_cities);
        CitiesSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter)
            .Bind(out _citiesList)
            .Subscribe();

        LoadCandidateSearch();

        Source.ToObservableChangeSet()
            .Page(Pager.Pager)
            .Do(x => Pager.PagingUpdate(x.Response.TotalSize, x.Response.Page, x.Response.Pages))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        SearchCmd = CreateSearchCmd();
    }

    public CandidateSearchViewModel(VacancyViewModel vacancyViewModel, ICandidateService candidateService, IDataAccess<Country> countries, IDataAccess<City> cities, IProperties properties)
    {
        _vacancyViewModel = vacancyViewModel;
        _candidateService = candidateService;
        _countriesData = countries;
        _citiesData = cities;
        _properties = properties;

        Title = $"{LocalizationService.Default["Candidate_Search_Vacancy"]} {_vacancyViewModel.Name}";
        LocalizationService.Default.OnCultureChanged += CultureChanged;

        var _cities = new List<City>() { new City() { Id = 0, CountryId = 0, Name = string.Empty } };
        _cities.AddRange(_citiesData.ItemsList.Where(x => x.Enabled == true));

        CitiesSource = new ObservableCollectionExtended<City>(_cities);
        CitiesSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter)
            .Bind(out _citiesList)
            .Subscribe();

        LoadCandidateSearch();

        Source.ToObservableChangeSet()
            .Page(Pager.Pager)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(x => Pager.PagingUpdate(x.Response.TotalSize, x.Response.Page, x.Response.Pages))
            .Bind(out _itemList)
            .DisposeMany()
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        SearchCmd = CreateSearchCmd();
    }

    private void LoadCandidateSearch()
    {
        if (_vacancyViewModel == null)
        {
            Skills = new SkillsViewModel(new List<SkillModel>(), _properties);
            CandidateSearch = new CandidateSearch();
        }
        else
        {
            Skills = new SkillsViewModel(_vacancyViewModel.Vacancy.VacancySkills.Select(x => new SkillModel(x.Id, x.Skill, x.Seniority)), _properties);
            var location = _vacancyViewModel.Vacancy.Office.Location;
            Country = Countries.First(x => x.Id == location.City.CountryId);
            City = Cities.First(x => x.Id == location.CityId);
            CandidateSearch = new CandidateSearch(_vacancyViewModel.Vacancy.VacancySkills)
            {
                CountryId = location.City.CountryId,
                CityId = location.CityId
            };
        }

        Enabled = true;
        Pager = new PagerViewModel();
        Source = new ObservableCollectionExtended<Candidate>();
        Skills.WhenAnyValue(x => x.IsValid).Subscribe(x => { this.RaisePropertyChanged(nameof(IsValid)); });
        this.WhenAnyValue(x => x.FirstName).Subscribe(x => { CandidateSearch.FirstName = x; });
        this.WhenAnyValue(x => x.LastName).Subscribe(x => { CandidateSearch.LastName = x; });
        this.WhenAnyValue(x => x.Enabled).Subscribe(x => { CandidateSearch.Enabled = x; });

        this.WhenAnyValue(x => x.City).Subscribe(
            x =>
            {
                if (x != null)
                {
                    CandidateSearch.CountryId = null;
                    CandidateSearch.CityId = x.Id > 0 ? x.Id : null;
                }
            }
        );

        this.WhenAnyValue(x => x.Country).Subscribe(
            x =>
            {
                if (x != null)
                {
                    CandidateSearch.CityId = null;
                    CandidateSearch.CountryId = x.Id > 0 ? x.Id : null;
                }

            }
        );
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        if (_vacancyViewModel == null)
        {
            Title = LocalizationService.Default["Candidate_Search"];
        }
        else
        {
            Title = $"{LocalizationService.Default["Candidate_Search_Vacancy"]} {_vacancyViewModel.Name}";
        }
    }

    #region CandidateSearch
    private CandidateSearch _candidateSearch;
    public CandidateSearch CandidateSearch
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
                && Skills.IsValid;
            return retVal;
        }
    }

    public IReactiveCommand OpenCmd { get; }
    private IReactiveCommand CreateOpenCmd()
    {
        return ReactiveCommand.Create(
                    async () =>
                        {
                            var doc = new CandidateViewModel(_candidateService, _countriesData, _citiesData, _properties, SelectedItem.Id)
                            {
                                Factory = this.Factory
                            };
                            this.Factory.AddDockable(this.Factory.GetDockable<IDocumentDock>("Documents"), doc);
                            this.Factory.SetActiveDockable(doc);
                        }, this.WhenAnyValue(x => x.SelectedItem, x => x.ItemList,
                            (obj, list) => obj != null && list.Count > 0)
                    );
    }
    public IReactiveCommand AddToVacancyCmd { get; }
    public IReactiveCommand SearchCmd { get; }
    private IReactiveCommand CreateSearchCmd()
    {
        return ReactiveCommand.Create(
            async () =>
            {
                CandidateSearch.Skills = Skills.Skills.Select(x => x.ToSkillVaue());
                Source.Load(_candidateService.Search(CandidateSearch));
                Pager.PagingUpdate(Source.Count());
            }, this.WhenAnyValue(x => x.IsValid, v => v == true)
        );
    }


    #region Pager
    private PagerViewModel _pagerViewModel;
    public PagerViewModel Pager
    {
        get => _pagerViewModel;
        set => this.RaiseAndSetIfChanged(ref _pagerViewModel, value);
    }
    #endregion

    #region ItemList
    public ObservableCollectionExtended<Candidate> Source;
    private readonly ReadOnlyObservableCollection<Candidate> _itemList;
    public ReadOnlyObservableCollection<Candidate> ItemList => _itemList;
    #endregion

    #region SelectedItem
    private Candidate? _selectedItem;
    public Candidate? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    #endregion

    #region FirstName
    private string _firstName;
    [StringLength(250)]
    public string FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);

    }
    #endregion

    #region LastName
    private string _lastName;
    [StringLength(250)]
    public string LastName
    {
        get => _lastName;
        set => this.RaiseAndSetIfChanged(ref _lastName, value);
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

    #region Cities
    public ObservableCollectionExtended<City> CitiesSource;
    private readonly ReadOnlyObservableCollection<City> _citiesList;
    public ReadOnlyObservableCollection<City> Cities => _citiesList;
    #endregion

    #region Countries
    private IEnumerable<Country> _countriesList;
    public IEnumerable<Country> Countries
    {
        get
        {
            if (_countriesList == null)
            {
                var retVal = new List<Country>() { new Country() { Id = 0, Name = string.Empty } };
                retVal.AddRange(_countriesData.ItemsList.Where(x => x.Enabled == true));
                _countriesList = retVal;
            }
            return _countriesList;
        }
    }
    #endregion

    #region Country
    private Country _country;
    public Country Country
    {
        get => _country;
        set => this.RaiseAndSetIfChanged(ref _country, value);
    }
    #endregion

    #region City
    private City _city;
    public City City
    {
        get => _city;
        set => this.RaiseAndSetIfChanged(ref _city, value);
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
