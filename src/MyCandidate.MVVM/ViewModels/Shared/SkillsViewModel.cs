using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.MVVM.ViewModels.Tools;
using ReactiveUI;
using System.ComponentModel;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.Models;
using System.Collections.Generic;
using Avalonia.Input;

namespace MyCandidate.MVVM.ViewModels.Shared;

public class SkillsViewModel : ViewModelBase
{
    public SkillsViewModel(IEnumerable<SkillModel> skills, IProperties properties)
    {
        Properties = properties;
        SourceSkills = new ObservableCollectionExtended<SkillModel>(skills);
        SourceSkills.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _skills)
            .Subscribe();

        _skills.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);

        this.WhenAnyValue(x => x.SelectedSkill)
            .Subscribe(
                x =>
                {
                    if (Properties != null)
                    {
                        Properties.SelectedItem = x;
                    }
                }
            );

        DeleteSkillCmd = ReactiveCommand.Create(
            (SkillModel obj) =>
            {
                SourceSkills.Remove(obj);
                this.RaisePropertyChanged(nameof(IsValid));
            },
            this.WhenAnyValue(x => x.SelectedSkill, x => x.Skills,
                (obj, list) => obj != null && list.Count > 0)
        );

        DeleteSkillKeyDownCmd = ReactiveCommand.Create(
            (KeyEventArgs args) =>
            {
                if (args.Key == Key.Delete && SelectedSkill != null)
                {
                    SourceSkills.Remove(SelectedSkill);
                    this.RaisePropertyChanged(nameof(IsValid));
                }
            },
            this.WhenAnyValue(x => x.SelectedSkill, x => x.Skills,
                (obj, list) => obj != null && list.Count > 0)
        );

        CreateSkillCmd = ReactiveCommand.Create(
            () =>
            {
                var _newSkill = new SkillModel
                {
                    Seniority = new Seniority
                    {
                        Id = 1,
                        Name = SeniorityNames.Unknown,
                        Enabled = true
                    }
                };
                SourceSkills.Add(_newSkill);
                _newSkill.PropertyChanged += ItemPropertyChanged;
                SelectedSkill = _newSkill;
                this.RaisePropertyChanged(nameof(IsValid));
            }
        );
    }

    private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(IsValid));
    }

    public bool IsValid
    {
        get => Skills.Select(x => x.Skill!.Id).Distinct().Count() == Skills.Count();
    }

    public IProperties? Properties { get; set; }

    #region Skills
    public ObservableCollectionExtended<SkillModel> SourceSkills;
    private readonly ReadOnlyObservableCollection<SkillModel> _skills;
    public ReadOnlyObservableCollection<SkillModel> Skills => _skills;
    #endregion

    #region SelectedSkill
    private SkillModel? _selectedSkill;
    public SkillModel? SelectedSkill
    {
        get => _selectedSkill;
        set => this.RaiseAndSetIfChanged(ref _selectedSkill, value);
    }
    #endregion  

    public IReactiveCommand CreateSkillCmd { get; }
    public IReactiveCommand DeleteSkillCmd { get; }
    public IReactiveCommand DeleteSkillKeyDownCmd { get; }
}
