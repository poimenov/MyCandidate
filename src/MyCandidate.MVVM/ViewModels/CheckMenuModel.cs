using Avalonia.Controls;
using MyCandidate.Common;
using System;
using System.IO;
using System.Text.Json;

namespace MyCandidate.MVVM.ViewModels;

public class CheckMenuModel : ViewModelBase
{
    private const string CHECK_IMAGE_URI = "avares://MyCandidate.MVVM/Assets/pngaaa.com-5178883.png";    
    protected readonly AppSettings _appSettings;

    public CheckMenuModel(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    protected Image CheckImage
    {
        get
        {
            return GetAssetImage(CHECK_IMAGE_URI);
        }
    }
}
