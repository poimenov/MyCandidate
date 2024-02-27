using System;
using System.Collections.Generic;
using Avalonia.PropertyGrid.Services;
using Dock.Model.ReactiveUI.Controls;
using MyCandidate.Common;
using MyCandidate.MVVM.Models;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Tools;

public class PropertiesViewModel : Tool, IProperties
{
    public PropertiesViewModel()
    {
        CanClose = false;
        CanFloat = false;
        CanPin = false;
        _selectedTypeName = "Properties";

        LocalizationService.Default.OnCultureChanged += CultureChanged;

        this.WhenAnyValue(x => x.SelectedTypeName)
            .Subscribe
            (
                x =>
                {
                    Title = LocalizationService.Default[SelectedTypeName];
                }
            ); 
        this.WhenAnyValue(x => x.SelectedItem)
            .Subscribe
            (
                x =>
                {
                    if(x != null)
                    {
                        var @switch = new Dictionary<Type, Action> {
                            { typeof(Country), () => SelectedTypeName = "Country" },
                            { typeof(City), () => SelectedTypeName = "City" },
                            { typeof(Office), () => SelectedTypeName = "Office" },
                            { typeof(Company), () => SelectedTypeName = "Company" },
                            { typeof(SkillCategory), () => SelectedTypeName = "SkillCategory" },
                            { typeof(Skill), () => SelectedTypeName = "Skill" },
                            { typeof(CandidateOnVacancyExt), () => SelectedTypeName = "Candidate_for_vacancy" },
                            { typeof(ResourceModel), () => SelectedTypeName = "Resource" },
                            { typeof(SkillModel), () => SelectedTypeName = "Skill" }, 
                            { typeof(CommentExt), () => SelectedTypeName = "Comment" },    
                            { typeof(CandidateModel), () => SelectedTypeName = "Candidate" }, 
                            { typeof(VacancyModel), () => SelectedTypeName = "Vacancy" },                         
                        };                        
                        @switch[x.GetType()]();                        
                    }                    
                }
            );             
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        Title = LocalizationService.Default[SelectedTypeName];
    }

    #region SelectedItem
    private object? _selectedItem;
    public object? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    #endregion

    #region SelectedTypeName
    private string _selectedTypeName;
    public string SelectedTypeName
    {
        get => _selectedTypeName;
        set => this.RaiseAndSetIfChanged(ref _selectedTypeName, value);
    }    
    #endregion
}
