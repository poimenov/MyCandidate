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
    private readonly IAppServiceProvider _provider;
    private Candidate _candidate;

    public CandidateViewModel(IAppServiceProvider appServiceProvider)
    {
        _provider = appServiceProvider;
        _candidate = NewCandidate;
        LoadCandidate();
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = CreateCancelCmd();
        SaveCmd = CreateSaveCmd();
        DeleteCmd = CreateDeleteCmd();
        SearchCmd = CreateSearchCmd();
    }

    public CandidateViewModel(IAppServiceProvider appServiceProvider, int candidateId)
    {
        _provider = appServiceProvider;
        _candidate = _provider.CandidateService.Get(candidateId);
        LoadCandidate();
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = CreateCancelCmd();
        SaveCmd = CreateSaveCmd();
        DeleteCmd = CreateDeleteCmd();
        SearchCmd = CreateSearchCmd();
    }

    private Candidate NewCandidate
    {
        get
        {
            var defaultCity = _provider.CityService.ItemsList.First();
            return new Candidate
            {
                Id = 0,
                FirstName = string.Empty,
                LastName = string.Empty,
                Enabled = true,
                Location = new Location
                {
                    Enabled = true,
                    CityId = defaultCity.Id,
                    City = defaultCity
                },
                CandidateResources = new List<CandidateResource>(),
                CandidateSkills = new List<CandidateSkill>()
            };
        }
    }

    private void LoadCandidate()
    {
        if (_candidate.Id == 0)
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
        CandidateTitle = _candidate.Title;
        Enabled = _candidate.Enabled;
        Location = new LocationViewModel(_provider.CountryService.ItemsList.Where(x => x.Enabled == true), _provider.CityService.ItemsList.Where(x => x.Enabled == true))
        {
            Location = _candidate.Location
        };

        Resources = new ResourcesViewModel(_candidate, _provider.Properties);
        Resources.WhenAnyValue(x => x.IsValid).Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); });
        CandidateSkills = new SkillsViewModel(_candidate.CandidateSkills.Select(x => new SkillModel(x.Id, x.Skill, x.Seniority)), _provider.Properties);
        CandidateSkills.WhenAnyValue(x => x.IsValid).Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); });
        CandidatesOnVacancy = new CandidateOnVacancyViewModel(this, _provider);
        Comments = new CommentsViewModel(this, _provider);
        Comments.WhenAnyValue(x => x.IsValid).Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); });
        this.RaisePropertyChanged(nameof(CandidateId));
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        if (_candidate.Id == 0)
        {
            Title = LocalizationService.Default["New_Candidate"];
        }
    }

    public Candidate Candidate => _candidate;
    public int CandidateId => _candidate.Id;

    public bool IsValid
    {
        get
        {
            var retVal = Validator.TryValidateObject(this, new ValidationContext(this), null, true)
                && Resources.IsValid
                && CandidateSkills.IsValid
                && Comments.IsValid;
            return retVal;
        }
    }

    #region Commands
    public IReactiveCommand SaveCmd { get; }

    private IReactiveCommand CreateSaveCmd()
    {
        return ReactiveCommand.Create(
            async () =>
                {
                    var dialog = this.GetMessageBox(LocalizationService.Default["Save"],
                                                        LocalizationService.Default["Save_Text"],
                                                        ButtonEnum.YesNo, Icon.Question);
                    var result = await dialog.ShowAsync();
                    if (result == ButtonResult.No)
                    {
                        return;
                    }

                    _candidate.CandidateResources = Resources.Resources.Select(x => x.ToCandidateResource(Candidate)).ToList();
                    _candidate.CandidateSkills = CandidateSkills.Skills.Select(x => x.ToCandidateSkill(_candidate)).ToList();
                    _candidate.CandidateOnVacancies = CandidatesOnVacancy.GetCandidateOnVacancies();
                    string message;
                    int id;
                    bool success;

                    if (_candidate.Id == 0)
                    {
                        success = _provider.CandidateService.Create(_candidate, out id, out message);
                    }
                    else
                    {
                        success = _provider.CandidateService.Update(_candidate, out message);
                        id = _candidate.Id;
                    }

                    if (success)
                    {
                        _candidate = _provider.CandidateService.Get(id);
                        LoadCandidate();
                    }
                    else
                    {
                        dialog = this.GetMessageBox(LocalizationService.Default["Save"],
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
                    var dialog = this.GetMessageBox(LocalizationService.Default["Cancel"],
                                                        LocalizationService.Default["Cancel_Text"],
                                                        ButtonEnum.YesNo, Icon.Question);
                    var result = await dialog.ShowAsync();
                    if (result == ButtonResult.No)
                    {
                        return;
                    }
                    _candidate = _candidate.Id == 0 ? NewCandidate : _provider.CandidateService.Get(_candidate.Id);
                    LoadCandidate();
                }
            );
    }

    public IReactiveCommand SearchCmd { get; }

    private IReactiveCommand CreateSearchCmd()
    {
        return ReactiveCommand.Create(
            async () =>
            {
                _provider.OpenDock(_provider.GetVacancySearchViewModel(this));
            }, this.WhenAnyValue(x => x.CandidateId, y => y != 0));
    }

    public IReactiveCommand DeleteCmd { get; }

    private IReactiveCommand CreateDeleteCmd()
    {
        return ReactiveCommand.Create(
            async () =>
                {
                    var dialog = this.GetMessageBox(LocalizationService.Default["Delete"],
                                                        LocalizationService.Default["DeleteCandidate_Text"],
                                                        ButtonEnum.YesNo, Icon.Question);
                    var result = await dialog.ShowAsync();
                    if (result == ButtonResult.No)
                    {
                        return;
                    }

                    string message;
                    if (_provider.CandidateService.Delete(CandidateId, out message))
                    {
                        _provider.CloseDock(this);
                    }
                    else
                    {
                        dialog = this.GetMessageBox(LocalizationService.Default["Delete"],
                                                        message, ButtonEnum.Ok, Icon.Error);
                        await dialog.ShowAsync();
                    }

                }, this.WhenAnyValue(x => x.CandidateId, y => y != 0)
            );
    }
    #endregion

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

    #region CandidateTitle
    private string? _candidateTitle;
    [StringLength(250, MinimumLength = 2)]
    public string? CandidateTitle
    {
        get => _candidateTitle;
        set
        {
            _candidate.Title = value;
            this.RaiseAndSetIfChanged(ref _candidateTitle, value);
        }
    }
    #endregion    

    #region BirthDate
    private DateTime? _birthDate;

    [BirthDate]
    public DateTime? BirthDate
    {
        get => _birthDate;
        set
        {
            _candidate.BirthDate = value;
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

    #region Resources
    private ResourcesViewModel _resources;
    public ResourcesViewModel Resources
    {
        get => _resources;
        set => this.RaiseAndSetIfChanged(ref _resources, value);
    }
    #endregion

    #region CandidateSkills
    private SkillsViewModel _candidateSkills;
    public SkillsViewModel CandidateSkills
    {
        get => _candidateSkills;
        set => this.RaiseAndSetIfChanged(ref _candidateSkills, value);
    }
    #endregion

    #region CandidatesOnVacancy
    private CandidateOnVacancyViewModel _candidatesOnVacancy;
    public CandidateOnVacancyViewModel CandidatesOnVacancy
    {
        get => _candidatesOnVacancy;
        set => this.RaiseAndSetIfChanged(ref _candidatesOnVacancy, value);
    }
    #endregion

    #region Comments
    private CommentsViewModel _comments;
    public CommentsViewModel Comments
    {
        get => _comments;
        set => this.RaiseAndSetIfChanged(ref _comments, value);
    }
    #endregion     
}
