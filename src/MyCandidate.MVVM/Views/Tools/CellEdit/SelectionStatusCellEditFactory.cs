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

namespace MyCandidate.MVVM.Views.Tools.CellEdit;


public class SelectionStatusCellEditFactory : AbstractCellEditFactory
{
    private readonly IDictionariesDataAccess _dataAccess;
    public SelectionStatusCellEditFactory()
    {
        _dataAccess = ((App)Application.Current!)!.GetRequiredService<IDictionariesDataAccess>();
    }

    public SelectionStatusCellEditFactory(IDictionariesDataAccess dataAccess)
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
        if (propertyDescriptor.PropertyType != typeof(SelectionStatus))
        {
            return null;
        }
        var control = new ComboBox
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            ItemsSource = _dataAccess.GetSelectionStatusesAsync().Result.Where(x => x.Enabled == true),

            ItemTemplate = new FuncDataTemplate<SelectionStatus>((value, namescope) =>
            {
                return new TextBlock() { Text = value?.Name };
            })
        };

        control.WhenPropertyChanged(x => x.SelectedValue)
        .Subscribe(
            x =>
            {
                if (x.Value is SelectionStatus status
                    && target is CandidateOnVacancy candidateOnVacancy
                    && candidateOnVacancy.SelectionStatusId != status.Id)
                {
                    candidateOnVacancy.SelectionStatusId = status.Id;
                    candidateOnVacancy.RaisePropertyChanged(nameof(CandidateOnVacancy.SelectionStatusId));
                    candidateOnVacancy.SelectionStatus = status;
                    candidateOnVacancy.RaisePropertyChanged(nameof(CandidateOnVacancy.SelectionStatus));
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

        if (propertyDescriptor.PropertyType != typeof(SelectionStatus))
        {
            return false;
        }

        if (control != null)
        {
            ValidateProperty(control, propertyDescriptor, target);
        }

        if (control is ComboBox cb && target is CandidateOnVacancy candidateOnVacancy)
        {
            cb.SelectedItem = candidateOnVacancy.SelectionStatus;
            cb.SelectedIndex = cb.ItemsSource!.OfType<SelectionStatus>().IndexOf(candidateOnVacancy.SelectionStatus, new SelectionStatusEqualityComparer());
            return true;
        }

        return false;
    }
}