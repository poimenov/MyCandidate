using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MyCandidate.MVVM.Views.Shared;

public partial class CommentsView : UserControl
{
    public CommentsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

