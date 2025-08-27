using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using Avalonia.PropertyGrid.Services;
using MsBox.Avalonia.Enums;
using MyCandidate.Common;
using MyCandidate.MVVM.DataAnnotations;
using MyCandidate.MVVM.DataTemplates;
using MyCandidate.MVVM.Extensions;
using MyCandidate.MVVM.Models;
using MyCandidate.MVVM.Services;
using MyCandidate.MVVM.ViewModels.Shared;
using MyCandidate.MVVM.ViewModels.Tools;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Candidates;

public class CandidateViewModel : DocumentBase
{
    private readonly IAppServiceProvider _provider;
    private Candidate? _candidate;
    private City? _defaultCity;

    public CandidateViewModel(IAppServiceProvider appServiceProvider)
    {
        _provider = appServiceProvider;
        LoadDataCmd = ReactiveCommand.CreateFromTask<int?>(LoadCandidate).DisposeWith(Disposables);
        LoadDataCmd.Execute(null).Subscribe().DisposeWith(Disposables);
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = CreateCancelCmd();
        SaveCmd = CreateSaveCmd();
        DeleteCmd = CreateDeleteCmd();
        SearchCmd = CreateSearchCmd();
        ExportCmd = CreateExportCmd();
    }

    public CandidateViewModel(IAppServiceProvider appServiceProvider, int candidateId)
    {
        _provider = appServiceProvider;
        LoadDataCmd = ReactiveCommand.CreateFromTask<int?>(LoadCandidate).DisposeWith(Disposables);
        LoadDataCmd.Execute(candidateId).Subscribe().DisposeWith(Disposables);
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        CancelCmd = CreateCancelCmd();
        SaveCmd = CreateSaveCmd();
        DeleteCmd = CreateDeleteCmd();
        SearchCmd = CreateSearchCmd();
        ExportCmd = CreateExportCmd();
    }

    private Candidate NewCandidate
    {
        get
        {
            return new Candidate
            {
                Id = 0,
                FirstName = string.Empty,
                LastName = string.Empty,
                Enabled = true,
                Location = new Location
                {
                    Enabled = true,
                    CityId = _defaultCity != null ? _defaultCity.Id : 0,
                    City = _defaultCity
                },
                CandidateResources = new List<CandidateResource>(),
                CandidateSkills = new List<CandidateSkill>()
            };
        }
    }

