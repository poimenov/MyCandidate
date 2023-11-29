using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using Avalonia.PropertyGrid.Services;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.DataAccess;
using PropertyModels.Extensions;
using ReactiveUI;

namespace MyCandidate.MVVM.Views.Tools
{
    public class ExtendedPropertyGrid : PropertyGrid
    {
        static ExtendedPropertyGrid()
        {
            CellEditFactoryService.Default.AddFactory(new CountryCellEditFactory(new Countries()));
        }
    }

    class CountryCellEditFactory : AbstractCellEditFactory
    {
        private readonly IDataAccess<Country> _dataAccess;

        public CountryCellEditFactory()
        {
            _dataAccess = new Countries();
        }

        public CountryCellEditFactory(IDataAccess<Country> dataAccess)
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
            if (propertyDescriptor.PropertyType != typeof(Country))
            {
                return null;
            }

            ComboBox control = new ComboBox();
            control.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            control.ItemsSource = _dataAccess.ItemsList.Where(x => x.Enabled == true);
            
            control.ItemTemplate = new FuncDataTemplate<Country>((value, namescope) =>
            {
                return new TextBlock() { Text = value?.Name };
            });

            control.WhenPropertyChanged(x => x.SelectedValue)
            .Subscribe(
                x =>
                {
                    if(x.Value is Country country && target is City city)
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

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ComboBox cb && target is City city)
            {
                cb.SelectedItem = city.Country;
                cb.SelectedIndex = cb.ItemsSource!.OfType<Country>().IndexOf(city.Country, new CountryEqualityComparer());
                return true;
            }

            return false;
        }
    }

    class CountryEqualityComparer : IEqualityComparer<Country>
    {
        public bool Equals(Country? x, Country? y)
        {
            if(ReferenceEquals(x, y))
            {
                return true;
            }

        if (x is null || y is null)
            return false;

        return x.Id == y.Id
            && x.Name == y.Name
            && x.Enabled == y.Enabled;            
        }

        public int GetHashCode([DisallowNull] Country obj) => HashCode.Combine(obj.Id.GetHashCode(), obj.Name.GetHashCode(), obj.Enabled.GetHashCode());
    }
}

