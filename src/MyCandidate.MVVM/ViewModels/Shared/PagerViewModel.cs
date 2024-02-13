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
        //CurrentPage = FIRST_PAGE;
        FirstPageCmd = ReactiveCommand.Create(
                    async () =>
                    {
                        CurrentPage = FIRST_PAGE;
                    }, this.WhenAnyValue(x => x.CurrentPage, y => y > FIRST_PAGE)
                );
        PreviousPageCmd = ReactiveCommand.Create(
                    async () =>
                    {
                        CurrentPage = CurrentPage - 1;
                    }, this.WhenAnyValue(x => x.CurrentPage, y => y > FIRST_PAGE)
                );   
        NextPageCmd = ReactiveCommand.Create(
                    async () =>
                    {
                        CurrentPage = CurrentPage + 1;
                    }, this.WhenAnyValue(x => x.CurrentPage, y => y < TotalPages)
                );    
        LastPageCmd = ReactiveCommand.Create(
                    async () =>
                    {
                        CurrentPage = TotalPages;
                    }, this.WhenAnyValue(x => x.CurrentPage, y => y < TotalPages)
                );                                           
    }

    public System.IObservable<IPageRequest> Pager =>
        this.WhenAnyValue(x => x.CurrentPage)
            .Select(x => new PageRequest(x == 0 ? 1 : x, PageSize));    

    public void PagingUpdate(int page, int totalCount, int pages)
    {            
            TotalPages = pages;
            CurrentPage = page;
            TotalItems = totalCount;
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