    private async Task LoadCandidate(int? id)
    {
        var cities = await _provider.CityService.GetItemsListAsync();
        _defaultCity = cities.FirstOrDefault();

        if (id != null && id > 0)
        {
            _candidate = await _provider.CandidateService.GetAsync(id.Value) ?? NewCandidate;
        }
        else
        {
            _candidate = NewCandidate;
        }

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
        var countriesList = await _provider.CountryService.GetItemsListAsync();
        var citiesList = await _provider.CityService.GetItemsListAsync();

        Location = new LocationViewModel(countriesList.Where(x => x.Enabled == true),
            citiesList.Where(x => x.Enabled == true))
        {
            Location = _candidate.Location
        }.DisposeWith(Disposables);

        Resources = new ResourcesViewModel(_candidate, _provider.Properties!).DisposeWith(Disposables);
        Resources.WhenAnyValue(x => x.IsValid)
            .Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); })
            .DisposeWith(Disposables);

        CandidateSkills = new SkillsViewModel(_candidate.CandidateSkills
            .Select(x => new SkillModel(x.Id, x.Skill!, x.Seniority!)), _provider.Properties!)
            .DisposeWith(Disposables);
        CandidateSkills.WhenAnyValue(x => x.IsValid)
            .Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); })
            .DisposeWith(Disposables);

        CandidatesOnVacancy = new CandidateOnVacancyViewModel(this, _provider).DisposeWith(Disposables);

        Comments = new CommentsViewModel(this, _provider).DisposeWith(Disposables);
        Comments.WhenAnyValue(x => x.IsValid)
            .Subscribe((x) => { this.RaisePropertyChanged(nameof(IsValid)); })
            .DisposeWith(Disposables);

        this.RaisePropertyChanged(nameof(CandidateId));
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        if (Candidate?.Id == 0)
        {
            Title = LocalizationService.Default["New_Candidate"];
        }
    }

    protected override void OnClosed()
    {
        LocalizationService.Default.OnCultureChanged -= CultureChanged;
        base.OnClosed();
    }

    public Candidate? Candidate => _candidate;
    public int CandidateId => (_candidate != null) ? _candidate.Id : 0;

    public bool IsValid
    {
        get
        {
            var retVal = Validator.TryValidateObject(this, new ValidationContext(this), null, true)
                && Resources?.IsValid == true
                && CandidateSkills?.IsValid == true
                && Comments?.IsValid == true;
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

                    if (Candidate != null)
                    {
                        Candidate.CandidateResources = Resources?.Resources?.Select(x => x.ToCandidateResource(Candidate))?.ToList() ?? new List<CandidateResource>();
                        Candidate.CandidateSkills = CandidateSkills?.Skills?.Select(x => x.ToCandidateSkill(Candidate))?.ToList() ?? new List<CandidateSkill>();
                        Candidate.CandidateOnVacancies = CandidatesOnVacancy?.GetCandidateOnVacancies() ?? new List<CandidateOnVacancy>();

                        string message = string.Empty;
                        int id;
                        bool success;

                        if (Candidate.Id == 0)
                        {
                            var createResult = await _provider.CandidateService.CreateAsync(Candidate);
                            id = createResult.Result;
                            message = createResult.Message ?? string.Empty;
                            success = createResult.Success;
                        }
                        else
                        {
                            var updateResult = await _provider.CandidateService.UpdateAsync(Candidate);
                            success = updateResult.Success;
                            id = Candidate.Id;
                        }

                        if (success)
                        {
                            await LoadCandidate(id);
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

                    if (Candidate == null || Candidate.Id == 0)
                    {
                        _candidate = NewCandidate;
                    }

                    await LoadCandidate(Candidate?.Id);
                }
            ).DisposeWith(Disposables);
    }

    public IReactiveCommand SearchCmd { get; }

    private IReactiveCommand CreateSearchCmd()
    {
        return ReactiveCommand.Create(
            () =>
            {
                _provider.OpenDock(_provider.GetVacancySearchViewModel(this));
            }, this.WhenAnyValue(x => x.CandidateId, y => y != 0)).DisposeWith(Disposables);
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

                    var deleteResult = await _provider.CandidateService.DeleteAsync(CandidateId);
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

                }, this.WhenAnyValue(x => x.CandidateId, y => y != 0)
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
                    var entityName = "candidate";
                    var doc = await _provider.CandidateService.GetXmlAsync(CandidateId);
                    var path = Path.Combine(exportFolder, $"{entityName}.{CandidateId}.xml");
                    switch (format)
                    {
                        case "html":
                            var xslt = XsltExtObject.GetTransform(entityName, format);
                            var args = new XsltArgumentList();
                            args.AddExtensionObject("urn:ExtObj", new XsltExtObject());
                            path = Path.Combine(exportFolder, $"{entityName}.{CandidateId}.html");
                            await SaveDocumentAsync(doc, path, xslt, args);
                            break;
                        default:
                            await SaveDocumentAsync(doc, path, null, null);
                            break;
                    }
                    DataTemplateProvider.Open(path);
                }, this.WhenAnyValue(x => x.CandidateId, y => y != 0)
            ).DisposeWith(Disposables);
    }
    #endregion

    private async Task SaveDocumentAsync(XmlDocument xmlDoc, string filePath,
                                XslCompiledTransform? xslt, XsltArgumentList? args)
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

    #region FirstName
    private string? _firstName;
    [Required]
    [StringLength(250, MinimumLength = 2)]
    public string? FirstName
    {
        get => _firstName;
        set
        {
            if (!string.IsNullOrWhiteSpace(value) && Candidate != null)
            {
                Candidate.FirstName = value;
                this.RaiseAndSetIfChanged(ref _firstName, value);
                this.RaisePropertyChanged(nameof(IsValid));
            }
        }
    }
    #endregion

    #region LastName
    private string? _lastName;
    [Required]
    [StringLength(250, MinimumLength = 2)]
    public string? LastName
    {
        get => _lastName;
        set
        {
            if (!string.IsNullOrWhiteSpace(value) && Candidate != null)
            {
                Candidate.LastName = value;
                this.RaiseAndSetIfChanged(ref _lastName, value);
                this.RaisePropertyChanged(nameof(IsValid));
            }
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
            if (!string.IsNullOrWhiteSpace(value) && Candidate != null)
            {
                Candidate.Title = value;
                this.RaiseAndSetIfChanged(ref _candidateTitle, value);
            }
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
            if (value != null && Candidate != null)
            {
                Candidate.BirthDate = value;
                this.RaiseAndSetIfChanged(ref _birthDate, value);
                this.RaisePropertyChanged(nameof(this.Age));
                this.RaisePropertyChanged(nameof(IsValid));
            }
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
    private LocationViewModel? _location;
    public LocationViewModel? Location
    {
        get => _location;
        set
        {
            if (value != null && Candidate != null)
            {
                Candidate.Location = value.Location;
                this.RaiseAndSetIfChanged(ref _location, value);
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
            if (Candidate != null)
            {
                Candidate.Enabled = value;
                this.RaiseAndSetIfChanged(ref _enabled, value);
                this.RaisePropertyChanged(nameof(IsValid));
            }
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

    #region CandidateSkills
    private SkillsViewModel? _candidateSkills;
    public SkillsViewModel? CandidateSkills
    {
        get => _candidateSkills;
        set => this.RaiseAndSetIfChanged(ref _candidateSkills, value);
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
