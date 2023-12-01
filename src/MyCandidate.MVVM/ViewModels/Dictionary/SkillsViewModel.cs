using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.PropertyGrid.Services;
using log4net;
using MyCandidate.Common;
using MyCandidate.MVVM.Services;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Dictionary;

public class SkillsViewModel : DictionaryViewModel<Skill>
{
    public SkillsViewModel(IDictionaryService<Skill> service, ILog log) : base(service, log)
    {
        Id = "Skills";
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        Title = LocalizationService.Default["Skills"];
        SelectedTypeName = LocalizationService.Default["Skill"];
        var categories = new List<SkillCategory>() { new SkillCategory() { Id = 0, Name = string.Empty } };
        categories.AddRange(ItemList.Select(x => x.SkillCategory).Distinct().ToList());
        SkillCategories = categories;        
    }

    protected override IObservable<Func<Skill, bool>>? Filter =>
        this.WhenAnyValue(x => x.Enabled, x => x.Name, x => x.SelectedSkillCategory)
            .Select((x) => MakeFilter(x.Item1, x.Item2, x.Item3));    

    private void CultureChanged(object? sender, EventArgs e)
    {
        Title = LocalizationService.Default["Skills"];
        SelectedTypeName = LocalizationService.Default["Skill"];
    }  

    #region SkillCategories
    private IEnumerable<SkillCategory> _skillCategories;
    public IEnumerable<SkillCategory> SkillCategories
    {
        get => _skillCategories;
        set => this.RaiseAndSetIfChanged(ref _skillCategories, value);
    }
    #endregion     

    #region SelectedSkillCategory
    private SkillCategory? _selectedskillCategory;
    public SkillCategory? SelectedSkillCategory
    {
        get => _selectedskillCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedskillCategory, value);
    }
    #endregion   

    private Func<Skill, bool> MakeFilter(bool? enabled, string name, SkillCategory? category)
    {
        return item =>
        {
            var byCountry = true;
            if(category != null && category.Id != 0)
            {
                byCountry = item.SkillCategoryId == category.Id;
            }
            var byName = true;
            if (!string.IsNullOrEmpty(name) && name.Length > 2)
            {
                byName = item.Name.StartsWith(name, true, CultureInfo.InvariantCulture);
            }

            if (enabled.HasValue)
            {
                return item.Enabled == enabled && byName && byCountry;
            }
            else
            {
                return byName && byCountry;
            }
        };
    }          
}