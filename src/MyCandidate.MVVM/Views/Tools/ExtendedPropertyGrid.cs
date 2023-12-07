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
using MyCandidate.DataAccess;
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

            ComboBox control = new ComboBox();
            control.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            control.ItemsSource = _dataAccess.ItemsList.Where(x => x.Enabled == true);
            
            control.ItemTemplate = new FuncDataTemplate<SkillCategory>((value, namescope) =>
            {
                return new TextBlock() { Text = value?.Name };
            });

            control.WhenPropertyChanged(x => x.SelectedValue)
            .Subscribe(
                x =>
                {
                    if(x.Value is SkillCategory category && target is Skill skill)
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

            ComboBox control = new ComboBox();
            control.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            control.ItemsSource = _dataAccess.ItemsList.Where(x => x.Enabled == true);
            
            control.ItemTemplate = new FuncDataTemplate<Company>((value, namescope) =>
            {
                return new TextBlock() { Text = value?.Name };
            });

            control.WhenPropertyChanged(x => x.SelectedValue)
            .Subscribe(
                x =>
                {
                    if(x.Value is Company company && target is Office office)
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

    class SkillCategoryEqualityComparer : IEqualityComparer<SkillCategory>
    {
        public bool Equals(SkillCategory? x, SkillCategory? y)
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

        public int GetHashCode([DisallowNull] SkillCategory obj) => HashCode.Combine(obj.Id.GetHashCode(), obj.Name.GetHashCode(), obj.Enabled.GetHashCode());
    } 

    class CompanyEqualityComparer : IEqualityComparer<Company>
    {
        public bool Equals(Company? x, Company? y)
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

        public int GetHashCode([DisallowNull] Company obj) => HashCode.Combine(obj.Id.GetHashCode(), obj.Name.GetHashCode(), obj.Enabled.GetHashCode());
    }       
}

