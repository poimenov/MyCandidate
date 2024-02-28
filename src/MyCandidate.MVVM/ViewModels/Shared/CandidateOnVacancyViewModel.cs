using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    private readonly CandidateViewModel _candidateViewModel;
    private readonly VacancyViewModel _vacancyViewModel;
    private readonly IAppServiceProvider _provider;
    private bool _isVacancy;
    public CandidateOnVacancyViewModel(CandidateViewModel candidateViewModel, IAppServiceProvider appServiceProvider)
    {
        _candidateViewModel = candidateViewModel;
        _provider = appServiceProvider;
        _isVacancy = false;
        Source = new ObservableCollectionExtended<CandidateOnVacancyExt>(_provider.CandidateService.GetCandidateOnVacancies(candidateViewModel.CandidateId).Select(x => new CandidateOnVacancyExt(x)));
        Source.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        DeleteCmd = CreateDeleteCmd();
        DeleteKeyDownCmd = CreateDeleteKeyDownCmd();

        LoadCandidateOnVacancy();
    }

    public CandidateOnVacancyViewModel(VacancyViewModel vacancyViewModel, IAppServiceProvider appServiceProvider)
    {
        _vacancyViewModel = vacancyViewModel;
        _provider = appServiceProvider;
        _isVacancy = true;
        Source = new ObservableCollectionExtended<CandidateOnVacancyExt>(_provider.VacancyService.GetCandidateOnVacancies(vacancyViewModel.VacancyId).Select(x => new CandidateOnVacancyExt(x)));
        Source.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        DeleteCmd = CreateDeleteCmd();
        DeleteKeyDownCmd = CreateDeleteKeyDownCmd();

        LoadCandidateOnVacancy();
    }

    private void LoadCandidateOnVacancy()
    {
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
        List<Comment> comments;
        if (_isVacancy)
        {
            comments = _vacancyViewModel.Comments.GetAllComments();
        }
        else
        {
            comments = _candidateViewModel.Comments.GetAllComments();
        }

        retVal.ForEach(x => x.Comments = comments.Where(c => c.CandidateOnVacancyId == x.Id).ToList());

        return retVal;
    }

    #region Commands
    public IReactiveCommand OpenCmd { get; }
    private IReactiveCommand CreateOpenCmd()
    {
        return ReactiveCommand.Create(
            async (CandidateOnVacancyExt item) =>
            {
                if (_isVacancy)
                {
                    _provider.OpenCandidateViewModel(SelectedItem.CandidateId);
                }
                else
                {
                    _provider.OpenVacancyViewModel(SelectedItem.VacancyId);
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
            async (CandidateOnVacancyExt item) =>
            {
                if (_isVacancy)
                {
                    _vacancyViewModel.Comments.DeleteByCandidateOnVacancyId(item.Id);
                }
                else
                {
                    _candidateViewModel.Comments.DeleteByCandidateOnVacancyId(item.Id);
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
    private CandidateOnVacancyExt _selectedItem;
    public CandidateOnVacancyExt SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    #endregion

    public bool CandidateColumnVisible
    {
        get => _isVacancy;
    }

    public bool VacancyColumnVisible
    {
        get => !_isVacancy;
    }
}
