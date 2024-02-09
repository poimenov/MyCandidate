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
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.DataAnnotations;
using MyCandidate.MVVM.Extensions;
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

        Pager = new PagerViewModel();
        Source = new ObservableCollectionExtended<Candidate>(_candidateService.Search(new CandidateSearch()));
        Source.ToObservableChangeSet()
            .Page(Pager.Pager)
            .Do(x => Pager.PagingUpdate(x.Response.TotalSize, x.Response.Page, x.Response.Pages))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();        
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
        Pager = new PagerViewModel();
    }

    private void LoadCandidateSearch()
    {
        

        if (_vacancyViewModel == null)
        {
            Skills = new SkillsViewModel(new List<SkillModel>(), _properties);
        }
        else
        {
            Skills = new SkillsViewModel(_vacancyViewModel.Vacancy.VacancySkills.Select(x => new SkillModel(x.Id, x.Skill, x.Seniority)), _properties);
            Skills.WhenAnyValue(x => x.IsValid).Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); }); 
        }
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
    public IReactiveCommand AddToVacancyCmd { get; }

    private PagerViewModel _pagerViewModel;
    public PagerViewModel Pager
    {
        get => _pagerViewModel;
        set => this.RaiseAndSetIfChanged(ref _pagerViewModel, value);
    }

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
        set
        {
            this.RaiseAndSetIfChanged(ref _firstName, value);
        }
    }
    #endregion

    #region LastName
    private string _lastName;
    [StringLength(250)]
    public string LastName
    {
        get => _lastName;
        set
        {
            this.RaiseAndSetIfChanged(ref _lastName, value);
        }
    }
    #endregion    

    #region Enabled
    private bool _enabled;
    public bool Enabled
    {
        get => _enabled;
        set
        {
            this.RaiseAndSetIfChanged(ref _enabled, value);
        }
    }
    #endregion  

    public ObservableCollectionExtended<City> CitiesSource;
    private readonly ReadOnlyObservableCollection<City> _citiesList;
    public ReadOnlyObservableCollection<City> Cities => _citiesList;

    private IEnumerable<Country> _countriesList;
    public IEnumerable<Country> Countries
    {
        get
        {
            if(_countriesList == null)
            {
                var retVal =  new List<Country>() { new Country() { Id = 0, Name = string.Empty } };
                retVal.AddRange(_countriesData.ItemsList.Where(x => x.Enabled == true));
                _countriesList = retVal;
            }
            return _countriesList;
        }
    }

    private Country _country;
    public Country Country
    {
        get => _country;
        set => this.RaiseAndSetIfChanged(ref _country, value);
    }

    private City _city;
    public City City
    {
        get => _city;
        set => this.RaiseAndSetIfChanged(ref _city, value);
    } 

    private SkillsViewModel _Skills;
    public SkillsViewModel Skills
    {
        get => _Skills;
        set => this.RaiseAndSetIfChanged(ref _Skills, value);
    }      
}
