using Avalonia.Controls;
using MsBox.Avalonia.Enums;
using MyCandidate.MVVM.Extensions;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels;

public class ViewModelBase : ReactiveObject
{
    protected Image GetAssetImage(string uri) => new Image { Source = MessageBoxExtension.GetBitmap(uri) };

    protected void ShowMessageBox(string title, string message, ButtonEnum @enum = ButtonEnum.Ok, Icon icon = Icon.Info)
    {
        var messageBoxStandardWindow = this.GetMessageBox(title, message, @enum, icon);
        messageBoxStandardWindow.ShowAsync();
    }    
}
