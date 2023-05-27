using CommunityToolkit.WinUI.UI.Controls;

using JPDict2.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace JPDict2.Views;

public sealed partial class FlashcardPage : Page
{
    public FlashcardViewModel ViewModel
    {
        get;
    }

    public FlashcardPage()
    {
        ViewModel = App.GetService<FlashcardViewModel>();
        InitializeComponent();
    }

    private void OnViewStateChanged(object sender, ListDetailsViewState e)
    {
        if (e == ListDetailsViewState.Both)
        {
            ViewModel.EnsureItemSelected();
        }
    }
}
