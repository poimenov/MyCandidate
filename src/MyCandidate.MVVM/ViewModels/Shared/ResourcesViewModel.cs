using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Input;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.MVVM.Models;
using MyCandidate.MVVM.ViewModels.Tools;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Shared;

public class ResourcesViewModel : ViewModelBase
{
    private readonly ResourceModelType _resourceModelType;
    public ResourcesViewModel(Vacancy vacancy, IProperties properties)
    {
        _resourceModelType = ResourceModelType.VacancyResource;
        Properties = properties;
        SourceResources = new ObservableCollectionExtended<ResourceModel>(vacancy.VacancyResources.Select(x => new ResourceModel(x)).ToList());
        SourceResources.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _resources)
            .Subscribe();

        LoadResources();

        DeleteResourceCmd = CreateDeleteResourceCmd();
        DeleteResourceKeyDownCmd = CreateDeleteResourceKeyDownCmd();
        CreateResourceCmd = CreateCreateResourceCmd();
    }

    public ResourcesViewModel(Candidate candidate, IProperties properties)
    {
        _resourceModelType = ResourceModelType.CandidateResource;
        Properties = properties;
        SourceResources = new ObservableCollectionExtended<ResourceModel>(candidate.CandidateResources.Select(x => new ResourceModel(x)).ToList());
        SourceResources.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _resources)
            .Subscribe();

        LoadResources();

        DeleteResourceCmd = CreateDeleteResourceCmd();
        DeleteResourceKeyDownCmd = CreateDeleteResourceKeyDownCmd();
        CreateResourceCmd = CreateCreateResourceCmd();
    }    

    private void LoadResources()
    {
        _resources.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);

        this.WhenAnyValue(x => x.SelectedResource)
            .Subscribe(
                x =>
                {
                    if (Properties != null)
                    {
                        Properties.SelectedItem = x;
                    }
                }
            );
    }

    private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is ResourceModel resource)
        {
            this.RaisePropertyChanged(nameof(Resources));
        }
        this.RaisePropertyChanged(nameof(IsValid));
    }

    public bool IsValid
    {
        get
        {
            foreach (var item in _resources)
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

    #region Resources
    public ObservableCollectionExtended<ResourceModel> SourceResources;
    private readonly ReadOnlyObservableCollection<ResourceModel> _resources;
    public ReadOnlyObservableCollection<ResourceModel> Resources => _resources;
    #endregion

    #region SelectedResource
    private ResourceModel? _selectedResource;
    public ResourceModel? SelectedResource
    {
        get => _selectedResource;
        set => this.RaiseAndSetIfChanged(ref _selectedResource, value);
    }
    #endregion

    #region Commands
    public IReactiveCommand CreateResourceCmd { get; }
    private IReactiveCommand CreateCreateResourceCmd()
    {
        return ReactiveCommand.Create(
            async () =>
            {
                var _newResource = new ResourceModel(_resourceModelType);
                SourceResources.Add(_newResource);
                _newResource.PropertyChanged += ItemPropertyChanged;
                SelectedResource = _newResource;
            });
    }
    public IReactiveCommand DeleteResourceCmd { get; }
    private IReactiveCommand CreateDeleteResourceCmd()
    {
        return ReactiveCommand.Create(
            async (ResourceModel obj) =>
            {
                SourceResources.Remove(obj);
            },
            this.WhenAnyValue(x => x.SelectedResource, x => x.Resources,
                (obj, list) => obj != null && list.Count > 0)
        );
    }
    public IReactiveCommand DeleteResourceKeyDownCmd { get; }
    private IReactiveCommand CreateDeleteResourceKeyDownCmd()
    {
        return ReactiveCommand.Create(
            async (KeyEventArgs args) =>
            {
                if (args.Key == Key.Delete && SelectedResource != null)
                {
                    SourceResources.Remove(SelectedResource);
                }
            },
            this.WhenAnyValue(x => x.SelectedResource, x => x.Resources,
                (obj, list) => obj != null && list.Count > 0)
        );
    }
    #endregion
}
