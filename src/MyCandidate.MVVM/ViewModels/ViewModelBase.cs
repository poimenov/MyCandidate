using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels;

public class ViewModelBase : ReactiveObject
{
    protected Image GetAssetImage(string uri)
    {
        return new Image
        {
            Source = new Bitmap(AssetLoader.Open(new System.Uri(uri)))
        };
    }

    private void ShowErrorMessageBox(Exception ex)
    {
        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(
            "Error", ex.Message, ButtonEnum.Ok, Icon.Error);       
        messageBoxStandardWindow.ShowAsync();
    }     
}
