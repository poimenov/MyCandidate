using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.PropertyGrid.Services;
using log4net;
using MyCandidate.Common;
using MyCandidate.MVVM.Services;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Dictionary;

public class OfficiesViewModel : DictionaryViewModel<Office>
{
    public OfficiesViewModel(IDictionaryService<Office> service, ILog log) : base(service, log)
    {
        Id = "Officies";
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        Title = LocalizationService.Default["Officies"];
        SelectedTypeName = LocalizationService.Default["Office"];
        var companies = new List<Company>() { new Company() { Id = 0, Name = string.Empty } };
        companies.AddRange(ItemList.Select(x => x.Company).Distinct().ToList());
        Companies = companies;        
    }

    protected override IObservable<Func<Office, bool>>? Filter =>
        this.WhenAnyValue(x => x.Enabled, x => x.Name, x => x.SelectedCompany)
            .Select((x) => MakeFilter(x.Item1, x.Item2, x.Item3));

    private void CultureChanged(object? sender, EventArgs e)
    {
        Title = LocalizationService.Default["Cities"];
        SelectedTypeName = LocalizationService.Default["City"];
    }

    #region Companies
    private IEnumerable<Company> _companies;
    public IEnumerable<Company> Companies
    {
        get => _companies;
        set => this.RaiseAndSetIfChanged(ref _companies, value);
    }
    #endregion     

    #region SelectedCompany
    private Company? _selectedCompany;
    public Company? SelectedCompany
    {
        get => _selectedCompany;
        set => this.RaiseAndSetIfChanged(ref _selectedCompany, value);
    }
    #endregion     

    private Func<Office, bool> MakeFilter(bool? enabled, string name, Company? company)
    {
        return item =>
        {
            var byCountry = true;
            if(company != null && company.Id != 0)
            {
                byCountry = item.CompanyId == company.Id;
            }
            var byName = true;
            if (!string.IsNullOrEmpty(name) && name.Length > 2)
            {
                byName = item.Name.StartsWith(name, true, CultureInfo.InvariantCulture);
            }

            if (enabled.HasValue)
            {
                return item.Enabled == enabled && byName && byCountry;
            }
            else
            {
                return byName && byCountry;
            }
        };
    }    
}
