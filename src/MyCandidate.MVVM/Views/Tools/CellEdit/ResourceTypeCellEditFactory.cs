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
using MyCandidate.MVVM.DataTemplates;
using MyCandidate.MVVM.Models;
using PropertyModels.Extensions;

namespace MyCandidate.MVVM.Views.Tools.CellEdit;

class ResourceTypeCellEditFactory : AbstractCellEditFactory
{
    private readonly IDictionariesDataAccess _dataAccess;

    public ResourceTypeCellEditFactory()
    {
        _dataAccess = ((App)Application.Current!)!.GetRequiredService<IDictionariesDataAccess>();
    }
    public ResourceTypeCellEditFactory(IDictionariesDataAccess dataAccess)
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
        if (propertyDescriptor.PropertyType != typeof(ResourceType))
        {
            return null;
        }

        var itemSource = _dataAccess.GetResourceTypes().Where(x => x.Enabled == true).ToList();
        if (target is ResourceModel resourceModel && resourceModel.ResourceModelType == TargetModelType.Vacancy)
        {
            itemSource = itemSource.Where(x => x.Name == ResourceTypeNames.Path
                        || x.Name == ResourceTypeNames.Url).ToList();
        }

        var control = new ComboBox
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            ItemsSource = itemSource,

            ItemTemplate = new FuncDataTemplate<ResourceType>((value, namescope) =>
            {
                var retVal = new StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal
                };

                var contentControl = new ContentControl
                {
                    ContentTemplate = DataTemplateProvider.ResourceTypeImage,
                    Content = value,
                    Margin = new Thickness(4, 0)
                };

                var resourceTypeName = DataTemplateProvider.ResourceTypeName.Build(value);

                if (contentControl != null)
                {
                    retVal.Children.Add(contentControl);
                }

                if (resourceTypeName != null)
                {
                    retVal.Children.Add(resourceTypeName);
                }

                return retVal;
            })
        };

        control.WhenPropertyChanged(x => x.SelectedValue)
        .Subscribe(
            x =>
            {
                if (x.Value is ResourceType resourceType && target is ResourceModel resourceModel)
                {
                    resourceModel.ResourceTypeId = resourceType.Id;
                    resourceModel.RaisePropertyChanged(nameof(ResourceModel.ResourceTypeId));
                    resourceModel.ResourceType = resourceType;
                    resourceModel.RaisePropertyChanged(nameof(ResourceModel.ResourceType));
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

        if (propertyDescriptor.PropertyType != typeof(ResourceType))
        {
            return false;
        }

        if (control != null)
        {
            ValidateProperty(control, propertyDescriptor, target);
        }

        if (control is ComboBox cb && target is ResourceModel resourceModel)
        {
            cb.SelectedItem = resourceModel.ResourceType;
            cb.SelectedIndex = cb.ItemsSource!.OfType<ResourceType>().IndexOf(resourceModel.ResourceType, new ResourceTypeEqualityComparer());
            return true;
        }

        return false;
    }
}