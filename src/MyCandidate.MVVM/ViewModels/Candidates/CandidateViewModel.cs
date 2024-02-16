using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.PropertyGrid.Services;
using Dock.Model.ReactiveUI.Controls;
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
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Candidates;

public class CandidateViewModel : Document
{
    private readonly ICandidateService _candidateService;
    private readonly IProperties _properties;
    private readonly IDataAccess<Country> _countries;
    private readonly IDataAccess<City> _cities;
    private Candidate _candidate;

    public CandidateViewModel(ICandidateService candidateService, IDataAccess<Country> countries, IDataAccess<City> cities, IProperties properties)
    {
        _candidateService = candidateService;
         _countries = countries;
        _cities = cities;       
        _properties = properties;
        _candidate = NewCandidate;
        LoadCandidate();
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = CreateCancelCmd();
        SaveCmd = CreateSaveCmd();
        DeleteCmd = CreateDeleteCmd();
    }

    public CandidateViewModel(ICandidateService candidateService, IDataAccess<Country> countries, IDataAccess<City> cities, IProperties properties, int candidateId)
    {
        _candidateService = candidateService;
        _countries = countries;
        _cities = cities;        
        _properties = properties;
        _candidate = _candidateService.Get(candidateId);
        LoadCandidate();
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = CreateCancelCmd();
        SaveCmd = CreateSaveCmd();
        DeleteCmd = CreateDeleteCmd();
    }

