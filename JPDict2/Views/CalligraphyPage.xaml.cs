using JPDict2.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace JPDict2.Views;

public sealed partial class CalligraphyPage : Page
{
    public CalligraphyViewModel ViewModel
    {
        get;
    }

    public CalligraphyPage()
    {
        ViewModel = App.GetService<CalligraphyViewModel>();
        InitializeComponent();
    }
}
