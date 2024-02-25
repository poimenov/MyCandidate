using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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
}
