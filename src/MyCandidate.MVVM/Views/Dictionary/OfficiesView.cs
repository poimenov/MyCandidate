using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MyCandidate.MVVM.Views.Dictionary;

public partial class OfficiesView : UserControl
{
    public OfficiesView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

