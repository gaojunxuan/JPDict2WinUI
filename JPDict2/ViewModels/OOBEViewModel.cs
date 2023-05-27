using CommunityToolkit.Mvvm.ComponentModel;
using JPDict2.Contracts.Services;

namespace JPDict2.ViewModels;

public partial class OOBEViewModel : ObservableRecipient
{
    [ObservableProperty]
    public int jmDictUpgradePercentage = 0;

    [ObservableProperty]
    public int kanjiDictUpgradePercentage = 0;

    [ObservableProperty]
    public int uniDicUpgradePercentage = 0;

    public ILocalSettingsService LocalSettingsService
    {
        get;
    }

    public OOBEViewModel(ILocalSettingsService localSettingsService)
    {
        LocalSettingsService = localSettingsService;
    }
}
