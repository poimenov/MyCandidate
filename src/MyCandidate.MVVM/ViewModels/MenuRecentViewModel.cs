using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.PropertyGrid.Services;
using MyCandidate.MVVM.Models;
using MyCandidate.MVVM.Services;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels;

public class MenuRecentViewModel : ViewModelBase
{
    private readonly IAppServiceProvider _provider;
    private readonly TargetModelType _modelType;
    private readonly int _countItems;
    public MenuRecentViewModel(IAppServiceProvider appServiceProvider, TargetModelType modelType, int countItems)
    {
        _provider = appServiceProvider;
        _modelType = modelType;
        _countItems = countItems;

        OpenCandidateCmd = ReactiveCommand.Create<int, Unit>(
            (id) =>
            {
                _provider.OpenCandidateViewModel(id);
                return Unit.Default;
            }
        );

        OpenVacancyCmd = ReactiveCommand.Create<int, Unit>(
            (id) =>
            {
                _provider.OpenVacancyViewModel(id);
                return Unit.Default;
            }
        );

        var items = new List<MenuItem>();

        switch (modelType)
        {
            case TargetModelType.Candidate:
                items.AddRange(_provider.CandidateService
                    .GetRecent(countItems)
                    .Select(x => GetMenuItem(OpenCandidateCmd, x.Id, x.Name)));
                break;
            case TargetModelType.Vacancy:
                items.AddRange(_provider.VacancyService
                    .GetRecent(countItems)
                    .Select(x => GetMenuItem(OpenVacancyCmd, x.Id, x.Name)));
                break;
        }

        if (!items.Any())
        {
            items.Add(new MenuItem() { Header = LocalizationService.Default["Empty"] });
            LocalizationService.Default.OnCultureChanged += CultureChanged;
        }

        Items = new ReadOnlyObservableCollection<MenuItem>(new ObservableCollection<MenuItem>(items));
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        Items.First().Header = LocalizationService.Default["Empty"];
    }

    private MenuItem GetMenuItem(ICommand command, int id, string header)
    {
        return new MenuItem
        {
            Header = header,
            Command = command,
            CommandParameter = id
        };
    }



    public ReadOnlyObservableCollection<MenuItem> Items { get; private set; }
    public ReactiveCommand<int, Unit> OpenCandidateCmd { get; }
    public ReactiveCommand<int, Unit> OpenVacancyCmd { get; }
}
