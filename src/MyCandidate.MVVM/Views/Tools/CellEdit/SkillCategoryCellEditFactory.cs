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

class SkillCategoryCellEditFactory : AbstractCellEditFactory
{
    private readonly IDataAccess<SkillCategory> _dataAccess;

    public SkillCategoryCellEditFactory()
    {
        _dataAccess = ((App)Application.Current).GetRequiredService<IDataAccess<SkillCategory>>();
    }

    public SkillCategoryCellEditFactory(IDataAccess<SkillCategory> dataAccess)
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
        if (propertyDescriptor.PropertyType != typeof(SkillCategory))
        {
            return null;
        }

        ComboBox control = new ComboBox
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            ItemsSource = _dataAccess.ItemsList.Where(x => x.Enabled == true),

            ItemTemplate = new FuncDataTemplate<SkillCategory>((value, namescope) =>
            {
                return new TextBlock() { Text = value?.Name };
            })
        };

        control.WhenPropertyChanged(x => x.SelectedValue)
        .Subscribe(
            x =>
            {
                if (x.Value is SkillCategory category
                    && target is Skill skill
                    && skill.SkillCategoryId != category.Id)
                {
                    skill.SkillCategoryId = category.Id;
                    skill.RaisePropertyChanged(nameof(Skill.SkillCategoryId));
                    skill.SkillCategory = category;
                    skill.RaisePropertyChanged(nameof(Skill.SkillCategory));
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

        if (propertyDescriptor.PropertyType != typeof(SkillCategory))
        {
            return false;
        }

        ValidateProperty(control, propertyDescriptor, target);

        if (control is ComboBox cb && target is Skill skill)
        {
            cb.SelectedItem = skill.SkillCategory;
            cb.SelectedIndex = cb.ItemsSource!.OfType<SkillCategory>().IndexOf(skill.SkillCategory, new SkillCategoryEqualityComparer());
            return true;
        }

        return false;
    }
}
