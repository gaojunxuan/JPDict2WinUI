using JPDict2.Activation;
using JPDict2.Contracts.Services;
using JPDict2.Core;
using JPDict2.Views;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace JPDict2.Services;

public class ActivationService : IActivationService
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ILocalSettingsService _localSettingsService;
    private UIElement? _shell = null;

    public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, IEnumerable<IActivationHandler> activationHandlers, IThemeSelectorService themeSelectorService, ILocalSettingsService localSettingsService)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
        _themeSelectorService = themeSelectorService;
        _localSettingsService = localSettingsService;
    }

    public async Task ActivateAsync(object activationArgs)
    {
        // Execute tasks before activation.
        await InitializeAsync();

        // Set the MainWindow Content.
        if (App.MainWindow.Content == null)
        {
            if (_localSettingsService != null)
            {
                var localSettingsService = _localSettingsService;
                int majorVer = await localSettingsService.ReadSettingAsync<int>("DbMajorVersion");
                int minorVer = await localSettingsService.ReadSettingAsync<int>("DbMinorVersion");
                if ((majorVer == default && minorVer == default) || (majorVer < Constants.DbMajorVersion) || (majorVer == Constants.DbMajorVersion && minorVer < Constants.DbMinorVersion))
                {
                    _shell = App.GetService<OOBEPage>();
                }
                else
                {
                    _shell = App.GetService<ShellPage>();
                }
            }
            else
            {
                _shell = App.GetService<ShellPage>();
            }
            App.MainWindow.Content = _shell ?? new Frame();
        }

        // Handle activation via ActivationHandlers.
        await HandleActivationAsync(activationArgs);

        // Activate the MainWindow.
        App.MainWindow.Activate();

        // Execute tasks after activation.
        await StartupAsync();
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }

        if (_defaultHandler.CanHandle(activationArgs))
        {
            await _defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task InitializeAsync()
    {
        await _themeSelectorService.InitializeAsync().ConfigureAwait(false);
        await Task.CompletedTask;
    }

    private async Task StartupAsync()
    {
        await _themeSelectorService.SetRequestedThemeAsync();
        await Task.CompletedTask;
    }
}
