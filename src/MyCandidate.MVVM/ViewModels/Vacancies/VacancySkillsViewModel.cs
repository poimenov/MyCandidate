using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia.PropertyGrid.Services;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.MVVM.ViewModels.Tools;
using ReactiveUI;
using System.ComponentModel;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.ViewModels.Vacancies;

public class VacancySkillsViewModel : ViewModelBase
{
    private readonly Vacancy _vacancy;

    public VacancySkillsViewModel(Vacancy vacancy, IProperties properties)
    {
        _vacancy = vacancy;
        Properties = properties;
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        SourceVacancySkills = new ObservableCollectionExtended<VacancySkill>(_vacancy.VacancySkills);
        SourceVacancySkills.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _vacancySkills)
            .Subscribe();

        _vacancySkills.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);

        this.WhenAnyValue(x => x.SelectedVacancySkill)
            .Subscribe(
                x =>
                {
                    if (Properties != null)
                    {
                        Properties.SelectedItem = x;
                        Properties.SelectedTypeName = LocalizationService.Default["Skill"]; ;
                    }
                }
            );

        DeleteVacancySkillCmd = ReactiveCommand.Create(
            (object obj) =>
            {
                SourceVacancySkills.Remove((VacancySkill)obj);
            }
        );

        CreateVacancySkillCmd = ReactiveCommand.Create(
            () =>
            {
                var _newVacancySkill = new VacancySkill()
                {
                    Vacancy = _vacancy,
                    VacancyId = _vacancy.Id,
                    Seniority = new Seniority
                    {
                        Id = 1,
                        Name = SeniorityNames.Unknown,
                        Enabled =true
                    }                                        
                };
                SourceVacancySkills.Add(_newVacancySkill);
                _newVacancySkill.PropertyChanged += ItemPropertyChanged;
                SelectedVacancySkill = _newVacancySkill;
            }
        );         
    }

    private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(IsValid));
    }

    public bool IsValid
    {
        get
        {
            if(VacancySkills.Select(x => x.Skill.Name).Distinct().Count() != VacancySkills.Count())
            {
                return false;
            }
            return true;
        }
    }    

    private void CultureChanged(object? sender, EventArgs e)
    {
        if (Properties != null)
        {
            Properties.SelectedTypeName = LocalizationService.Default["Skill"];
        }
    }

    public IProperties? Properties { get; set; }

    #region VacancySkills
    public ObservableCollectionExtended<VacancySkill> SourceVacancySkills;
    private readonly ReadOnlyObservableCollection<VacancySkill> _vacancySkills;
    public ReadOnlyObservableCollection<VacancySkill> VacancySkills => _vacancySkills;
    #endregion

    #region SelectedVacancySkill
    private VacancySkill? _selectedVacancySkill;
    public VacancySkill? SelectedVacancySkill
    {
        get => _selectedVacancySkill;
        set => this.RaiseAndSetIfChanged(ref _selectedVacancySkill, value);
    }
    #endregion  

    public IReactiveCommand CreateVacancySkillCmd { get; }
    public IReactiveCommand DeleteVacancySkillCmd { get; }       
}
