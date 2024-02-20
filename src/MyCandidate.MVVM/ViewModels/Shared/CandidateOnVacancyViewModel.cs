using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.MVVM.Services;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Shared;

public class CandidateOnVacancyViewModel : ViewModelBase
{
    private readonly Candidate _candidate;
    private readonly Vacancy _vacancy;
    private readonly IAppServiceProvider _provider;
    public CandidateOnVacancyViewModel(Candidate candidate, IAppServiceProvider appServiceProvider)
    {
        _candidate = candidate;
        _provider = appServiceProvider;
        Source = new ObservableCollectionExtended<CandidateOnVacancy>(_provider.CandidateService.GetCandidateOnVacancies(candidate.Id));
        Source.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        DeleteCmd = CreateDeleteCmd();
    }

    public CandidateOnVacancyViewModel(Vacancy vacancy, IAppServiceProvider appServiceProvider)
    {
        _vacancy = vacancy;
        _provider = appServiceProvider;
        Source = new ObservableCollectionExtended<CandidateOnVacancy>(_provider.VacancyService.GetCandidateOnVacancies(vacancy.Id));
        Source.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        DeleteCmd = CreateDeleteCmd();
    }

    public void Add(CandidateOnVacancy item)
    {
        Source.Add(item);
    }

    public IReactiveCommand OpenCmd { get; }
    private IReactiveCommand CreateOpenCmd()
    {
        return ReactiveCommand.Create(
            async (CandidateOnVacancy item) =>
            {
                if (CandidateColumnVisible)
                {
                    _provider.OpenDock(_provider.GetCandidateViewModel(SelectedItem.CandidateId));
                }
                else if (VacancyColumnVisible)
                {
                    _provider.OpenDock(_provider.GetVacancyViewModel(SelectedItem.VacancyId));
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
            async (CandidateOnVacancy item) =>
            {
                Source.Remove(item);
            },
            this.WhenAnyValue(x => x.SelectedItem, x => x.ItemList,
                (obj, list) => obj != null && list.Count > 0)
        );
    }

    #region ItemList
    public ObservableCollectionExtended<CandidateOnVacancy> Source;
    private readonly ReadOnlyObservableCollection<CandidateOnVacancy> _itemList;
    public ReadOnlyObservableCollection<CandidateOnVacancy> ItemList => _itemList;
    #endregion

    #region SelectedItem
    private CandidateOnVacancy? _selectedItem;
    public CandidateOnVacancy? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    #endregion  

    public bool CandidateColumnVisible
    {
        get => _candidate == null;
    }

    public bool VacancyColumnVisible
    {
        get => _vacancy == null;
    }
}
