using TicTacToe.DI;
using TicTacToe.Services.Analytics;
using TicTacToe.Settings;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.Settings
{
    public class SettingsSubWindowController : ASubWindowController<SettingsSubWindowView>
    {
        private readonly ISettingsManager _settingsManager;
        private readonly ITicTacToeAnalytics _analytics;

        public override string WindowId => SubWindowsId.SETTINGS;
        public override string Title => "Settings";

        public SettingsSubWindowController()
        {
            _settingsManager = ProjectContext.GetInstance<ISettingsManager>();
            _analytics = ProjectContext.GetInstance<ITicTacToeAnalytics>();
        }

        protected override void OnSubscribe()
        {
            view.SwitchMusic += ViewOnSwitchMusic;
            view.SwitchSound += ViewOnSwitchSound;
        }

        protected override void OnUnSubscribe()
        {
            view.SwitchMusic -= ViewOnSwitchMusic;
            view.SwitchSound -= ViewOnSwitchSound;
        }

        public override void BeforeShow()
        {
            base.BeforeShow();

            view.SetValues(_settingsManager);
        }

        private void ViewOnSwitchMusic(bool value)
        {
            _settingsManager.MusicEnabled.Value = value;
            _analytics.SettingsMusic(value);
        }

        private void ViewOnSwitchSound(bool value)
        {
            _settingsManager.SoundEnabled.Value = value;
            _analytics.SettingsSound(value);
        }
    }
}