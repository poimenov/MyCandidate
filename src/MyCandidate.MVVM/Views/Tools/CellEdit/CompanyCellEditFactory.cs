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

class CompanyCellEditFactory : AbstractCellEditFactory
{
    private readonly IDataAccess<Company> _dataAccess;

    public CompanyCellEditFactory()
    {
        _dataAccess = ((App)Application.Current!)!.GetRequiredService<IDataAccess<Company>>();
    }

    public CompanyCellEditFactory(IDataAccess<Company> dataAccess)
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
        if (propertyDescriptor.PropertyType != typeof(Company))
        {
            return null;
        }

        ComboBox control = new ComboBox
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            ItemsSource = _dataAccess.GetItemsListAsync().Result.Where(x => x.Enabled == true),

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

        if (control != null)
        {
            ValidateProperty(control, propertyDescriptor, target);
        }

        if (control is ComboBox cb && target is Office office)
        {
            cb.SelectedItem = office.Company;
            cb.SelectedIndex = cb.ItemsSource!.OfType<Company>().IndexOf(office.Company, new CompanyEqualityComparer());
            return true;
        }

        return false;
    }
}