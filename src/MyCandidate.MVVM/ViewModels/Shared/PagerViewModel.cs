using System;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Shared;

public class PagerViewModel : ViewModelBase
{
    private const int PAGE_SIZE = 25;
    private const int FIRST_PAGE = 1;

    public PagerViewModel()
    {
        FirstPageCmd = ReactiveCommand.Create(
                    () =>
                    {
                        CurrentPage = FIRST_PAGE;
                    }, this.WhenAnyValue(x => x.PreviousPageEnabled, y => y == true)
                );
        PreviousPageCmd = ReactiveCommand.Create(
                    () =>
                    {
                        CurrentPage = CurrentPage - 1;
                    }, this.WhenAnyValue(x => x.PreviousPageEnabled, y => y == true)
                );
        NextPageCmd = ReactiveCommand.Create(
                    () =>
                    {
                        CurrentPage = CurrentPage + 1;
                    }, this.WhenAnyValue(x => x.NextPageEnabled, y => y == true)
                );
        LastPageCmd = ReactiveCommand.Create(
                    () =>
                    {
                        CurrentPage = TotalPages;
                    }, this.WhenAnyValue(x => x.NextPageEnabled, y => y == true)
                );
    }

    private bool _previousPageEnabled;
    public bool PreviousPageEnabled
    {
        get => _previousPageEnabled;
        set => this.RaiseAndSetIfChanged(ref _previousPageEnabled, value);
    }

    private bool _nextPageEnabled;
    public bool NextPageEnabled
    {
        get => _nextPageEnabled;
        set => this.RaiseAndSetIfChanged(ref _nextPageEnabled, value);
    }           

    public System.IObservable<IPageRequest> Pager =>
        this.WhenAnyValue(x => x.CurrentPage)
            .Select(x => new PageRequest(x == 0 ? 1 : x, PageSize));

    public void PagingUpdate(int page, int totalCount, int pages)
    {
        TotalPages = pages;
        CurrentPage = page;
        TotalItems = totalCount;
        PreviousPageEnabled = CurrentPage > FIRST_PAGE;
        NextPageEnabled = CurrentPage < TotalItems;
    }

    public void PagingUpdate(int totalCount)
    {        
        TotalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalCount) / PAGE_SIZE));
        CurrentPage = 1;
        TotalItems = totalCount;
        PreviousPageEnabled = CurrentPage > FIRST_PAGE;
        NextPageEnabled = CurrentPage < TotalItems;        
    }

    public IReactiveCommand FirstPageCmd { get; }
    public IReactiveCommand PreviousPageCmd { get; }
    public IReactiveCommand NextPageCmd { get; }
    public IReactiveCommand LastPageCmd { get; }

    private int _totalItems;
    public int TotalItems
    {
        get => _totalItems;
        set => this.RaiseAndSetIfChanged(ref _totalItems, value);
    }

    private int _currentPage;
    public int CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    private int _totalPages;
    public int TotalPages
    {
        get => _totalPages;
        set => this.RaiseAndSetIfChanged(ref _totalPages, value);
    }

    public int PageSize => PAGE_SIZE;
}
