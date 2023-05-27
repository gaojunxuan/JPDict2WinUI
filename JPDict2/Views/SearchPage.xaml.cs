using System.Diagnostics;
using System.Reactive.Linq;
using CommunityToolkit.WinUI.UI.Controls;
using JPDict2.Core;
using JPDict2.NLP;
using JPDict2.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using NMeCab;
using NMeCab.Specialized;
using Windows.Data.Text;
using Windows.Storage;

namespace JPDict2.Views;

public sealed partial class SearchPage : Page
{
    public SearchViewModel ViewModel
    {
        get;
    }

    public SearchPage()
    {
        ViewModel = App.GetService<SearchViewModel>();
        InitializeComponent();
    }

    private void OnViewStateChanged(object sender, ListDetailsViewState e)
    {
        if (e == ListDetailsViewState.Both)
        {
            ViewModel.EnsureItemSelected();
        }
    }

    private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void SearchBox_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(h => SearchBox.TextChanged += h, h => SearchBox.TextChanged -= h)
                  .Throttle(TimeSpan.FromMilliseconds(300))
                  .Subscribe(_ =>
                  {
                      DispatcherQueue.TryEnqueue(() =>
                      {
                          ViewModel.UpdateSearchResults(SearchBox.Text);
                          ViewModel.UpdateSegmentedInput(SearchBox.Text);
                          StartLemmatizerPanelAnimation();
                          Bindings.Update();
                          
                      });
                  });
    }

    private void ListDetailsViewControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void SegmentedElementButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender != null && sender is HyperlinkButton)
        {
            if ((sender as HyperlinkButton).Tag.ToString() != "*" && !string.IsNullOrWhiteSpace((sender as HyperlinkButton).Tag.ToString()))
                this.SearchBox.Text = (sender as HyperlinkButton).Tag.ToString();
        }
    }

    public void StartLemmatizerPanelAnimation()
    {
        if (LemmatizerPanel.Visibility == Microsoft.UI.Xaml.Visibility.Collapsed)
            EaseInAnimation.Begin();
    }
}