    private Candidate NewCandidate
    {
        get
        {
            var defaultCity = _cities.ItemsList.First();
            return new Candidate
            {
                Id = 0,
                FirstName = string.Empty,
                LastName = string.Empty,
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
        }
    }

    private void LoadCandidate()
    {
        if(_candidate.Id == 0)
        {
            Title = LocalizationService.Default["New_Candidate"];
            BirthDate = null;
        }
        else
        {
            Title = _candidate.Name;
            BirthDate = _candidate.BirthDate;
        } 
        
        FirstName = _candidate.FirstName;
        LastName = _candidate.LastName;
        Enabled = _candidate.Enabled;
        Location = new LocationViewModel(_countries, _cities)
        {
            Location = _candidate.Location
        };

        CandidateResources = new CandidateResourcesViewModel(_candidate, _properties);
        CandidateResources.WhenAnyValue(x => x.IsValid).Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); });
        CandidateSkills = new SkillsViewModel(_candidate.CandidateSkills.Select(x => new SkillModel(x.Id, x.Skill, x.Seniority)), _properties);
        CandidateSkills.WhenAnyValue(x => x.IsValid).Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); });  
        this.RaisePropertyChanged(nameof(CandidateId));      
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        if(_candidate.Id == 0)
        {
            Title = LocalizationService.Default["New_Candidate"];
        }        
    }

    public Candidate Candidate =>_candidate;

    public bool IsValid
    {
        get
        {
            var retVal = Validator.TryValidateObject(this, new ValidationContext(this), null, true)
                && CandidateResources.IsValid
                && CandidateSkills.IsValid;
            return retVal;
        }
    }

    public int CandidateId
    {
        get
        {
            return _candidate.Id;
        }
    }

    public IReactiveCommand SaveCmd { get; }

    private IReactiveCommand CreateSaveCmd()
    {
        return ReactiveCommand.Create(
                    async () =>
                        {
                            var dialog = MessageBoxManager.GetMessageBoxStandard(LocalizationService.Default["Save"],
                                                                                    LocalizationService.Default["Save_Text"],
                                                                                    ButtonEnum.YesNo, Icon.Question);
                            var result = await dialog.ShowAsync();
                            if (result == ButtonResult.No)
                            {
                                return;
                            }

                            _candidate.CandidateResources = CandidateResources.CandidateResources.Select(x => x.ToCandidateResource()).ToList();
                            _candidate.CandidateSkills = CandidateSkills.Skills.Select(x => x.ToCandidateSkill(_candidate)).ToList();
                            string message;
                            int id;
                            bool success;

                            if (_candidate.Id == 0)
                            {
                                success = _candidateService.Create(_candidate, out id, out message);
                            }
                            else
                            {
                                success = _candidateService.Update(_candidate, out message);
                                id = _candidate.Id;
                            }

                            if (success)
                            {
                                _candidate = _candidateService.Get(id);
                                LoadCandidate();                                
                            }
                            else
                            {
                                dialog = MessageBoxManager.GetMessageBoxStandard(LocalizationService.Default["Save"],
                                                                                    message, ButtonEnum.Ok, Icon.Error);
                                await dialog.ShowAsync();
                            }

                        }, this.WhenAnyValue(x => x.IsValid, v => v == true)
                    );
    }
    public IReactiveCommand CancelCmd { get; }

    private IReactiveCommand CreateCancelCmd()
    {
        return ReactiveCommand.Create(
                    async () =>
                        {
                            var dialog = MessageBoxManager.GetMessageBoxStandard(LocalizationService.Default["Cancel"],
                                                                                    LocalizationService.Default["Cancel_Text"],
                                                                                    ButtonEnum.YesNo, Icon.Question);
                            var result = await dialog.ShowAsync();
                            if (result == ButtonResult.No)
                            {
                                return;
                            }
                            _candidate = _candidate.Id == 0 ? NewCandidate : _candidateService.Get(_candidate.Id);
                            LoadCandidate();
                        }
                    );
    }

    public IReactiveCommand DeleteCmd { get; }

    private IReactiveCommand CreateDeleteCmd()
    {
        return ReactiveCommand.Create(
                    async () =>
                        {
                            var dialog = MessageBoxManager.GetMessageBoxStandard(LocalizationService.Default["Delete"],
                                                                                    LocalizationService.Default["DeleteCandidate_Text"],
                                                                                    ButtonEnum.YesNo, Icon.Question);
                            var result = await dialog.ShowAsync();
                            if (result == ButtonResult.No)
                            {
                                return;
                            }

                            string message;
                            if (_candidateService.Delete(CandidateId, out message))
                            {
                                this.Factory.CloseDockable(this);
                            }
                            else
                            {
                                dialog = MessageBoxManager.GetMessageBoxStandard(LocalizationService.Default["Delete"],
                                                                                    message, ButtonEnum.Ok, Icon.Error);
                                await dialog.ShowAsync();
                            }

                        }, this.WhenAnyValue(x => x.CandidateId, y => y != 0)
                    );
    }

    #region FirstName
    private string _firstName;
    [Required]
    [StringLength(250, MinimumLength = 2)]
    public string FirstName
    {
        get => _firstName;
        set
        {
            _candidate.FirstName = value;
            this.RaiseAndSetIfChanged(ref _firstName, value);
            this.RaisePropertyChanged(nameof(IsValid));
        }
    }
    #endregion

    #region LastName
    private string _lastName;
    [Required]
    [StringLength(250, MinimumLength = 2)]
    public string LastName
    {
        get => _lastName;
        set
        {
            _candidate.LastName = value;
            this.RaiseAndSetIfChanged(ref _lastName, value);
            this.RaisePropertyChanged(nameof(IsValid));
        }
    }
    #endregion

    #region BirthDate
    private DateTime? _birthDate;

    [Required]
    [BirthDate]
    public DateTime? BirthDate
    {
        get => _birthDate;
        set
        {
            if (value.HasValue)
            {
                _candidate.BirthDate = value!.Value;
            }
            this.RaiseAndSetIfChanged(ref _birthDate, value);
            this.RaisePropertyChanged(nameof(this.Age));
            this.RaisePropertyChanged(nameof(IsValid));
        }
    }

    public DateTime MaxDateEnd
    {
        get => DateTime.Today.AddYears(-18);
    }
    #endregion

    public string Age
    {
        get => _birthDate.HasValue ? $"{LocalizationService.Default["Age"]} {_birthDate.Value.GetAge()}" : string.Empty;
    }

    #region Location
    private LocationViewModel _location;
    public LocationViewModel Location
    {
        get => _location;
        set
        {
            _candidate.Location = value.Location;
            this.RaiseAndSetIfChanged(ref _location, value);
            this.RaisePropertyChanged(nameof(IsValid));
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
            _candidate.Enabled = value;
            this.RaiseAndSetIfChanged(ref _enabled, value);
            this.RaisePropertyChanged(nameof(IsValid));
        }
    }
    #endregion   

    private CandidateResourcesViewModel _candidateResources;
    public CandidateResourcesViewModel CandidateResources
    {
        get => _candidateResources;
        set => this.RaiseAndSetIfChanged(ref _candidateResources, value);
    }

    private SkillsViewModel _candidateSkills;
    public SkillsViewModel CandidateSkills
    {
        get => _candidateSkills;
        set => this.RaiseAndSetIfChanged(ref _candidateSkills, value);
    }
}
