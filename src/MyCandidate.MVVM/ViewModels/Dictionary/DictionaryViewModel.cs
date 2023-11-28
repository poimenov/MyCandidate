using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Dock.Model.ReactiveUI.Controls;
using DynamicData;
using DynamicData.Binding;
using log4net;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.Extensions;
using MyCandidate.MVVM.Services;
using MyCandidate.MVVM.ViewModels.Tools;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace MyCandidate.MVVM.ViewModels.Dictionary;

public abstract class DictionaryViewModel<T> : Document where T : Entity, new()
{
    protected readonly ILog _log;
    protected readonly IDictionaryService<T>? _service;
    protected List<int> _deletedIds;
    protected List<int> _updatedIds;
    protected virtual IObservable<Func<T, bool>>? Filter =>
        this.WhenAnyValue(x => x.Enabled, x => x.Name)
            .Select((x) => MakeFilter(x.Item1, x.Item2));

    public DictionaryViewModel(IDictionaryService<T> service, ILog log)
    {
        _service = service;
        _log = log;
        SelectedTypeName = string.Empty;
        _deletedIds = new List<int>();
        _updatedIds = new List<int>();

        Source = new ObservableCollectionExtended<T>(_service!.ItemsList);
        Source.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter)
            .Bind(out _itemList)
            .Subscribe();


        _itemList.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);

        this.WhenAnyValue(x => x.SelectedItem)
            .Subscribe(
                x =>
                {
                    if (Properties != null)
                    {
                        Properties.SelectedItem = x;
                        Properties.SelectedTypeName = SelectedTypeName;
                    }
                }
            );

        CreateCmd = ReactiveCommand.Create(
            () =>
            {
                var _newT = new T()
                {
                    Enabled = true
                };
                _newT.PropertyChanged += ItemPropertyChanged;
                Source.Add(_newT);
                SelectedItem = _newT;
                this.RaisePropertyChanged(nameof(IsValid));
            }
        );

        CancelCmd = ReactiveCommand.Create(() => { OnCancel(); });

        DeleteCmd = ReactiveCommand.Create(
            (object obj) =>
            {
                T item = (T)obj;
                if (item.Id > 0)
                {
                    _deletedIds.Add(item.Id);
                }

                if (_updatedIds.Any(x => x == item.Id))
                {
                    _updatedIds.Remove(item.Id);
                }

                Source.Remove((T)obj);
                this.RaisePropertyChanged(nameof(IsValid));
            },
            this.WhenAnyValue(x => x.SelectedItem, x => x.ItemList,
                (obj, list) => obj != null && obj.Enabled && list.Count > 0)
        );

        SaveCmd = ReactiveCommand.Create(
            (object obj) =>
            {
                if (IsValid)
                {
                    try
                    {
                        _service.Delete(_deletedIds);
                        var newItems = ItemList.Where(x => x.Id == 0).ToList();
                        _service.Create(newItems);
                        var updatedItems = ItemList.Where(x => _updatedIds.Contains(x.Id)).ToList();
                        _service.Update(updatedItems);
                        
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                        ShowErrorMessageBox(ex);
                        throw;
                    }

                    OnCancel();
                }

            },
            this.WhenAnyValue(x => x.IsValid, v => v == true)
        );
    }

    public bool IsValid
    {
        get
        {
            foreach (var item in ItemList)
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

    #region ItemList
    public ObservableCollectionExtended<T> Source;
    private readonly ReadOnlyObservableCollection<T> _itemList;
    public ReadOnlyObservableCollection<T> ItemList => _itemList;
    #endregion

    #region SelectedItem
    private T? _selectedItem;
    public T? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    #endregion

    #region SelectedTypeName
    private string _selectedTypeName;
    protected string SelectedTypeName
    {
        get => _selectedTypeName;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedTypeName, value);
            if (Properties != null)
            {
                Properties.SelectedTypeName = SelectedTypeName;
            }
        }
    }
    #endregion

    #region Enabled
    private bool? _enabled;
    public bool? Enabled
    {
        get => _enabled;
        set => this.RaiseAndSetIfChanged(ref _enabled, value);
    }
    #endregion

    #region Name
    private string _name;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    #endregion

    public IReactiveCommand SaveCmd { get; }
    public IReactiveCommand CancelCmd { get; }
    public IReactiveCommand CreateCmd { get; }
    public IReactiveCommand DeleteCmd { get; }

    private Func<T, bool> MakeFilter(bool? enabled, string name)
    {
        return item =>
        {
            var byName = true;
            if (!string.IsNullOrEmpty(name) && name.Length > 2)
            {
                byName = item.Name.StartsWith(name, true, CultureInfo.InvariantCulture);
            }

            if (enabled.HasValue)
            {
                return item.Enabled == enabled && byName;
            }
            else
            {
                return byName;
            }
        };
    }

    private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender != null
            && sender is T entity
            && entity.Id != 0
            && !_updatedIds.Any(x => x == entity.Id))
        {
            _updatedIds.Add(entity.Id);
        }

        this.RaisePropertyChanged(nameof(IsValid));
    }

    private void OnCancel()
    {
        Source.Load(_service!.ItemsList);
        if (ItemList.Count() > 0)
        {
            _itemList.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);
            SelectedItem = ItemList.First();
        }
        this._deletedIds = new List<int>();
        this._updatedIds = new List<int>();
        this.RaisePropertyChanged(nameof(IsValid));
    }

    private void ShowErrorMessageBox(Exception ex)
    {
        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(
            "Error", ex.Message, ButtonEnum.Ok, Icon.Error);       
        messageBoxStandardWindow.ShowAsync();
    }    
}
