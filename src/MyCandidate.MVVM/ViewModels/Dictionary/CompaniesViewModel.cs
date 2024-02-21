using System;
using Avalonia.PropertyGrid.Services;
using log4net;
using MyCandidate.Common;
using MyCandidate.MVVM.Services;

namespace MyCandidate.MVVM.ViewModels.Dictionary;

public class CompaniesViewModel : DictionaryViewModel<Company>
{
    public CompaniesViewModel(IDictionaryService<Company> service, ILog log) : base(service, log)
    {
        Id = "Companies";
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        Title = LocalizationService.Default["Companies"];     
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        Title = LocalizationService.Default["Companies"];
    }     
}
