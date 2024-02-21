using System;
using Avalonia.PropertyGrid.Services;
using log4net;
using MyCandidate.Common;
using MyCandidate.MVVM.Services;

namespace MyCandidate.MVVM.ViewModels.Dictionary;

public class CategoriesViewModel : DictionaryViewModel<SkillCategory>
{
    public CategoriesViewModel(IDictionaryService<SkillCategory> service, ILog log) : base(service, log)
    {
        Id = "SkillCategories";
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        Title = LocalizationService.Default["SkillCategories"];
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        Title = LocalizationService.Default["SkillCategories"];
    }    
}
