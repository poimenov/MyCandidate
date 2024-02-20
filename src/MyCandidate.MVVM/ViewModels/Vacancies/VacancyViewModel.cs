using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.PropertyGrid.Services;
using Dock.Model.Controls;
using Dock.Model.ReactiveUI.Controls;
using DynamicData;
using DynamicData.Binding;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.Models;
using MyCandidate.MVVM.Services;
using MyCandidate.MVVM.ViewModels.Candidates;
using MyCandidate.MVVM.ViewModels.Shared;
using MyCandidate.MVVM.ViewModels.Tools;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Vacancies;

public class VacancyViewModel : Document
{
    private readonly IAppServiceProvider _provider;
    private Vacancy _vacancy;
    private bool _initialSet = false;

    public VacancyViewModel(IAppServiceProvider appServiceProvider)
    {
        _provider = appServiceProvider;
        OfficesSource = new ObservableCollectionExtended<Office>(_provider.OfficeService.ItemsList.Where(x => x.Enabled == true));
        OfficesSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter)
            .Bind(out _offices)
            .Subscribe();

        _vacancy = NewVacancy;
        LoadVacancy();
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = CreateCancelCmd();
        SaveCmd = CreateSaveCmd();
        DeleteCmd = CreateDeleteCmd();
        SearchCmd = CreateSearchCmd();
    }

    public VacancyViewModel(IAppServiceProvider appServiceProvider, int vacancyId)
    {
        _provider = appServiceProvider;

        OfficesSource = new ObservableCollectionExtended<Office>(_provider.OfficeService.ItemsList.Where(x => x.Enabled == true));
        OfficesSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter)
            .Bind(out _offices)
            .Subscribe();

        _vacancy = _provider.VacancyService.Get(vacancyId);
        LoadVacancy();
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = CreateCancelCmd();
        SaveCmd = CreateSaveCmd();
        DeleteCmd = CreateDeleteCmd();
        SearchCmd = CreateSearchCmd();
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        Title = LocalizationService.Default["New_Vacancy"];
    }

    private Vacancy NewVacancy
    {
        get
        {
            var vacancyStatus = VacancyStatuses.First(x => x.Name == VacancyStatusNames.New);
            var office = OfficesSource.First();
            return new Vacancy
            {
                Id = 0,
                Name = string.Empty,
                Description = string.Empty,
                Enabled = true,
                VacancyStatusId = vacancyStatus.Id,
                VacancyStatus = vacancyStatus,
                Office = office,
                OfficeId = office.Id,
                VacancyResources = new List<VacancyResource>(),
                VacancySkills = new List<VacancySkill>()
            };
        }
    }

    public Vacancy Vacancy => _vacancy;

    #region Filter
    private IObservable<Func<Office, bool>>? Filter =>
        this.WhenAnyValue(x => x.SelectedCompany)
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

    private void LoadVacancy()
    {
        Title = (_vacancy.Id == 0) ? LocalizationService.Default["New_Vacancy"] : _vacancy.Name;
        Name = _vacancy.Name;
        Description = _vacancy.Description;
        Enabled = _vacancy.Enabled;
        SelectedVacancyStatus = VacancyStatuses.First(x => x.Id == _vacancy.VacancyStatusId);
        _initialSet = true;
        SelectedCompany = Companies.First(x => x.Id == _vacancy.Office.CompanyId);
        _initialSet = false;
        SelectedOffice = Offices.First(x => x.Id == _vacancy.OfficeId);

        VacancyResources = new VacancyResourcesViewModel(_vacancy, _provider.Properties);
        VacancyResources.WhenAnyValue(x => x.IsValid).Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); });
        VacancySkills = new SkillsViewModel(_vacancy.VacancySkills.Select(x => new SkillModel(x.Id, x.Skill, x.Seniority)), _provider.Properties);
        VacancySkills.WhenAnyValue(x => x.IsValid).Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); });
        CandidatesOnVacancy = new CandidateOnVacancyViewModel(_vacancy, _provider);
        this.RaisePropertyChanged(nameof(VacancyId));
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

                    _vacancy.VacancyResources = VacancyResources.VacancyResources.Select(x => x.ToVacancyResource()).ToList();
                    _vacancy.VacancySkills = VacancySkills.Skills.Select(x => x.ToVacancySkill(_vacancy)).ToList();
                    string message;
                    int id;
                    bool success;

                    if (_vacancy.Id == 0)
                    {
                        success = _provider.VacancyService.Create(_vacancy, out id, out message);
                    }
                    else
                    {
                        success = _provider.VacancyService.Update(_vacancy, out message);
                        id = _vacancy.Id;
                    }

                    if (success)
                    {
                        _vacancy = _provider.VacancyService.Get(id);
                        LoadVacancy();
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
                    _vacancy = _vacancy.Id == 0 ? NewVacancy : _provider.VacancyService.Get(_vacancy.Id);
                    LoadVacancy();
                }
            );
    }

    public IReactiveCommand SearchCmd { get; }

    private IReactiveCommand CreateSearchCmd()
    {
        return ReactiveCommand.Create(
            async () =>
            {
                _provider.OpenDock(_provider.GetCandidateSearchViewModel(this));
            }, this.WhenAnyValue(x => x.VacancyId, y => y != 0));
    }

    public IReactiveCommand DeleteCmd { get; }

    private IReactiveCommand CreateDeleteCmd()
    {
        return ReactiveCommand.Create(
            async () =>
                {
                    var dialog = MessageBoxManager.GetMessageBoxStandard(LocalizationService.Default["Delete"],
                                                                            LocalizationService.Default["DeleteVacancy_Text"],
                                                                            ButtonEnum.YesNo, Icon.Question);
                    var result = await dialog.ShowAsync();
                    if (result == ButtonResult.No)
                    {
                        return;
                    }

                    string message;
                    if (_provider.VacancyService.Delete(VacancyId, out message))
                    {
                        this.Factory.CloseDockable(this);
                    }
                    else
                    {
                        dialog = MessageBoxManager.GetMessageBoxStandard(LocalizationService.Default["Delete"],
                                                                            message, ButtonEnum.Ok, Icon.Error);
                        await dialog.ShowAsync();
                    }

                }, this.WhenAnyValue(x => x.VacancyId, y => y != 0)
            );
    }

    public int VacancyId
    {
        get
        {
            return _vacancy.Id;
        }
    }

    public bool IsValid
    {
        get
        {
            var retVal = Validator.TryValidateObject(this, new ValidationContext(this), null, true)
            && VacancyResources.IsValid
            && VacancySkills.IsValid;
            return retVal;
        }
    }

    #region Name
    private string _name;
    [Required]
    [StringLength(250, MinimumLength = 3)]
    public string Name
    {
        get => _name;
        set
        {
            _vacancy.Name = value;
            this.RaiseAndSetIfChanged(ref _name, value);
            this.RaisePropertyChanged(nameof(IsValid));
        }
    }
    #endregion

    #region Description
    private string _description;
    public string Description
    {
        get => _description;
        set
        {
            _vacancy.Description = value;
            this.RaiseAndSetIfChanged(ref _description, value);
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
            _vacancy.Enabled = value;
            this.RaiseAndSetIfChanged(ref _enabled, value);
            this.RaisePropertyChanged(nameof(IsValid));
        }
    }
    #endregion

    #region VacancyStatus
    private IEnumerable<VacancyStatus> _vacancyStatuses;
    public IEnumerable<VacancyStatus> VacancyStatuses
    {
        get
        {
            if (_vacancyStatuses == null)
            {
                _vacancyStatuses = _provider.DictionariesDataAccess.GetVacancyStatuses();
            }

            return _vacancyStatuses;
        }
    }

    private VacancyStatus _selectedVacancyStatus;
    public VacancyStatus SelectedVacancyStatus
    {
        get => _selectedVacancyStatus;
        set
        {
            _vacancy.VacancyStatus = value;
            _vacancy.VacancyStatusId = value.Id;
            this.RaiseAndSetIfChanged(ref _selectedVacancyStatus, value);
        }
    }
    #endregion

    #region Company
    private IEnumerable<Company> _companies;
    public IEnumerable<Company> Companies
    {
        get
        {
            if (_companies == null)
            {
                _companies = _provider.CompanyService.ItemsList.Where(x => x.Enabled == Enabled);
            }

            return _companies;
        }
    }

    private Company _selectedCompany;
    public Company SelectedCompany
    {
        get => _selectedCompany;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedCompany, value);
            if (!_initialSet)
            {
                SelectedOffice = Offices.First();
            }
        }
    }
    #endregion

    #region Office
    public ObservableCollectionExtended<Office> OfficesSource;
    private readonly ReadOnlyObservableCollection<Office> _offices;
    public ReadOnlyObservableCollection<Office> Offices => _offices;

    private Office _selectedOffice;
    public Office SelectedOffice
    {
        get => _selectedOffice;
        set
        {
            if (value != null)
            {
                _vacancy.Office = value;
                _vacancy.OfficeId = value.Id;
            }

            this.RaiseAndSetIfChanged(ref _selectedOffice, value);
        }
    }
    #endregion

    #region VacancyResources
    private VacancyResourcesViewModel _vacancyResources;
    public VacancyResourcesViewModel VacancyResources
    {
        get => _vacancyResources;
        set => this.RaiseAndSetIfChanged(ref _vacancyResources, value);
    }
    #endregion

    #region VacancySkills
    private SkillsViewModel _vacancySkills;
    public SkillsViewModel VacancySkills
    {
        get => _vacancySkills;
        set => this.RaiseAndSetIfChanged(ref _vacancySkills, value);
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
}
