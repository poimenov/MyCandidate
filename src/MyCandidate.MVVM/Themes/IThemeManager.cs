using Avalonia;
namespace MyCandidate.MVVM.Themes;

public enum ThemeName
{
    Light,
    Dark
}
public interface IThemeManager
{
    void Initialize(Application application);

    void Switch(ThemeName themeName);
}
