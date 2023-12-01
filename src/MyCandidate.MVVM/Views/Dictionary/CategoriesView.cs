using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MyCandidate.MVVM.Views.Dictionary;

public partial class CategoriesView : UserControl
{
    public CategoriesView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

