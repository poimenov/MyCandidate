using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Dock.Model.ReactiveUI.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MyCandidate.MVVM.Converters;
using MyCandidate.MVVM.ViewModels;

namespace MyCandidate.MVVM.Extensions;

public static class MessageBoxExtension
{
    private static string LogoPath => $"{ResourceTypeNameToSvgPathConverter.BASE_PATH}/avalonia-logo.ico";
    public static Bitmap GetBitmap(string uri) => new Bitmap(AssetLoader.Open(new System.Uri(uri)));
    public static WindowIcon AppLogoIcon => new WindowIcon(GetBitmap(LogoPath));
    public static IMsBox<ButtonResult> GetMessageBox(this Document obj, string title, string message, ButtonEnum @enum, Icon icon)
    {
        return GetMessageBox(title, message, @enum, icon);
    }

    public static IMsBox<ButtonResult> GetMessageBox(this ViewModelBase obj, string title, string message, ButtonEnum @enum, Icon icon)
    {
        return GetMessageBox(title, message, @enum, icon);
    }    

    public static IMsBox<ButtonResult> GetMessageBox(string title, string message, ButtonEnum @enum, Icon icon)
    {
        var standardParams = new MessageBoxStandardParams
        {

            WindowIcon = AppLogoIcon,
            ContentTitle = title,
            ContentMessage = message,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ShowInCenter = true,
            ButtonDefinitions = @enum,
            Icon = icon
        };

        return MessageBoxManager.GetMessageBoxStandard(standardParams);
    }    
}
