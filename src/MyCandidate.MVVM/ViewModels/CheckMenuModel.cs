using Avalonia.Controls;
using MyCandidate.Common;
using MyCandidate.MVVM.Converters;

namespace MyCandidate.MVVM.ViewModels;

public class CheckMenuModel : ViewModelBase
{
    private string CheckImageUri => $"{ResourceTypeNameToSvgPathConverter.BASE_PATH}/pngaaa.com-5178883.png";
    protected readonly AppSettings _appSettings;

    public CheckMenuModel(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    protected Image CheckImage
    {
        get
        {
            return GetAssetImage(CheckImageUri);
        }
    }
}
