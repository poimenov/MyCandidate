using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.PropertyGrid.Services;
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
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        SourceCandidateResources = new ObservableCollectionExtended<CandidateResourceExt>(_candidate.CandidateResources.Select(x => new CandidateResourceExt(x)).ToList());
        SourceCandidateResources.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _candidateResources)
            .Subscribe();

        this.WhenAnyValue(x => x.SelectedCandidateResource)
            .Subscribe(
                x =>
                {
                    if (Properties != null)
                    {
                        Properties.SelectedItem = x;
                        Properties.SelectedTypeName = LocalizationService.Default["CandidateResource"]; ;
                    }
                }
            );

        DeleteCandidateResourceCmd = ReactiveCommand.Create(
            (object obj) =>
            {
                SourceCandidateResources.Remove((CandidateResourceExt)obj);
            }
        );

        CreateCandidateResourceCmd = ReactiveCommand.Create(
            () =>
            {
                var _newCandidateResource = new CandidateResourceExt()
                {
                    Candidate = _candidate,
                    CandidateId = _candidate.Id
                };
                SourceCandidateResources.Add(_newCandidateResource);
                SelectedCandidateResource = _newCandidateResource;
            }
        );
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        if (Properties != null)
        {
            Properties.SelectedTypeName = LocalizationService.Default["CandidateResource"];
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
