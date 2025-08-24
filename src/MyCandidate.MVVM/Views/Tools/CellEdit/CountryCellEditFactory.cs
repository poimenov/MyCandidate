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

class CountryCellEditFactory : AbstractCellEditFactory
{
    private readonly IDataAccess<Country> _dataAccess;

    public CountryCellEditFactory()
    {
        _dataAccess = ((App)Application.Current!)!.GetRequiredService<IDataAccess<Country>>();
    }

    public CountryCellEditFactory(IDataAccess<Country> dataAccess)
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
        if (propertyDescriptor.PropertyType != typeof(Country))
        {
            return null;
        }

        var control = new ComboBox
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            ItemsSource = _dataAccess.GetItemsListAsync().Result.Where(x => x.Enabled == true),

            ItemTemplate = new FuncDataTemplate<Country>((value, namescope) =>
            {
                return new TextBlock() { Text = value?.Name };
            })
        };

        control.WhenPropertyChanged(x => x.SelectedValue)
        .Subscribe(
            x =>
            {
                if (x.Value is Country country
                    && target is City city
                    && city.CountryId != country.Id)
                {
                    city.CountryId = country.Id;
                    city.RaisePropertyChanged(nameof(City.CountryId));
                    city.Country = country;
                    city.RaisePropertyChanged(nameof(City.Country));
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

        if (propertyDescriptor.PropertyType != typeof(Country))
        {
            return false;
        }

        if (control != null)
        {
            ValidateProperty(control, propertyDescriptor, target);
        }

        if (control is ComboBox cb && target is City city)
        {
            cb.SelectedItem = city.Country;
            cb.SelectedIndex = cb.ItemsSource!.OfType<Country>().IndexOf(city.Country, new CountryEqualityComparer());
            return true;
        }

        return false;
    }
}