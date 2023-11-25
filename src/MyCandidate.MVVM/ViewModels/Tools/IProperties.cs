using System;

namespace MyCandidate.MVVM.ViewModels.Tools;

public interface IProperties
{
    object? SelectedItem { get; set; }
    string SelectedTypeName { get; set; }
}
