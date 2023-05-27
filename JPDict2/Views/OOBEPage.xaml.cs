using System.Diagnostics;
using JPDict2.Core;
using JPDict2.Helpers;
using JPDict2.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SharpCompress.Common;
using SharpCompress.Readers;
using Windows.Storage;

namespace JPDict2.Views;

public sealed partial class OOBEPage : Page
{
    public OOBEViewModel ViewModel
    {
        get;
    }

    public OOBEPage()
    {
        ViewModel = App.GetService<OOBEViewModel>();
        InitializeComponent();

        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        AppTitleBarText.Text = "AppDisplayName".GetLocalized();

        JmdictDbQueryEngine.Instance.CloseDatabaseConnection();
        KanjiDictQueryEngine.Instance.CloseDatabaseConnection();
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        var resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";

        AppTitleBarText.Foreground = (SolidColorBrush)App.Current.Resources[resource];
        App.AppTitlebar = AppTitleBarText as UIElement;

        AppTitleBar.Margin = new Thickness()
        {
            Left = 24,
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom
        };
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);
    }

    private async void ExtractFiles()
    {
        StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/db.tar.xz"));
        
        using (var stream = await storageFile.OpenReadAsync())
        {
            using (var reader = ReaderFactory.Open(stream.AsStreamForRead()))
            {
                await Task.Run(() =>
                {
                    JmdictDbQueryEngine.Instance.CloseDatabaseConnection();
                    KanjiDictQueryEngine.Instance.CloseDatabaseConnection();
                    reader.EntryExtractionProgress += (sender, args) =>
                    {
                        if (args.ReaderProgress != null)
                        {
                            if (args.Item.Key == "JMDict.db")
                                ViewModel.JmDictUpgradePercentage = args.ReaderProgress.PercentageRead;
                            else if (args.Item.Key == "kanjidict.db")
                                ViewModel.KanjiDictUpgradePercentage = args.ReaderProgress.PercentageRead;
                            else
                                ViewModel.UniDicUpgradePercentage = args.ReaderProgress.PercentageRead;
                        }

                        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                        {
                            Bindings.Update();
                        });
                    };
                    reader.WriteAllToDirectory(ApplicationData.Current.LocalFolder.Path, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                    FinalizeUpgrade();
                });

            }
        }


    }

    private void ContentArea_Loaded(object sender, RoutedEventArgs e)
    {
        ExtractFiles();
        MainDictProgressRing.IsIndeterminate = false;
        KanjiDictProgressRing.IsIndeterminate = false;
        AuxDbProgressRing.IsIndeterminate = false;
    }

    private async void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
    {
        //await ExtractFiles();
    }

    public void FinalizeUpgrade()
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            await ViewModel.LocalSettingsService.SaveSettingAsync("DbMajorVersion", Constants.DbMajorVersion);
            await ViewModel.LocalSettingsService.SaveSettingAsync("DbMinorVersion", Constants.DbMinorVersion);
            App.MainWindow.Content = App.GetService<ShellPage>();
            App.MainWindow.Activate();
            JmdictDbQueryEngine.Instance.InitializeDatabase();
            KanjiDictQueryEngine.Instance.InitializeDatabase();
        });
    }
}
