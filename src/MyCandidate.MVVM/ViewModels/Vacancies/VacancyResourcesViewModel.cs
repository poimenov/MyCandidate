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

namespace MyCandidate.MVVM.ViewModels.Vacancies;

public class VacancyResourcesViewModel : ViewModelBase
{
    private readonly Vacancy _vacancy;
    public VacancyResourcesViewModel(Vacancy vacancy, IProperties properties)
    {
        _vacancy = vacancy;
        Properties = properties;
        SourceVacancyResources = new ObservableCollectionExtended<VacancyResourceExt>(_vacancy.VacancyResources.Select(x => new VacancyResourceExt(x)).ToList());
        SourceVacancyResources.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _vacancyResources)
            .Subscribe();

        _vacancyResources.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);

        this.WhenAnyValue(x => x.SelectedVacancyResource)
            .Subscribe(
                x =>
                {
                    if (Properties != null)
                    {
                        Properties.SelectedItem = x;
                    }
                }
            );

        DeleteVacancyResourceCmd = ReactiveCommand.Create(
            async (VacancyResourceExt obj) =>
            {
                SourceVacancyResources.Remove(obj);
            },
            this.WhenAnyValue(x => x.SelectedVacancyResource, x => x.VacancyResources,
                (obj, list) => obj != null && list.Count > 0)              
        );

        CreateVacancyResourceCmd = ReactiveCommand.Create(
            async () =>
            {
                var _newVacancyResource = new VacancyResourceExt()
                {
                    Vacancy = _vacancy,
                    VacancyId = _vacancy.Id
                };
                SourceVacancyResources.Add(_newVacancyResource);
                _newVacancyResource.PropertyChanged += ItemPropertyChanged;
                SelectedVacancyResource = _newVacancyResource;
            }
        );        
    }

    private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(sender is VacancyResourceExt vacancyResource)
        {
            this.RaisePropertyChanged(nameof(VacancyResources));
        }
        this.RaisePropertyChanged(nameof(IsValid));
    }

    public bool IsValid
    {
        get
        {
            foreach(var item in _vacancyResources)
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

    #region VacancyResources
    public ObservableCollectionExtended<VacancyResourceExt> SourceVacancyResources;
    private readonly ReadOnlyObservableCollection<VacancyResourceExt> _vacancyResources;
    public ReadOnlyObservableCollection<VacancyResourceExt> VacancyResources => _vacancyResources;
    #endregion

    #region SelectedVacancyResource
    private VacancyResourceExt? _selectedVacancyResource;
    public VacancyResourceExt? SelectedVacancyResource
    {
        get => _selectedVacancyResource;
        set => this.RaiseAndSetIfChanged(ref _selectedVacancyResource, value);
    }
    #endregion  

    public IReactiveCommand CreateVacancyResourceCmd { get; }
    public IReactiveCommand DeleteVacancyResourceCmd { get; }    
}
