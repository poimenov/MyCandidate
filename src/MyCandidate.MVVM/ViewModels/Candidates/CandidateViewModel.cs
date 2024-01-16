using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.PropertyGrid.Services;
using Dock.Model.ReactiveUI.Controls;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.Extensions;
using MyCandidate.MVVM.Services;
using MyCandidate.MVVM.ViewModels.Tools;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Candidates;

public class CandidateViewModel : Document
{
    private readonly ICandidateService _candidateService;
    private App CurrentApplication => (App)Application.Current;
    private IDataAccess<Country> Countries => CurrentApplication.GetRequiredService<IDataAccess<Country>>();
    private IDataAccess<City> Cities => CurrentApplication.GetRequiredService<IDataAccess<City>>();
    private Candidate _candidate;

    public CandidateViewModel(ICandidateService candidateService, IProperties properties)
    {
        _candidateService = candidateService;
        var cities = Cities;
        var defaultCity = cities.ItemsList.First();
        _candidate = new Candidate
        {
            Enabled = true,
            Location = new Location
            {
                Enabled = true,
                CityId = defaultCity.CountryId,
                City = defaultCity
            },
            CandidateResources = new List<CandidateResource>(),
            CandidateSkills = new List<CandidateSkill>()
        };
        Title = LocalizationService.Default["New_Candidate"];
        FirstName = string.Empty;
        LastName = string.Empty;
        Enabled = _candidate.Enabled;
        Location = new LocationViewModel(Countries, cities)
        {
            Location = _candidate.Location
        };                       
        
        CandidateResources = new CandidateResourcesViewModel(_candidate, properties);

        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = ReactiveCommand.Create(() => { OnCancel(); });
        SaveCmd = ReactiveCommand.Create(() => { OnSave(); });
        DeleteCmd = ReactiveCommand.Create(() => { OnDelete(); });
    }

    public CandidateViewModel(ICandidateService candidateService, IProperties properties, int candidateId)
    {
        _candidateService = candidateService;
        _candidate = _candidateService.Get(candidateId);
        Title = _candidate.Name;
        FirstName = _candidate.FirstName;
        LastName = _candidate.LastName;
        Enabled = _candidate.Enabled;
        Location = new LocationViewModel(Countries, Cities)
        {
            Location = _candidate.Location
        };
        CandidateResources = new CandidateResourcesViewModel(_candidate, properties);        

        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = ReactiveCommand.Create(() => { OnCancel(); });
        SaveCmd = ReactiveCommand.Create(() => { OnSave(); });
        DeleteCmd = ReactiveCommand.Create(() => { OnDelete(); });
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        Title = LocalizationService.Default["New_Candidate"];
    }

    public IReactiveCommand SaveCmd { get; }
    public IReactiveCommand CancelCmd { get; }
    public IReactiveCommand DeleteCmd { get; }


    #region FirstName
    private string _firstName;
    [Required]
    [StringLength(250, MinimumLength = 2)]
    public string FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);
    }
    #endregion

    #region LastName
    private string _lastName;
    [Required]
    [StringLength(250, MinimumLength = 2)]
    public string LastName
    {
        get => _lastName;
        set => this.RaiseAndSetIfChanged(ref _lastName, value);
    }
    #endregion

    #region BirthDate
    private DateTimeOffset? _birthDate;
    [Required]
    public DateTimeOffset? BirthDate
    {
        get => _birthDate;
        set
        {
            this.RaiseAndSetIfChanged(ref _birthDate, value);
            this.RaisePropertyChanged(nameof(this.Age));
        }
    }
    #endregion

    public string Age
    {
        get => _birthDate.HasValue ? $"{LocalizationService.Default["Age"]} {_birthDate.Value.DateTime.GetAge()}" : string.Empty;
    }

    #region Location
    private LocationViewModel _location;
    public LocationViewModel Location
    {
        get => _location;
        set => this.RaiseAndSetIfChanged(ref _location, value);
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

    private CandidateResourcesViewModel _candidateResources;
    public CandidateResourcesViewModel CandidateResources 
    {
        get => _candidateResources;
        set => this.RaiseAndSetIfChanged(ref _candidateResources, value);
    }

    private void OnCancel()
    {
        //
    }

    private void OnSave()
    {
        //
    }

    private void OnDelete()
    {
        //
    }
}
