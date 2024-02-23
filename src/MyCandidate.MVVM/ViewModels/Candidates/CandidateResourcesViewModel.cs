using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.MVVM.Models;
using MyCandidate.MVVM.ViewModels.Tools;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Candidates;

public class CandidateResourcesViewModel : ViewModelBase
{
    private readonly Candidate _candidate;
    public CandidateResourcesViewModel(Candidate candidate, IProperties properties)
    {
        _candidate = candidate;
        Properties = properties;
        SourceCandidateResources = new ObservableCollectionExtended<CandidateResourceExt>(_candidate.CandidateResources.Select(x => new CandidateResourceExt(x)).ToList());
        SourceCandidateResources.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _candidateResources)
            .Subscribe();

        _candidateResources.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);

        this.WhenAnyValue(x => x.SelectedCandidateResource)
            .Subscribe(
                x =>
                {
                    if (Properties != null)
                    {
                        Properties.SelectedItem = x;
                    }
                }
            );

        DeleteCandidateResourceCmd = ReactiveCommand.Create(
            async (CandidateResourceExt obj) =>
            {
                SourceCandidateResources.Remove(obj);
            },
            this.WhenAnyValue(x => x.SelectedCandidateResource, x => x.CandidateResources,
                (obj, list) => obj != null && list.Count > 0)             
        );

        CreateCandidateResourceCmd = ReactiveCommand.Create(
            async () =>
            {
                var _newCandidateResource = new CandidateResourceExt()
                {
                    Candidate = _candidate,
                    CandidateId = _candidate.Id
                };
                SourceCandidateResources.Add(_newCandidateResource);
                _newCandidateResource.PropertyChanged += ItemPropertyChanged;
                SelectedCandidateResource = _newCandidateResource;
            }
        );
    }

    private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(sender is CandidateResourceExt candidateResource)
        {
            this.RaisePropertyChanged(nameof(CandidateResources));
        }
        this.RaisePropertyChanged(nameof(IsValid));
    }

    public bool IsValid
    {
        get
        {
            foreach(var item in _candidateResources)
            {
                if (!item.IsValid())
                {
                    return false;
                }                
            }
            return true;
        }
    }     

    public IProperties? Properties { get; set; }

    #region CandidateResources
    public ObservableCollectionExtended<CandidateResourceExt> SourceCandidateResources;
    private readonly ReadOnlyObservableCollection<CandidateResourceExt> _candidateResources;
    public ReadOnlyObservableCollection<CandidateResourceExt> CandidateResources => _candidateResources;
    #endregion

    #region SelectedCandidateResource
    private CandidateResourceExt? _selectedCandidateResource;
    public CandidateResourceExt? SelectedCandidateResource
    {
        get => _selectedCandidateResource;
        set => this.RaiseAndSetIfChanged(ref _selectedCandidateResource, value);
    }
    #endregion  

    public IReactiveCommand CreateCandidateResourceCmd { get; }
    public IReactiveCommand DeleteCandidateResourceCmd { get; }
}
