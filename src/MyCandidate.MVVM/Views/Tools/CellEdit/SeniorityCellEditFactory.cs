using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using PropertyModels.Extensions;

namespace MyCandidate.MVVM.Views.Tools.CellEdit;

public class SeniorityCellEditFactory : AbstractCellEditFactory
{
    private readonly IDictionariesDataAccess _dataAccess;
    public SeniorityCellEditFactory()
    {
        _dataAccess = ((App)Application.Current).GetRequiredService<IDictionariesDataAccess>();
    }

    public SeniorityCellEditFactory(IDictionariesDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public override bool Accept(object accessToken)
    {
        return accessToken is ExtendedPropertyGrid;
    }

    public override Control HandleNewProperty(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        var target = context.Target;
        if (propertyDescriptor.PropertyType != typeof(Seniority))
        {
            return null;
        }

        var control = new ComboBox
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            ItemsSource = _dataAccess.GetSeniorities().Where(x => x.Enabled == true),

            ItemTemplate = new FuncDataTemplate<Seniority>((value, namescope) =>
            {
                return new TextBlock() { Text = value?.Name };
            })
        };

        control.WhenPropertyChanged(x => x.SelectedValue)
        .Subscribe(
            x =>
            {
                if(x.Value is Seniority seniority)
                {
                    if(target is CandidateSkill candidateSkill)
                    {
                        candidateSkill.SeniorityId = seniority.Id;
                        candidateSkill.RaisePropertyChanged(nameof(CandidateSkill.SeniorityId));
                        candidateSkill.Seniority = seniority;
                        candidateSkill.RaisePropertyChanged(nameof(CandidateSkill.Seniority));                        
                    }
                    else if(target is VacancySkill vacancySkill)
                    {
                        vacancySkill.SeniorityId = seniority.Id;
                        vacancySkill.RaisePropertyChanged(nameof(CandidateSkill.SeniorityId));
                        vacancySkill.Seniority = seniority;
                        vacancySkill.RaisePropertyChanged(nameof(CandidateSkill.Seniority)); 
                    }
                }
            }
        );

        return control;
    }

    public override bool HandlePropertyChanged(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        var target = context.Target;
        var control = context.CellEdit;

        if (propertyDescriptor.PropertyType != typeof(Seniority))
        {
            return false;
        }

        ValidateProperty(control, propertyDescriptor, target);

        if (control is ComboBox cb)
        {
            if(target is CandidateSkill candidateSkill)
            {
                cb.SelectedItem = candidateSkill.Seniority;
                cb.SelectedIndex = cb.ItemsSource!.OfType<Seniority>().IndexOf(candidateSkill.Seniority, new SeniorityEqualityComparer());
                return true;
            }
            else if(target is VacancySkill vacancySkill)
            {
                cb.SelectedItem = vacancySkill.Seniority;
                cb.SelectedIndex = cb.ItemsSource!.OfType<Seniority>().IndexOf(vacancySkill.Seniority, new SeniorityEqualityComparer());
                return true;
            }
        }

        return false;
    }
}
