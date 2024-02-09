using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.Models;
using MyCandidate.MVVM.ViewModels.Tools;

namespace MyCandidate.MVVM.Views.Tools.CellEdit;

public class SkillCellEditFactory : AbstractCellEditFactory
{
    private readonly IDataAccess<SkillCategory> _skillCategories;
    private readonly IDataAccess<Skill> _skills;

    public SkillCellEditFactory()
    {
        _skillCategories = ((App)Application.Current).GetRequiredService<IDataAccess<SkillCategory>>();
        _skills = ((App)Application.Current).GetRequiredService<IDataAccess<Skill>>();
    }

    public SkillCellEditFactory(IDataAccess<SkillCategory> skillCategories, IDataAccess<Skill> skills)
    {
        _skillCategories = skillCategories;
        _skills = skills;
    }

    public override bool Accept(object accessToken)
    {
        return accessToken is ExtendedPropertyGrid;
    }

    public override Control HandleNewProperty(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        var target = context.Target;
        if (propertyDescriptor.PropertyType != typeof(Skill))
        {
            return null;
        }

        var vm = new SkillViewModel(_skillCategories, _skills);
        var control = new SkillView
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            DataContext = vm
        };

        vm.WhenPropertyChanged(x => x.Skill)
        .Subscribe(
            x =>
            {
                if (x.Value is Skill skill)
                {
                    if (target is SkillModel skillModel)
                    {
                        skillModel.RaisePropertyChanged(nameof(SkillModel.Skill));
                        skillModel.Skill.RaisePropertyChanged(nameof(SkillModel.Skill.Name));
                    }
                    SetAndRaise(context, context.CellEdit, skill);
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

        if (propertyDescriptor.PropertyType != typeof(Skill))
        {
            return false;
        }

        ValidateProperty(control, propertyDescriptor, target);

        if (control is SkillView vw
                && vw.DataContext is SkillViewModel vm)
        {
            if (target is SkillModel skillModel)
            {
                if (skillModel.Skill == null)
                {
                    skillModel.Skill = _skills.ItemsList.First();
                }

                vm.Skill = skillModel.Skill;
                return true;
            }
        }

        return false;
    }
}
