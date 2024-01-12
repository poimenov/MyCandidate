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
using MyCandidate.MVVM.ViewModels.Tools;

namespace MyCandidate.MVVM.Views.Tools.CellEdit;

class LocationCellEditFactory : AbstractCellEditFactory
{
    private readonly IDataAccess<Country> _countries;
    private readonly IDataAccess<City> _cities;
    private readonly IDataAccess<Office> _offices;

    public LocationCellEditFactory()
    {
        _countries = ((App)Application.Current).GetRequiredService<IDataAccess<Country>>();
        _cities = ((App)Application.Current).GetRequiredService<IDataAccess<City>>();
        _offices = ((App)Application.Current).GetRequiredService<IDataAccess<Office>>();
    }

    public LocationCellEditFactory(IDataAccess<Country> countries, IDataAccess<City> cities, IDataAccess<Office> offices)
    {
        _countries = countries;
        _cities = cities;
        _offices = offices;
    }

    public override bool Accept(object accessToken)
    {
        return accessToken is ExtendedPropertyGrid;
    }

    public override Control HandleNewProperty(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        var target = context.Target;
        if (propertyDescriptor.PropertyType != typeof(Common.Location))
        {
            return null;
        }

        var vm = new LocationViewModel(_countries, _cities);
        var control = new LocationView
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            DataContext = vm
        };
        vm.WhenPropertyChanged(x => x.Location)
        .Subscribe(
            x =>
            {
                if (x.Value is Common.Location location
                    && target is Office office)
                {
                    if (office.Id != 0)
                    {
                        var originalOffice = _offices.Get(office.Id);
                        if (originalOffice.Location.CityId != location.CityId || originalOffice.Location.Address != location.Address)
                        {
                            office.RaisePropertyChanged(nameof(Office.Location));
                            office.Location.RaisePropertyChanged(nameof(Common.Location.Name));
                        }
                    }
                    else
                    {
                        office.RaisePropertyChanged(nameof(Office.Location));
                        office.Location.RaisePropertyChanged(nameof(Common.Location.Name));
                    }
                    SetAndRaise(context, context.CellEdit, location);
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

        if (propertyDescriptor.PropertyType != typeof(Common.Location))
        {
            return false;
        }

        ValidateProperty(control, propertyDescriptor, target);

        if (control is LocationView vw
                && vw.DataContext is LocationViewModel vm
                && target is Office office)
        {
            if (office.Location == null)
            {
                office.Location = new Common.Location
                {
                    CityId = _cities.ItemsList.First().Id,
                    City = _cities.ItemsList.First(),
                    Name = string.Empty,
                    Enabled = true
                };
            }

            vm.Location = office.Location;
            return true;
        }

        return false;
    }
}
