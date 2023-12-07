using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MyCandidate.MVVM.Views.Dictionary;

public partial class CompaniesView : UserControl
{
    public CompaniesView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

