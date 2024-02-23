using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.MVVM.Models;
using MyCandidate.MVVM.Services;
using MyCandidate.MVVM.ViewModels.Candidates;
using MyCandidate.MVVM.ViewModels.Vacancies;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Shared;

public class CommentsViewModel : ViewModelBase
{
    private readonly VacancyViewModel? _vacancyViewModel;
    private readonly CandidateViewModel? _candidateViewModel;
    private readonly IAppServiceProvider _provider;
    private bool _isVacancy;
    public CommentsViewModel(VacancyViewModel vacancyViewModel, IAppServiceProvider appServiceProvider)
    {
        _vacancyViewModel = vacancyViewModel;
        _provider = appServiceProvider;
        _isVacancy = true;

        Source = new ObservableCollectionExtended<CommentExt>(_provider.VacancyService.GetComments(vacancyViewModel.VacancyId).Select(x => new CommentExt(x)));
        Source.ToObservableChangeSet()
            .Filter(VacancyFilter)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        AddCmd = CreateAddCmd();
        DeleteCmd = CreateDeleteCmd();

        LoadComments();

        _vacancyViewModel.WhenAnyValue(x => x.CandidatesOnVacancy.SelectedItem)
            .Subscribe(x =>
            {
                CandidateOnVacancy = x;
            });
    }

    public CommentsViewModel(CandidateViewModel candidateViewModel, IAppServiceProvider appServiceProvider)
    {
        _candidateViewModel = candidateViewModel;
        _provider = appServiceProvider;
        _isVacancy = false;

        Source = new ObservableCollectionExtended<CommentExt>(_provider.CandidateService.GetComments(candidateViewModel.CandidateId).Select(x => new CommentExt(x)));
        Source.ToObservableChangeSet()
            .Filter(CandidateFilter)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        AddCmd = CreateAddCmd();
        DeleteCmd = CreateDeleteCmd();

        LoadComments();

        _candidateViewModel.WhenAnyValue(x => x.CandidatesOnVacancy.SelectedItem)
            .Subscribe(x =>
            {
                CandidateOnVacancy = x;
            });
    }

    public bool IsValid
    {
        get
        {            
            return ItemList.Any() ? !ItemList.Any(x => x.IsValid == false) : true;            
        }
    }

    #region Filter
    private IObservable<Func<CommentExt, bool>>? VacancyFilter =>
        _vacancyViewModel.WhenAnyValue(x => x.CandidatesOnVacancy.SelectedItem)
            .Select((x) => MakeFilter(x));

    private IObservable<Func<CommentExt, bool>>? CandidateFilter =>
        _candidateViewModel.WhenAnyValue(x => x.CandidatesOnVacancy.SelectedItem)
            .Select((x) => MakeFilter(x));

    private Func<CommentExt, bool> MakeFilter(CandidateOnVacancyExt candidateOnVacancy)
    {
        return item =>
        {
            if (candidateOnVacancy != null)
            {
                return item.CandidateOnVacancyId == candidateOnVacancy.Id;
            }

            return false;
        };
    }
    #endregion

    private void LoadComments()
    {
        _itemList.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);
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

    private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(IsValid));
    }

    public void DeleteByCandidateOnVacancyId(int candidateOnVacancyId)
    {
        var idsToDelete = Source.Where(x => x.CandidateOnVacancyId == candidateOnVacancyId).Select(x => x.Id).ToList();
        foreach (var id in idsToDelete)
        {
            Source.Remove(Source.First(x => x.Id == id));
        }
    }

    public List<Comment> GetAllComments()
    {
        return Source.Select(x => x.ToComment()).ToList();
    }

    #region Commands
    public IReactiveCommand AddCmd { get; }
    private IReactiveCommand CreateAddCmd()
    {
        return ReactiveCommand.Create(
            async () =>
            {
                if (CandidateOnVacancy != null)
                {
                    var id = 0;
                    if (Source.Any(x => x.Id == 0))
                    {
                        id = Source.Min(x => x.Id) - 1;
                    }

                    var newComment = new CommentExt
                    {
                        Id = id,
                        CandidateOnVacancy = CandidateOnVacancy,
                        CandidateOnVacancyId = CandidateOnVacancy.Id,
                        CreationDate = DateTime.Now,
                        LastModificationDate = DateTime.Now
                    };
                    newComment.PropertyChanged += ItemPropertyChanged;
                    Source.Add(newComment);
                    SelectedItem = newComment;
                    this.RaisePropertyChanged(nameof(IsValid));
                }
            },
            this.WhenAnyValue(x => x._isVacancy, x => x.CandidateOnVacancy,
                (b, obj) => obj != null)
        );
    }
    public IReactiveCommand DeleteCmd { get; }
    private IReactiveCommand CreateDeleteCmd()
    {
        return ReactiveCommand.Create(
            async (CommentExt item) =>
            {
                Source.Remove(item);
                this.RaisePropertyChanged(nameof(IsValid));
            },
            this.WhenAnyValue(x => x.SelectedItem, x => x.ItemList,
                (obj, list) => obj != null && list.Count > 0)
        );
    }
    #endregion

    #region CandidateOnVacancy
    private CandidateOnVacancyExt? _candidateOnVacancy;
    public CandidateOnVacancyExt? CandidateOnVacancy
    {
        get => _candidateOnVacancy;
        set => this.RaiseAndSetIfChanged(ref _candidateOnVacancy, value);
    }
    #endregion

    #region ItemList
    public ObservableCollectionExtended<CommentExt> Source;
    private readonly ReadOnlyObservableCollection<CommentExt> _itemList;
    public ReadOnlyObservableCollection<CommentExt> ItemList => _itemList;
    #endregion

    #region SelectedItem
    private CommentExt _selectedItem;
    public CommentExt SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    #endregion        
}
