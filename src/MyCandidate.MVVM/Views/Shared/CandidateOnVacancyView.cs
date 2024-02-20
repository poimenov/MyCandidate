using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MyCandidate.MVVM.Views.Shared;

public partial class CandidateOnVacancyView : UserControl
{
    public CandidateOnVacancyView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

