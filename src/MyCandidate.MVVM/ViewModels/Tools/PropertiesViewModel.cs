using System;
using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Tools;

public class PropertiesViewModel : Tool, IProperties
{
    public PropertiesViewModel()
    {
        CanClose = false;
        _selectedTypeName = "Properties";

        this.WhenAnyValue(x => x.SelectedTypeName)
            .Subscribe
            (
                x =>
                {
                    Title = x;
                }
            ); 
    }

    #region SelectedItem
    private object? _selectedItem;
    public object? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    #endregion

    #region SelectedTypeName
    private string _selectedTypeName;
    public string SelectedTypeName
    {
        get => _selectedTypeName;
        set => this.RaiseAndSetIfChanged(ref _selectedTypeName, value);
    }    
    #endregion
}
