using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.MVVM.Models;
using MyCandidate.MVVM.Services;
using MyCandidate.MVVM.ViewModels.Candidates;
using MyCandidate.MVVM.ViewModels.Vacancies;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Shared;

public class CandidateOnVacancyViewModel : ViewModelBase
{
    private readonly CandidateViewModel? _candidateViewModel;
    private readonly VacancyViewModel? _vacancyViewModel;
    private readonly IAppServiceProvider _provider;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    public CandidateOnVacancyViewModel(CandidateViewModel candidateViewModel, IAppServiceProvider appServiceProvider)
    {
        _candidateViewModel = candidateViewModel;
        _vacancyViewModel = null;
        _provider = appServiceProvider;
        Source = new ObservableCollectionExtended<CandidateOnVacancyExt>();
        Source.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        DeleteCmd = CreateDeleteCmd();
        DeleteKeyDownCmd = CreateDeleteKeyDownCmd();

        LoadDataCmd = ReactiveCommand.CreateFromTask(LoadCandidateOnVacancy);
        _isLoading = LoadDataCmd.IsExecuting
            .ToProperty(this, x => x.IsLoading);
        LoadDataCmd.Execute().Subscribe();
    }

    public CandidateOnVacancyViewModel(VacancyViewModel vacancyViewModel, IAppServiceProvider appServiceProvider)
    {
        _vacancyViewModel = vacancyViewModel;
        _candidateViewModel = null;
        _provider = appServiceProvider;
        Source = new ObservableCollectionExtended<CandidateOnVacancyExt>();
        Source.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        DeleteCmd = CreateDeleteCmd();
        DeleteKeyDownCmd = CreateDeleteKeyDownCmd();

        LoadDataCmd = ReactiveCommand.CreateFromTask(LoadCandidateOnVacancy);
        _isLoading = LoadDataCmd.IsExecuting
            .ToProperty(this, x => x.IsLoading);
        LoadDataCmd.Execute().Subscribe();
    }

    private async Task LoadCandidateOnVacancy()
    {
        var itemsExt = Enumerable.Empty<CandidateOnVacancyExt>();
        if (_candidateViewModel != null)
        {
            var items = await _provider.CandidateService.GetCandidateOnVacanciesAsync(_candidateViewModel.CandidateId);
            itemsExt = items.Select(x => new CandidateOnVacancyExt(x));
        }
        else if (_vacancyViewModel != null)
        {
            var items = await _provider.VacancyService.GetCandidateOnVacanciesAsync(_vacancyViewModel.VacancyId);
            itemsExt = items.Select(x => new CandidateOnVacancyExt(x));
        }

        if (itemsExt.Any())
        {
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Source.Clear();
                Source.AddRange(itemsExt);
            });
        }

        this.WhenAnyValue(x => x.SelectedItem)
            .Subscribe(
                x =>
                {
                    if (_provider.Properties != null && x != null)
                    {
                        _provider.Properties.SelectedItem = x;
                    }
                }
            );
    }

    public void Add(CandidateOnVacancy item)
    {
        var id = 0;
        if (Source.Any(x => x.Id == 0))
        {
            id = Source.Min(x => x.Id) - 1;
        }

        item.Id = id;
        var itemExt = new CandidateOnVacancyExt(item);
        Source.Add(itemExt);
        SelectedItem = itemExt;
    }

    public List<CandidateOnVacancy> GetCandidateOnVacancies()
    {
        var retVal = ItemList.Select(x => x.ToCandidateOnVacancy()).ToList();
        var comments = Enumerable.Empty<Comment>();
        if (_vacancyViewModel != null)
        {
            comments = _vacancyViewModel.Comments!.GetAllComments();
        }
        else if (_candidateViewModel != null)
        {
            comments = _candidateViewModel.Comments!.GetAllComments();
        }

        retVal.ForEach(x => x.Comments = comments.Where(c => c.CandidateOnVacancyId == x.Id).ToList());

        return retVal;
    }

    #region Commands
    public ReactiveCommand<Unit, Unit> LoadDataCmd { get; }
    public IReactiveCommand OpenCmd { get; }
    private IReactiveCommand CreateOpenCmd()
    {
        return ReactiveCommand.Create(
            async (CandidateOnVacancyExt item) =>
            {
                if (_vacancyViewModel != null)
                {
                    await _provider.OpenCandidateViewModelAsync(SelectedItem!.CandidateId);
                }
                else if (_candidateViewModel != null)
                {
                    await _provider.OpenVacancyViewModelAsync(SelectedItem!.VacancyId);
                }
            },
            this.WhenAnyValue(x => x.SelectedItem, x => x.ItemList,
                (obj, list) => obj != null && list.Count > 0)
        );
    }
    public IReactiveCommand DeleteCmd { get; }
    private IReactiveCommand CreateDeleteCmd()
    {
        return ReactiveCommand.Create(
            (CandidateOnVacancyExt item) =>
            {
                if (_vacancyViewModel != null)
                {
                    _vacancyViewModel.Comments!.DeleteByCandidateOnVacancyId(item.Id);
                }
                else if (_candidateViewModel != null)
                {
                    _candidateViewModel.Comments!.DeleteByCandidateOnVacancyId(item.Id);
                }

                Source.Remove(item);
            },
            this.WhenAnyValue(x => x.SelectedItem, x => x.ItemList,
                (obj, list) => obj != null && list.Count > 0)
        );
    }
    public IReactiveCommand DeleteKeyDownCmd { get; }
    private IReactiveCommand CreateDeleteKeyDownCmd()
    {
        return ReactiveCommand.Create(
            async (KeyEventArgs args) =>
            {
                if (args.Key == Key.Delete && SelectedItem != null && await DeleteCmd.CanExecute.FirstAsync())
                {
                    await ((ReactiveCommand<CandidateOnVacancyExt, Task>)DeleteCmd).Execute(SelectedItem);
                }
            },
            this.WhenAnyValue(x => x.SelectedItem, x => x.ItemList,
                (obj, list) => obj != null && list.Count > 0)
        );
    }
    #endregion

    #region ItemList
    public ObservableCollectionExtended<CandidateOnVacancyExt> Source;
    private readonly ReadOnlyObservableCollection<CandidateOnVacancyExt> _itemList;
    public ReadOnlyObservableCollection<CandidateOnVacancyExt> ItemList => _itemList;
    #endregion

    #region SelectedItem
    private CandidateOnVacancyExt? _selectedItem;
    public CandidateOnVacancyExt? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    #endregion

    public bool IsLoading => _isLoading.Value;

    public bool CandidateColumnVisible
    {
        get => _vacancyViewModel != null;
    }

    public bool VacancyColumnVisible
    {
        get => _candidateViewModel != null;
    }
}
