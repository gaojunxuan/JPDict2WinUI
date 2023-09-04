using JPDict2.ViewModels;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Input.Inking;
using Windows.UI.Core;
using System.Windows.Ink;
using System.Windows.Input;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

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

    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ContentDialog contentDialog = new ContentDialog();
        contentDialog.XamlRoot = this.XamlRoot;
        contentDialog.Title = "Writing Practice";
        contentDialog.PrimaryButtonText = "Done";
        contentDialog.Content = new CalligraphyFlyout();
        contentDialog.DefaultButton = ContentDialogButton.Primary;
        contentDialog.RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as Grid).ActualTheme;
        await contentDialog.ShowAsync();
    }
}
