using Avalonia;
using MyCandidate.Common;
namespace MyCandidate.MVVM.Themes;


public interface IThemeManager
{
    void Initialize(Application application);

    void Switch(ThemeName themeName, string? paletteName);
}
