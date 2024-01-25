namespace TicTacToe.Settings
{
    public interface ISettingsManager
    {
        RxProperty<bool> MusicEnabled { get; set; }
        RxProperty<bool> SoundEnabled { get; set; }

#if DEV
        RxProperty<bool> DebugAdsEnabled { get; set; }
        RxProperty<bool> DebugMonitorEnabled { get; set; }
#endif
    }
}