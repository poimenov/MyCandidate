using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.DataAccess;
using MyCandidate.MVVM.Models;

namespace MyCandidate.MVVM.Views.Tools.CellEdit;

public class SkillsCellFactory : AbstractCellEditFactory
{
    //private readonly IDataAccess<SkillCategory> _skillCategories;
    private readonly IDataAccess<Skill> _skills;
    private readonly IDictionariesDataAccess _dictionaries;

    public SkillsCellFactory()
    {
        //_skillCategories = ((App)Application.Current!)!.GetRequiredService<IDataAccess<SkillCategory>>();
        _skills = ((App)Application.Current!)!.GetRequiredService<IDataAccess<Skill>>();
        _dictionaries = ((App)Application.Current).GetRequiredService<IDictionariesDataAccess>();
    }

    public SkillsCellFactory(/*IDataAccess<SkillCategory> skillCategories,*/ IDataAccess<Skill> skills, IDictionariesDataAccess dictionaries)
    {
        //_skillCategories = skillCategories;
        _skills = skills;
        _dictionaries = dictionaries;
    }

    public override bool Accept(object accessToken)
    {
        return accessToken is ExtendedPropertyGrid;
    }

    public override Control? HandleNewProperty(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        var target = context.Target;
        if (propertyDescriptor.PropertyType != typeof(IEnumerable<SkillValue>))
        {
            return null;
        }

        var control = new StackPanel
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
        };

        if (target is ISkillValueList obj)
        {
            var skills = _skills.ItemsList.ToDictionary(x => x.Id);
            var seniorities = _dictionaries.GetSeniorities().ToDictionary(x => x.Id);
            foreach (var item in obj.Skills.OrderByDescending(x => x.SeniorityId))
            {
                var line = new TextBox
                {
                    Text = $"{skills[item.SkillId].Name} : {seniorities[item.SeniorityId].Name}",
                    IsEnabled = false,
                    Margin = new Thickness(0, 0, 0, 6),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
                };

                control.Children.Add(line);
            }
        }

        return control;
    }

    public override bool HandlePropertyChanged(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        var target = context.Target;
        var control = context.CellEdit;

        if (propertyDescriptor.PropertyType != typeof(IEnumerable<SkillValue>))
        {
            return false;
        }

        if (control != null)
        {
            ValidateProperty(control, propertyDescriptor, target);
        }

        return false;
    }
}