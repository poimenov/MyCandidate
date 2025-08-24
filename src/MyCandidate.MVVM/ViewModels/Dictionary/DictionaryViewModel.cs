using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.PropertyGrid.Services;
using Dock.Model.ReactiveUI.Controls;
using DynamicData;
using DynamicData.Binding;
using log4net;
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
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    protected List<int> _deletedIds;
    protected List<int> _updatedIds;
    protected virtual IObservable<Func<T, bool>>? Filter =>
        this.WhenAnyValue(x => x.Enabled, x => x.Name)
            .Select((x) => MakeFilter(x.Item1, x.Item2));

    public DictionaryViewModel(IDictionaryService<T> service, ILog log)
    {
        _service = service;
        _log = log;
        _deletedIds = new List<int>();
        _updatedIds = new List<int>();
        _name = string.Empty;

        Source = new ObservableCollectionExtended<T>();
        Source.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter ?? Observable.Return<Func<T, bool>>(x => true))
            .Bind(out _itemList)
            .Subscribe();

        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
        _isLoading = LoadDataCommand.IsExecuting
            .ToProperty(this, x => x.IsLoading);
        LoadDataCommand.Execute().Subscribe();

        this.WhenAnyValue(x => x.SelectedItem)
            .Subscribe(
                x =>
                {
                    if (Properties != null)
                    {
                        Properties.SelectedItem = x;
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

        CancelCmd = ReactiveCommand.Create(async () => { await OnCancel(); });

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
            async (object obj) =>
            {
                if (IsValid)
                {
                    var dialog = this.GetMessageBox(LocalizationService.Default["Save"],
                                                        LocalizationService.Default["Save_Text"],
                                                        ButtonEnum.YesNo, Icon.Question);
                    var result = await dialog.ShowAsync();
                    if (result == ButtonResult.No)
                    {
                        return;
                    }

                    var operationResult = await _service.DeleteAsync(_deletedIds);
                    if (!operationResult.Success)
                    {
                        await ShowErrorMessageBox(operationResult.Message ?? string.Empty);
                        return;
                    }

                    var newItems = ItemList.Where(x => x.Id == 0).ToList();
                    operationResult = await _service.CreateAsync(newItems);
                    if (!operationResult.Success)
                    {
                        await ShowErrorMessageBox(operationResult.Message ?? string.Empty);
                        return;
                    }

                    var updatedItems = ItemList.Where(x => _updatedIds.Contains(x.Id)).ToList();
                    operationResult = await _service.UpdateAsync(updatedItems);
                    if (!operationResult.Success)
                    {
                        await ShowErrorMessageBox(operationResult.Message ?? string.Empty);
                        return;
                    }

                    await OnCancel();
                }

            },
            this.WhenAnyValue(x => x.IsValid, v => v == true)
        );
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var items = await _service!.GetItemsListAsync();
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Source.Clear();
                Source.AddRange(items);
                _itemList.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);
            });
        }
        catch (Exception ex)
        {
            // Обработка ошибок
            _log.Error("Ошибка загрузки данных", ex);
        }
    }

    public virtual bool IsValid
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

    public bool IsLoading => _isLoading.Value;

    public IReactiveCommand SaveCmd { get; }
    public IReactiveCommand CancelCmd { get; }
    public IReactiveCommand CreateCmd { get; }
    public IReactiveCommand DeleteCmd { get; }
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

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

    private async Task OnCancel()
    {
        Source = new ObservableCollectionExtended<T>(await _service!.GetItemsListAsync());
        if (ItemList.Count() > 0)
        {
            _itemList.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);
            SelectedItem = ItemList.First();
        }
        this._deletedIds = new List<int>();
        this._updatedIds = new List<int>();
        this.RaisePropertyChanged(nameof(IsValid));
    }

    private async Task ShowErrorMessageBox(string message)
    {
        var messageBoxStandardWindow = this.GetMessageBox(LocalizationService.Default["Error"],
                                                            message, ButtonEnum.Ok, Icon.Error);
        await messageBoxStandardWindow.ShowAsync();
    }
}
