using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.PropertyGrid.Services;
using Dock.Model.ReactiveUI.Controls;
using DynamicData;
using DynamicData.Binding;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.DataAccess;
using MyCandidate.MVVM.DataAnnotations;
using MyCandidate.MVVM.Extensions;
using MyCandidate.MVVM.Services;
using MyCandidate.MVVM.ViewModels.Tools;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Vacancies;

public class VacancyViewModel : Document
{
    private readonly IVacancyService _vacancyService;
    private readonly IDictionariesDataAccess _dictionariesData;
    private readonly IDataAccess<Company> _companiesData;
    private readonly IDataAccess<Office> _officesData;
    private readonly IProperties _properties;
    private Vacancy _vacancy;
    private bool _initialSet = false;

    public VacancyViewModel(IVacancyService vacancyService, IDictionariesDataAccess dictionariesData, IDataAccess<Company> companies, IDataAccess<Office> officies, IProperties properties)
    {
        _vacancyService = vacancyService;
        _dictionariesData = dictionariesData;
        _companiesData = companies;
        _officesData = officies;
        _properties = properties;

        OfficesSource = new ObservableCollectionExtended<Office>(_officesData.ItemsList.Where(x => x.Enabled == true));
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
    }

    public VacancyViewModel(IVacancyService vacancyService, IDictionariesDataAccess dictionariesData, IDataAccess<Company> companies, IDataAccess<Office> officies, IProperties properties, int vacancyId)
    {
        _vacancyService = vacancyService;
        _dictionariesData = dictionariesData;
        _companiesData = companies;
        _officesData = officies;
        _properties = properties;

        OfficesSource = new ObservableCollectionExtended<Office>(_officesData.ItemsList.Where(x => x.Enabled == true));
        OfficesSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter)
            .Bind(out _offices)
            .Subscribe();

        _vacancy = _vacancyService.Get(vacancyId);
        LoadVacancy();
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = CreateCancelCmd();
        SaveCmd = CreateSaveCmd();
        DeleteCmd = CreateDeleteCmd();
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

        VacancyResources = new VacancyResourcesViewModel(_vacancy, _properties);
        VacancyResources.WhenAnyValue(x => x.IsValid).Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); });
        VacancySkills = new VacancySkillsViewModel(_vacancy, _properties);
        VacancySkills.WhenAnyValue(x => x.IsValid).Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); });        
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
                            _vacancy.VacancySkills = VacancySkills.VacancySkills.ToList();
                            string message;
                            int id;
                            bool success;

                            if (_vacancy.Id == 0)
                            {
                                success = _vacancyService.Create(_vacancy, out id, out message);
                            }
                            else
                            {
                                success = _vacancyService.Update(_vacancy, out message);
                                id = _vacancy.Id;
                            }

                            if (success)
                            {
                                _vacancy = _vacancyService.Get(id);
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
                            _vacancy = _vacancy.Id == 0 ? NewVacancy : _vacancyService.Get(_vacancy.Id);
                            LoadVacancy();
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
                                                                                    LocalizationService.Default["DeleteVacancy_Text"],
                                                                                    ButtonEnum.YesNo, Icon.Question);
                            var result = await dialog.ShowAsync();
                            if (result == ButtonResult.No)
                            {
                                return;
                            }

                            string message;
                            if (_vacancyService.Delete(VacancyId, out message))
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
                _vacancyStatuses = _dictionariesData.GetVacancyStatuses();
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
                _companies = _companiesData.ItemsList.Where(x => x.Enabled == Enabled);
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
            if(!_initialSet)
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
            if(value != null)
            {
                _vacancy.Office = value;
                _vacancy.OfficeId = value.Id;                
            }            

            this.RaiseAndSetIfChanged(ref _selectedOffice, value);
        }
    }
    #endregion

    private VacancyResourcesViewModel _vacancyResources;
    public VacancyResourcesViewModel VacancyResources
    {
        get => _vacancyResources;
        set => this.RaiseAndSetIfChanged(ref _vacancyResources, value);
    }  

    private VacancySkillsViewModel _vacancySkills;
    public VacancySkillsViewModel VacancySkills
    {
        get => _vacancySkills;
        set => this.RaiseAndSetIfChanged(ref _vacancySkills, value);
    }      
}
