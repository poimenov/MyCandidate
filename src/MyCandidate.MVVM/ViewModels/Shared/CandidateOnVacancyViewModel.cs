using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.PropertyGrid.Services;
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
    private readonly Candidate _candidate;
    private readonly Vacancy _vacancy;
    private readonly IAppServiceProvider _provider;
    private readonly string _selectedTypeNameKey;
    public CandidateOnVacancyViewModel(Candidate candidate, IAppServiceProvider appServiceProvider)
    {
        _candidate = candidate;
        _provider = appServiceProvider;
        Source = new ObservableCollectionExtended<CandidateOnVacancyExt>(_provider.CandidateService.GetCandidateOnVacancies(candidate.Id).Select(x => new CandidateOnVacancyExt(x)));
        Source.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        DeleteCmd = CreateDeleteCmd();

       LoadCandidateOnVacancy();
    }

    public CandidateOnVacancyViewModel(Vacancy vacancy, IAppServiceProvider appServiceProvider)
    {
        _vacancy = vacancy;
        _provider = appServiceProvider;
        Source = new ObservableCollectionExtended<CandidateOnVacancyExt>(_provider.VacancyService.GetCandidateOnVacancies(vacancy.Id).Select(x => new CandidateOnVacancyExt(x)));
        Source.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _itemList)
            .Subscribe();

        OpenCmd = CreateOpenCmd();
        DeleteCmd = CreateDeleteCmd();

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
        Source.Add(new CandidateOnVacancyExt(item));
    }

    public IReactiveCommand OpenCmd { get; }
    private IReactiveCommand CreateOpenCmd()
    {
        return ReactiveCommand.Create(
            async (CandidateOnVacancyExt item) =>
            {
                if (CandidateColumnVisible)
                {
                    var existed = _provider.Documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(CandidateViewModel) && ((CandidateViewModel)x).CandidateId == SelectedItem.CandidateId);
                    if(existed != null)
                    {
                        _provider.Factory.SetActiveDockable(existed);
                    }
                    else
                    {
                        _provider.OpenDock(_provider.GetCandidateViewModel(SelectedItem.CandidateId));
                    }                                                         
                }
                else if (VacancyColumnVisible)
                {
                    var existed = _provider.Documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(VacancyViewModel) && ((VacancyViewModel)x).VacancyId == SelectedItem.VacancyId);
                    if(existed != null)
                    {
                        _provider.Factory.SetActiveDockable(existed);
                    }
                    else
                    {
                        _provider.OpenDock(_provider.GetVacancyViewModel(SelectedItem.VacancyId));
                    }                                         
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
                Source.Remove(item);
            },
            this.WhenAnyValue(x => x.SelectedItem, x => x.ItemList,
                (obj, list) => obj != null && list.Count > 0)
        );
    }

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
        get => _candidate == null;
    }

    public bool VacancyColumnVisible
    {
        get => _vacancy == null;
    }
}
