using Dainty.UI;
using TicTacToe.UI.Windows.OptionsWindow.SubWindows.About;
using TicTacToe.UI.Windows.OptionsWindow.SubWindows.Rules;
using TicTacToe.UI.Windows.OptionsWindow.SubWindows.Settings;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.Options
{
    public class OptionsMainSubWindowController : ASubWindowController<OptionsMainSubWindowView>
    {
        public override string WindowId => SubWindowsId.MAIN;
        public override string Title => "Options";

        public override void BeforeShow()
        {
            base.BeforeShow();

#if DEV
            var settingsManager = DI.ProjectContext.GetInstance<TicTacToe.Settings.ISettingsManager>();
            var optionsDebugPanel = view.OptionsDebugPanel;
            optionsDebugPanel.AdsEnabledSwitchState = settingsManager.DebugAdsEnabled;
            optionsDebugPanel.MonitorEnabledSwitchState = settingsManager.DebugMonitorEnabled;
#endif
        }

        protected override void OnSubscribe()
        {
            view.ToAbout += ToAbout;
            view.ToRules += ToRules;
            view.ToSetting += ToGeneralSettings;

#if DEV
            var settingsManager = DI.ProjectContext.GetInstance<TicTacToe.Settings.ISettingsManager>();
            var optionsDebugPanel = view.OptionsDebugPanel;
            optionsDebugPanel.AdsEnabledSwitchChanged += isOn => settingsManager.DebugAdsEnabled.Value = isOn;
            optionsDebugPanel.MonitorEnabledSwitchChanged += isOn => settingsManager.DebugMonitorEnabled.Value = isOn;
#endif
        }

        protected override void OnUnSubscribe()
        {
            view.ToAbout -= ToAbout;
            view.ToRules -= ToRules;
            view.ToSetting -= ToGeneralSettings;

#if DEV
            view.OptionsDebugPanel.Unsubscribe();
#endif
        }

        private void ToAbout()
        {
            OpenSlide<AboutSubWindowController>();
        }

        private void ToRules()
        {
            OpenSlide<RulesSubWindowController>();
        }

        private void ToGeneralSettings()
        {
            OpenSlide<SettingsSubWindowController>();
        }

        private void OpenSlide<T>() where T : ISubWindowController, new()
        {
            uiManager.Open<T>(WindowTransition.AnimateClosing | WindowTransition.AnimateOpening);
        }
    }
}