using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
using MyCandidate.MVVM.ViewModels.Tools;
using PropertyModels.Extensions;

namespace MyCandidate.MVVM.Views.Tools
{
    public class ExtendedPropertyGrid : PropertyGrid
    {
        private static App CurrentApplication => (App)Application.Current;
        static ExtendedPropertyGrid()
        {
            CellEditFactoryService.Default.AddFactory(new CountryCellEditFactory(CurrentApplication.GetRequiredService<IDataAccess<Country>>()));
            CellEditFactoryService.Default.AddFactory(new SkillCategoryCellEditFactory(CurrentApplication.GetRequiredService<IDataAccess<SkillCategory>>()));
            CellEditFactoryService.Default.AddFactory(new CompanyCellEditFactory(CurrentApplication.GetRequiredService<IDataAccess<Company>>()));
            CellEditFactoryService.Default.AddFactory(new LocationCellEditFactory(CurrentApplication.GetRequiredService<IDataAccess<Country>>(),
                                                                                    CurrentApplication.GetRequiredService<IDataAccess<City>>(),
                                                                                    CurrentApplication.GetRequiredService<IDataAccess<Office>>()));
        }
    }

    class CountryCellEditFactory : AbstractCellEditFactory
    {
        private readonly IDataAccess<Country> _dataAccess;

        public CountryCellEditFactory()
        {
            _dataAccess = ((App)Application.Current).GetRequiredService<IDataAccess<Country>>();
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

            var control = new ComboBox
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                ItemsSource = _dataAccess.ItemsList.Where(x => x.Enabled == true),

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

    class CompanyCellEditFactory : AbstractCellEditFactory
    {
        private readonly IDataAccess<Company> _dataAccess;

        public CompanyCellEditFactory()
        {
            _dataAccess = ((App)Application.Current).GetRequiredService<IDataAccess<Company>>();
        }

        public CompanyCellEditFactory(IDataAccess<Company> dataAccess)
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
            if (propertyDescriptor.PropertyType != typeof(Company))
            {
                return null;
            }

            ComboBox control = new ComboBox
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                ItemsSource = _dataAccess.ItemsList.Where(x => x.Enabled == true),

                ItemTemplate = new FuncDataTemplate<Company>((value, namescope) =>
                {
                    return new TextBlock() { Text = value?.Name };
                })
            };

            control.WhenPropertyChanged(x => x.SelectedValue)
            .Subscribe(
                x =>
                {
                    if (x.Value is Company company
                        && target is Office office
                        && office.CompanyId != company.Id)
                    {
                        office.CompanyId = company.Id;
                        office.RaisePropertyChanged(nameof(Office.CompanyId));
                        office.Company = company;
                        office.RaisePropertyChanged(nameof(Office.Company));
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

            if (propertyDescriptor.PropertyType != typeof(Company))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ComboBox cb && target is Office office)
            {
                cb.SelectedItem = office.Company;
                cb.SelectedIndex = cb.ItemsSource!.OfType<Company>().IndexOf(office.Company, new CompanyEqualityComparer());
                return true;
            }

            return false;
        }
    }

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

    class CountryEqualityComparer : IEqualityComparer<Country>
    {
        public bool Equals(Country? x, Country? y)
        {
            if (ReferenceEquals(x, y))
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

    class SkillCategoryEqualityComparer : IEqualityComparer<SkillCategory>
    {
        public bool Equals(SkillCategory? x, SkillCategory? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
                return false;

            return x.Id == y.Id
                && x.Name == y.Name
                && x.Enabled == y.Enabled;
        }

        public int GetHashCode([DisallowNull] SkillCategory obj) => HashCode.Combine(obj.Id.GetHashCode(), obj.Name.GetHashCode(), obj.Enabled.GetHashCode());
    }

    class CompanyEqualityComparer : IEqualityComparer<Company>
    {
        public bool Equals(Company? x, Company? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
                return false;

            return x.Id == y.Id
                && x.Name == y.Name
                && x.Enabled == y.Enabled;
        }

        public int GetHashCode([DisallowNull] Company obj) => HashCode.Combine(obj.Id.GetHashCode(), obj.Name.GetHashCode(), obj.Enabled.GetHashCode());
    }
}

