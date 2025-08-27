using System;
using System.Reactive.Disposables;
using Dock.Model.ReactiveUI.Controls;

namespace MyCandidate.MVVM.ViewModels;

public class DocumentBase : Document, IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private bool disposedValue;

    protected CompositeDisposable Disposables => _disposables;

    protected virtual void OnClosed()
    {
        //
    }
    public override bool OnClose()
    {
        OnClosed();
        Dispose();
        return base.OnClose();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _disposables.Dispose();
            }

            // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
            // TODO: установить значение NULL для больших полей
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
