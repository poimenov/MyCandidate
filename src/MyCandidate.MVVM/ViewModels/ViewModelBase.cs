using System;
using System.Reactive.Disposables;
using Avalonia.Controls;
using MsBox.Avalonia.Enums;
using MyCandidate.MVVM.Extensions;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels;

public class ViewModelBase : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private bool disposedValue;
    protected CompositeDisposable Disposables => _disposables;

    protected Image GetAssetImage(string uri) => new Image { Source = MessageBoxExtension.GetBitmap(uri) };

    protected void ShowMessageBox(string title, string message, ButtonEnum @enum = ButtonEnum.Ok, Icon icon = Icon.Info)
    {
        var messageBoxStandardWindow = this.GetMessageBox(title, message, @enum, icon);
        messageBoxStandardWindow.ShowAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _disposables.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
