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
using MyCandidate.MVVM.Models;
using PropertyModels.Extensions;

namespace MyCandidate.MVVM.Views.Tools.CellEdit;

public class SeniorityCellEditFactory : AbstractCellEditFactory
{
    private readonly IDictionariesDataAccess _dataAccess;
    public SeniorityCellEditFactory()
    {
        _dataAccess = ((App)Application.Current!)!.GetRequiredService<IDictionariesDataAccess>();
    }

    public SeniorityCellEditFactory(IDictionariesDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public override bool Accept(object accessToken)
    {
        return accessToken is ExtendedPropertyGrid;
    }

    public override Control? HandleNewProperty(PropertyCellContext context)
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
            ItemsSource = _dataAccess.GetSenioritiesAsync().Result.Where(x => x.Enabled == true),

            ItemTemplate = new FuncDataTemplate<Seniority>((value, namescope) =>
            {
                return new TextBlock() { Text = value?.Name };
            })
        };

        control.WhenPropertyChanged(x => x.SelectedValue)
        .Subscribe(
            x =>
            {
                if (x.Value is Seniority seniority && target is SkillModel skillModel)
                {
                    skillModel.Seniority = seniority;
                    skillModel.RaisePropertyChanged(nameof(SkillModel.Seniority));
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

        if (control is not null)
        {
            ValidateProperty(control, propertyDescriptor, target);
        }

        if (control is ComboBox cb && target is SkillModel skillModel)
        {
            cb.SelectedItem = skillModel.Seniority;
            cb.SelectedIndex = cb.ItemsSource!.OfType<Seniority?>().IndexOf(skillModel.Seniority, new SeniorityEqualityComparer());
            return true;
        }

        return false;
    }
}