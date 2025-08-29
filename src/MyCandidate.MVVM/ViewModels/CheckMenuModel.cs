using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.Options;
using MyCandidate.Common;
using MyCandidate.MVVM.Converters;

namespace MyCandidate.MVVM.ViewModels;

public class CheckMenuModel : ViewModelBase
{
    private App CurrentApplication => (App)Application.Current!;
    private string CheckImageUri => $"{ResourceTypeNameToSvgPathConverter.BASE_PATH}/pngaaa.com-5178883.png";

    public CheckMenuModel()
    {
    }

    protected Image CheckImage
    {
        get
        {
            return GetAssetImage(CheckImageUri);
        }
    }

    protected AppSettings GetAppSettings()
    {
        var options = CurrentApplication.GetRequiredService<IOptions<AppSettings>>();
        return options.Value;
    }
}
