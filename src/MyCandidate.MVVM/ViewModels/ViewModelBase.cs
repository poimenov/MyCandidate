using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels;

public class ViewModelBase : ReactiveObject
{
    public const string LOGO_PATH = "avares://MyCandidate.MVVM/Assets/avalonia-logo.ico";
    protected Image GetAssetImage(string uri) => new Image { Source = GetBitmap(uri) };

    protected Bitmap GetBitmap(string uri) => new Bitmap(AssetLoader.Open(new System.Uri(uri)));

    protected WindowIcon AppLogoIcon => new WindowIcon(GetBitmap(LOGO_PATH));

    protected void ShowMessageBox(string title, string message)
    {
        var standardParams = new MessageBoxStandardParams
        {

            WindowIcon = AppLogoIcon,
            ContentTitle = title,
            ContentMessage = message,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ShowInCenter = true,
            ButtonDefinitions = ButtonEnum.Ok,
            Icon = Icon.Info
        };
        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(standardParams);
        messageBoxStandardWindow.ShowAsync();
    }    
}
