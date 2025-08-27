using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using Avalonia;
using Avalonia.PropertyGrid.Services;
using DynamicData;
using DynamicData.Binding;
using MsBox.Avalonia.Enums;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.DataTemplates;
using MyCandidate.MVVM.Extensions;
using MyCandidate.MVVM.Models;
using MyCandidate.MVVM.Services;
using MyCandidate.MVVM.ViewModels.Shared;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Vacancies;

public class VacancyViewModel : DocumentBase
{
    private readonly IAppServiceProvider _provider;
    private Vacancy? _vacancy;
    private bool _initialSet = false;

    public VacancyViewModel(IAppServiceProvider appServiceProvider)
    {
        _provider = appServiceProvider;

        OfficesSource = new ObservableCollectionExtended<Office>();
        OfficesSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter!)
            .Bind(out _offices)
            .Subscribe()
            .DisposeWith(Disposables);

        LoadDataCmd = ReactiveCommand.CreateFromTask<int?>(LoadVacancy).DisposeWith(Disposables);
        LoadDataCmd.Execute(null).Subscribe().DisposeWith(Disposables);

        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = CreateCancelCmd();
        SaveCmd = CreateSaveCmd();
        DeleteCmd = CreateDeleteCmd();
        SearchCmd = CreateSearchCmd();
        ExportCmd = CreateExportCmd();
    }

    public VacancyViewModel(IAppServiceProvider appServiceProvider, int vacancyId)
    {
        _provider = appServiceProvider;

        OfficesSource = new ObservableCollectionExtended<Office>();
        OfficesSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter!)
            .Bind(out _offices)
            .Subscribe()
            .DisposeWith(Disposables);

        LoadDataCmd = ReactiveCommand.CreateFromTask<int?>(LoadVacancy).DisposeWith(Disposables);
        LoadDataCmd.Execute(vacancyId).Subscribe().DisposeWith(Disposables);

        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = CreateCancelCmd();
        SaveCmd = CreateSaveCmd();
        DeleteCmd = CreateDeleteCmd();
        SearchCmd = CreateSearchCmd();
        ExportCmd = CreateExportCmd();
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        Title = (_vacancy == null || _vacancy.Id == 0) ? LocalizationService.Default["New_Vacancy"] : _vacancy?.Name ?? string.Empty;
    }

    private Vacancy NewVacancy
    {
        get
        {
            var vacancyStatus = VacancyStatuses.First(x => x.Name == VacancyStatusNames.New) ?? throw new InvalidOperationException("VacancyStatuses is null");
            var office = OfficesSource.First() ?? throw new InvalidOperationException("OfficesSource is null");
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

    protected override void OnClosed()
    {
        LocalizationService.Default.OnCultureChanged -= CultureChanged;
        base.OnClosed();
    }

    private async Task LoadVacancy(int? id)
    {
        _vacancyStatuses = await _provider.DictionariesDataAccess.GetVacancyStatusesAsync();
        var companies = await _provider.CompanyService.GetItemsListAsync();
        _companies = companies.Where(x => x.Enabled == true).ToArray();

        var offices = await _provider.OfficeService.GetItemsListAsync();
        RxApp.MainThreadScheduler.Schedule(() =>
        {
            OfficesSource.Clear();
            OfficesSource.AddRange(offices.Where(x => x.Enabled == true));
        }).DisposeWith(Disposables);

        if (id != null && id > 0)
        {
            _vacancy = await _provider.VacancyService.GetAsync(id.Value) ?? NewVacancy;
        }
        else
        {
            _vacancy = NewVacancy;
        }

        Title = (_vacancy.Id == 0) ? LocalizationService.Default["New_Vacancy"] : _vacancy.Name;
        Name = _vacancy.Name;
        Description = _vacancy.Description;
        Enabled = _vacancy.Enabled;
        SelectedVacancyStatus = VacancyStatuses.First(x => x.Id == _vacancy.VacancyStatusId);
        _initialSet = true;
        SelectedCompany = Companies.First(x => x.Id == _vacancy.Office!.CompanyId);
        _initialSet = false;
        SelectedOffice = Offices.First(x => x.Id == _vacancy.OfficeId);

        Resources = new ResourcesViewModel(_vacancy, _provider.Properties!).DisposeWith(Disposables);
        Resources.WhenAnyValue(x => x.IsValid)
            .Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); })
            .DisposeWith(Disposables);

        VacancySkills = new SkillsViewModel(_vacancy.VacancySkills.Select(x => new SkillModel(x.Id, x.Skill!, x.Seniority!)),
            _provider.Properties!).DisposeWith(Disposables);
        VacancySkills.WhenAnyValue(x => x.IsValid)
            .Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); })
            .DisposeWith(Disposables);

        CandidatesOnVacancy = new CandidateOnVacancyViewModel(this, _provider).DisposeWith(Disposables);

        Comments = new CommentsViewModel(this, _provider).DisposeWith(Disposables);
        Comments.WhenAnyValue(x => x.IsValid)
            .Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); })
            .DisposeWith(Disposables);

        this.RaisePropertyChanged(nameof(VacancyId));
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

                    if (Vacancy != null)
                    {
                        if (Resources?.Resources != null && VacancySkills?.Skills != null && CandidatesOnVacancy != null)
                        {
                            Vacancy.VacancyResources = Resources.Resources.Select(x => x.ToVacancyResource(Vacancy)).ToList();
                            Vacancy.VacancySkills = VacancySkills.Skills.Select(x => x.ToVacancySkill(Vacancy)).ToList();
                            Vacancy.CandidateOnVacancies = CandidatesOnVacancy.GetCandidateOnVacancies();
                        }

                        string message = string.Empty;
                        int id;
                        bool success;

                        if (Vacancy.Id == 0)
                        {
                            var createResult = await _provider.VacancyService.CreateAsync(Vacancy);
                            id = createResult.Result;
                            message = createResult.Message ?? string.Empty;
                            success = createResult.Success;
                        }
                        else
                        {
                            var updateResult = await _provider.VacancyService.UpdateAsync(Vacancy);
                            success = updateResult.Success;
                            id = Vacancy.Id;
                        }

                        if (success)
                        {
                            await LoadVacancy(id);
                        }
                        else
                        {
                            dialog = this.GetMessageBox(LocalizationService.Default["Save"],
                                                            message, ButtonEnum.Ok, Icon.Error);
                            await dialog.ShowAsync();
                        }
                    }

                }, this.WhenAnyValue(x => x.IsValid, v => v == true)
            ).DisposeWith(Disposables);
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

                    if (Vacancy != null && Vacancy.Id == 0)
                    {
                        _vacancy = NewVacancy;
                    }

                    await LoadVacancy(Vacancy?.Id);
                }
            ).DisposeWith(Disposables);
    }

    public IReactiveCommand SearchCmd { get; }

    private IReactiveCommand CreateSearchCmd()
    {
        return ReactiveCommand.Create(
            () =>
            {
                _provider.OpenDock(_provider.GetCandidateSearchViewModel(this));
            }, this.WhenAnyValue(x => x.VacancyId, y => y != 0)).DisposeWith(Disposables);
    }

    public IReactiveCommand DeleteCmd { get; }

    private IReactiveCommand CreateDeleteCmd()
    {
        return ReactiveCommand.Create(
            async () =>
                {
                    var dialog = this.GetMessageBox(LocalizationService.Default["Delete"],
                                                        LocalizationService.Default["DeleteVacancy_Text"],
                                                        ButtonEnum.YesNo, Icon.Question);
                    var result = await dialog.ShowAsync();
                    if (result == ButtonResult.No)
                    {
                        return;
                    }

                    var deleteResult = await _provider.VacancyService.DeleteAsync(VacancyId);
                    string message = deleteResult.Message ?? string.Empty;
                    if (deleteResult.Success)
                    {
                        _provider.CloseDock(this);
                    }
                    else
                    {
                        dialog = this.GetMessageBox(LocalizationService.Default["Delete"],
                                                        message, ButtonEnum.Ok, Icon.Error);
                        await dialog.ShowAsync();
                    }

                }, this.WhenAnyValue(x => x.VacancyId, y => y != 0)
            ).DisposeWith(Disposables);
    }

    public IReactiveCommand ExportCmd { get; }
    public ReactiveCommand<int?, Unit> LoadDataCmd { get; }

    private IReactiveCommand CreateExportCmd()
    {
        return ReactiveCommand.Create<string>(
            async (format) =>
                {
                    var exportFolder = Path.Combine(AppSettings.AppDataPath, "export");
                    if (!Directory.Exists(exportFolder))
                    {
                        Directory.CreateDirectory(exportFolder);
                    }
                    var entityName = "vacancy";
                    var doc = await _provider.VacancyService.GetXmlAsync(VacancyId);
                    var path = Path.Combine(exportFolder, $"{entityName}.{VacancyId}.xml");
                    switch (format)
                    {
                        case "html":
                            var xslt = XsltExtObject.GetTransform(entityName, format);
                            var args = new XsltArgumentList();
                            args.AddExtensionObject("urn:ExtObj", new XsltExtObject());
                            path = Path.Combine(exportFolder, $"{entityName}.{VacancyId}.html");
                            await SaveDocumentAsync(doc, path, xslt, args);
                            break;
                        default:
                            await SaveDocumentAsync(doc, path, null, null);
                            break;
                    }
                    DataTemplateProvider.Open(path);
                }, this.WhenAnyValue(x => x.VacancyId, y => y != 0)
            ).DisposeWith(Disposables);
    }
    #endregion

    private async Task SaveDocumentAsync(XmlDocument xmlDoc, string filePath, XslCompiledTransform? xslt, XsltArgumentList? args)
    {
        try
        {
            await xmlDoc.SaveAsync(filePath, xslt, args);
        }
        catch (Exception ex)
        {
            var dialog = this.GetMessageBox(LocalizationService.Default["Save"],
                                            ex.Message, ButtonEnum.Ok, Icon.Error);
            await dialog.ShowAsync();
        }
    }

    public Vacancy? Vacancy => _vacancy;
    public int VacancyId => (_vacancy != null) ? _vacancy.Id : 0;

    public bool IsValid
    {
        get
        {
            var retVal = Validator.TryValidateObject(this, new ValidationContext(this), null, true)
            && Resources?.IsValid == true
            && VacancySkills?.IsValid == true
            && Comments?.IsValid == true;
            return retVal;
        }
    }

    #region Name
    private string? _name;
    [Required]
    [StringLength(250, MinimumLength = 3)]
    public string? Name
    {
        get => _name;
        set
        {
            if (!string.IsNullOrEmpty(value) && Vacancy != null)
            {
                Vacancy.Name = value;
                this.RaiseAndSetIfChanged(ref _name, value);
                this.RaisePropertyChanged(nameof(IsValid));
            }
        }
    }
    #endregion

    #region Description
    private string? _description;
    [Required]
    public string? Description
    {
        get => _description;
        set
        {
            if (value != null && Vacancy != null)
            {
                Vacancy.Description = value;
                this.RaiseAndSetIfChanged(ref _description, value);
                this.RaisePropertyChanged(nameof(IsValid));
            }
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
            if (Vacancy != null)
            {
                Vacancy.Enabled = value;
                this.RaiseAndSetIfChanged(ref _enabled, value);
                this.RaisePropertyChanged(nameof(IsValid));
            }
        }
    }
    #endregion

    #region VacancyStatus
    private IEnumerable<VacancyStatus> _vacancyStatuses = Enumerable.Empty<VacancyStatus>();
    public IEnumerable<VacancyStatus> VacancyStatuses => _vacancyStatuses;

    private VacancyStatus? _selectedVacancyStatus;
    public VacancyStatus? SelectedVacancyStatus
    {
        get => _selectedVacancyStatus;
        set
        {
            if (value != null && Vacancy != null)
            {
                Vacancy.VacancyStatus = value;
                Vacancy.VacancyStatusId = value.Id;
                this.RaiseAndSetIfChanged(ref _selectedVacancyStatus, value);
            }
        }
    }
    #endregion

    #region Company
    private IEnumerable<Company> _companies = Enumerable.Empty<Company>();
    public IEnumerable<Company> Companies => _companies;

    private Company? _selectedCompany;
    public Company? SelectedCompany
    {
        get => _selectedCompany;
        set
        {
            if (value != null)
            {
                this.RaiseAndSetIfChanged(ref _selectedCompany, value);
                if (!_initialSet)
                {
                    SelectedOffice = Offices.First();
                }
            }
        }
    }
    #endregion

    #region Office
    public ObservableCollectionExtended<Office> OfficesSource;
    private readonly ReadOnlyObservableCollection<Office> _offices;
    public ReadOnlyObservableCollection<Office> Offices => _offices;

    private Office? _selectedOffice;
    public Office? SelectedOffice
    {
        get => _selectedOffice;
        set
        {
            if (value != null && Vacancy != null)
            {
                Vacancy.Office = value;
                Vacancy.OfficeId = value.Id;
            }

            this.RaiseAndSetIfChanged(ref _selectedOffice, value);
        }
    }
    #endregion

    #region Resources
    private ResourcesViewModel? _resources;
    public ResourcesViewModel? Resources
    {
        get => _resources;
        set => this.RaiseAndSetIfChanged(ref _resources, value);
    }
    #endregion

    #region VacancySkills
    private SkillsViewModel? _vacancySkills;
    public SkillsViewModel? VacancySkills
    {
        get => _vacancySkills;
        set => this.RaiseAndSetIfChanged(ref _vacancySkills, value);
    }
    #endregion

    #region CandidatesOnVacancy
    private CandidateOnVacancyViewModel? _candidatesOnVacancy;
    public CandidateOnVacancyViewModel? CandidatesOnVacancy
    {
        get => _candidatesOnVacancy;
        set => this.RaiseAndSetIfChanged(ref _candidatesOnVacancy, value);
    }
    #endregion

    #region Comments
    private CommentsViewModel? _comments;
    public CommentsViewModel? Comments
    {
        get => _comments;
        set => this.RaiseAndSetIfChanged(ref _comments, value);
    }
    #endregion    
}
