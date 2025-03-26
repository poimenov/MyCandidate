using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using PropertyModels.ComponentModel;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Tools;

public class SkillViewModel : ViewModelBase
{
    private bool _skillChanges = false;
    public SkillViewModel(IDataAccess<SkillCategory> skillCategories, IDataAccess<Skill> skills)
    {
        SkillCategories = skillCategories.ItemsList.Where(x => x.Enabled == true);
        SkillsSource = new ObservableCollectionExtended<Skill>(skills!.ItemsList.Where(x => x.Enabled == true));
        SkillsSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter!)
            .Bind(out _skillsList)
            .Subscribe();

        this.WhenAnyValue(x => x.Skill)
            .Subscribe
            (
                x =>
                {
                    if (x != null && !_skillChanges)
                    {
                        _skillChanges = true;
                        var skillCategoryId = Skills.First(c => c.Id == x.Id).SkillCategoryId;
                        SkillCategory = SkillCategories.First(c => c.Id == skillCategoryId);
                        SelectedSkill = Skills.First(c => c.Id == x.Id);
                        _skillChanges = false;
                    }
                }
            );

        this.WhenAnyValue(x => x.SelectedSkill)
            .Subscribe
            (
                x =>
                {
                    if (x != null && Skill!.Id != x.Id)
                    {
                        Skill = SelectedSkill;
                        this.RaisePropertyChanged(nameof(this.Skill));
                    }
                }
            );

        this.WhenAnyValue(x => x.SkillCategory)
            .Subscribe
            (
                x =>
                {
                    if (x != null && Skills.Any(c => c.SkillCategoryId == x.Id) && !_skillChanges)
                    {
                        this.Skill = Skills.First(c => c.SkillCategoryId == x.Id);
                    }
                }
            );
    }

    public ObservableCollectionExtended<Skill> SkillsSource;
    private readonly ReadOnlyObservableCollection<Skill> _skillsList;
    public ReadOnlyObservableCollection<Skill> Skills => _skillsList;

    private IEnumerable<SkillCategory>? _skillCategories;
    public IEnumerable<SkillCategory>? SkillCategories
    {
        get => _skillCategories;
        set => _skillCategories = value;
    }

    private Skill? _skill;
    public Skill? Skill
    {
        get => _skill;
        set => this.RaiseAndSetIfChanged(ref _skill, value);
    }

    private Skill? _selectedSkill;
    public Skill? SelectedSkill
    {
        get => _selectedSkill;
        set => this.RaiseAndSetIfChanged(ref _selectedSkill, value);
    }

    private SkillCategory? _skillCategory;
    public SkillCategory? SkillCategory
    {
        get => _skillCategory;
        set => this.RaiseAndSetIfChanged(ref _skillCategory, value);
    }

    private IObservable<Func<Skill, bool>>? Filter =>
        this.WhenAnyValue(x => x.SkillCategory)
            .Select((x) => MakeFilter(x));

    private Func<Skill, bool> MakeFilter(SkillCategory? skillCategory)
    {
        return item =>
        {
            var retVal = true;
            if (skillCategory != null && skillCategory.Id != 0)
            {
                retVal = item.SkillCategoryId == skillCategory.Id;
            }

            return retVal;
        };
    }
}
