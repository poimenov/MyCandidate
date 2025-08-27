using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.PropertyGrid.Services;
using DynamicData;
using DynamicData.Binding;
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

        OpenCandidateCmd = ReactiveCommand.CreateFromTask<int>(
            async (id) =>
            {
                await _provider.OpenCandidateViewModelAsync(id);
            }
        ).DisposeWith(Disposables);

        OpenVacancyCmd = ReactiveCommand.Create<int>(
            async (id) =>
            {
                await _provider.OpenVacancyViewModelAsync(id);
            }
        ).DisposeWith(Disposables);

        Source = new ObservableCollection<MenuItem>();
        Source.ToObservableChangeSet<MenuItem>()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe()
            .DisposeWith(Disposables);

        LoadDataCmd = ReactiveCommand.CreateFromTask(LoadDataAsync).DisposeWith(Disposables);
        LoadDataCmd.Execute().Subscribe().DisposeWith(Disposables);
    }

    private async Task LoadDataAsync()
    {
        var items = new List<MenuItem>();

        switch (_modelType)
        {
            case TargetModelType.Candidate:
                items.AddRange((await _provider.CandidateService
                    .GetRecentAsync(_countItems))
                    .Select(x => GetMenuItem(OpenCandidateCmd, x.Id, x.Name)));
                break;
            case TargetModelType.Vacancy:
                items.AddRange((await _provider.VacancyService
                    .GetRecentasync(_countItems))
                    .Select(x => GetMenuItem(OpenVacancyCmd, x.Id, x.Name)));
                break;
        }

        if (!items.Any())
        {
            items.Add(new MenuItem() { Header = LocalizationService.Default["Empty"] });
            LocalizationService.Default.OnCultureChanged += CultureChanged;
        }

        RxApp.MainThreadScheduler.Schedule(() =>
        {
            Source.Clear();
            Source.AddRange(items);
        }).DisposeWith(Disposables);
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

    public ObservableCollection<MenuItem> Source = new ObservableCollection<MenuItem>();
    private readonly ReadOnlyObservableCollection<MenuItem> _items;
    public ReadOnlyObservableCollection<MenuItem> Items => _items;
    public ReactiveCommand<int, Unit> OpenCandidateCmd { get; }
    public ReactiveCommand<int, Unit> OpenVacancyCmd { get; }
    public ReactiveCommand<Unit, Unit> LoadDataCmd { get; }
}
