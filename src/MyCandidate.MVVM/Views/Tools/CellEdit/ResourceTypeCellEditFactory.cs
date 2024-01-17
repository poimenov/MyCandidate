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
using PropertyModels.Extensions;

namespace MyCandidate.MVVM.Views.Tools.CellEdit;

class ResourceTypeCellEditFactory : AbstractCellEditFactory
{
    private readonly IDictionariesDataAccess _dataAccess;

    public ResourceTypeCellEditFactory()
    {
        _dataAccess = ((App)Application.Current).GetRequiredService<IDictionariesDataAccess>();
    }

    public ResourceTypeCellEditFactory(IDictionariesDataAccess dataAccess)
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
        if (propertyDescriptor.PropertyType != typeof(ResourceType))
        {
            return null;
        }

        var control = new ComboBox
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            ItemsSource = _dataAccess.GetResourceTypes().Where(x => x.Enabled == true),

            ItemTemplate = new FuncDataTemplate<ResourceType>((value, namescope) =>
            {
                var retVal = new StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal
                };

                retVal.Children.Add(new ContentControl
                {
                    ContentTemplate = DataTemplateProvider.ResourceTypeImage,
                    Content = value,
                    Margin = new Thickness(4, 0)
                });

                retVal.Children.Add(DataTemplateProvider.ResourceTypeName.Build(value));

                return retVal;
            })
        };

        control.WhenPropertyChanged(x => x.SelectedValue)
        .Subscribe(
            x =>
            {
                if (x.Value is ResourceType resourceType
                    && target is CandidateResource candidateResource)
                {
                    candidateResource.ResourceTypeId = resourceType.Id;
                    candidateResource.RaisePropertyChanged(nameof(CandidateResource.ResourceTypeId));
                    candidateResource.ResourceType = resourceType;
                    candidateResource.RaisePropertyChanged(nameof(CandidateResource.ResourceType));
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

        ValidateProperty(control, propertyDescriptor, target);

        if (control is ComboBox cb && target is CandidateResource candidateResource)
        {
            cb.SelectedItem = candidateResource.ResourceType;
            cb.SelectedIndex = cb.ItemsSource!.OfType<ResourceType>().IndexOf(candidateResource.ResourceType, new ResourceTypeEqualityComparer());
            return true;
        }

        return false;
    }
}
