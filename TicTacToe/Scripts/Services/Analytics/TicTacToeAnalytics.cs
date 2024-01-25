using System;
using System.Collections.Generic;
using System.Linq;
using Dainty.Analytics;
using Dainty.Analytics.Provider;
using TicTacToe.Players;
using TicTacToe.Services.RateUs;
using TicTacToe.UI.Windows.OptionsWindow.SubWindows.Tutorial;

namespace TicTacToe.Services.Analytics
{
    public class TicTacToeAnalytics : DaintyAnalyticsBase, ITicTacToeAnalytics
    {
        private const string API_KEY = "6abca15fbf517e10caa2c132ef60f1b7";

        private const string MENU_GAME_MODE_CHANGED = "menu_game_mode_changed";

        private const string TUTORIAL_OPEN = "tutorial_open";
        private const string TUTORIAL_STEP = "tutorial_step";
        private const string TUTORIAL_SKIP = "tutorial_skip";
        private const string TUTORIAL_COMPLETED = "tutorial_completed";

        private const string GAME_START = "game_start";
        private const string GAME_CONTINUE = "game_continue";
        private const string GAME_COMPLETE = "game_complete";
        private const string GAME_DROP = "game_drop";

        private const string SETTINGS_OPEN = "settings_open";
        private const string SETTINGS_MUSIC = "settings_music";
        private const string SETTINGS_SOUND = "settings_sound";

        private const string RATE_US_SHOWN = "rate_us_shown";
        private const string RATE_US_CLOSED = "rate_us_closed";

        private const string NOTIFICATIONS_ENABLED = "notifications_enabled";

        protected override IDaintyAnalyticsProvider GetProdProvider()
        {
            return new AmplitudeAnalyticsProvider(API_KEY);
        }

        public void SetPropertiesOnce(bool musicEnabled, bool soundEnabled)
        {
            SetOnceUserProperty(SETTINGS_MUSIC, musicEnabled.ToString().ToLower());
            SetOnceUserProperty(SETTINGS_SOUND, soundEnabled.ToString().ToLower());
        }

    #region Menu

        public void MenuGameModeChanged(EGameMode mode)
        {
            LogEvent(MENU_GAME_MODE_CHANGED, new Dictionary<string, object>
            {
                ["mode"] = mode.ToString().ToLowerSnakeCase()
            });
        }

    #endregion

    #region Tutorial

        public void TutorialOpen(IEnumerable<TutorialType> types, bool isManualOpen)
        {
            var modes = types.Select(type => type.ToString().ToLowerSnakeCase()).ToArray();
            LogEvent(TUTORIAL_OPEN, new Dictionary<string, object>
            {
                ["modes"] = modes,
                ["manual"] = isManualOpen
            });
        }

        public void TutorialStep(TutorialType type, int step, float time)
        {
            LogEvent(TUTORIAL_STEP, new Dictionary<string, object>
            {
                ["mode"] = type.ToString().ToLowerSnakeCase(),
                ["step"] = step,
                ["time"] = (float) Math.Round(time, 2)
            });
        }

        public void TutorialSkip(IEnumerable<TutorialType> types, int step, float time)
        {
            var modes = types.Select(type => type.ToString().ToLowerSnakeCase()).ToArray();
            LogEvent(TUTORIAL_SKIP, new Dictionary<string, object>
            {
                ["modes"] = modes,
                ["step"] = step,
                ["time"] = (float) Math.Round(time, 2)
            });
        }

        public void TutorialComplete(IEnumerable<TutorialType> types, float time)
        {
            var modes = types.Select(type => type.ToString().ToLowerSnakeCase()).ToArray();
            LogEvent(TUTORIAL_COMPLETED, new Dictionary<string, object>
            {
                ["modes"] = modes,
                ["time"] = (float) Math.Round(time, 2)
            });
        }

    #endregion

    #region Game

        public void GameStart(EGameMode mode, EWebType webType, EPersonType personType, string windowId)
        {
            LogEvent(GAME_START, new Dictionary<string, object>
            {
                ["mode"] = mode.ToString().ToLowerSnakeCase(),
                ["web_type"] = webType.ToString().ToLowerSnakeCase(),
                ["person_type"] = personType.ToString().ToLowerSnakeCase(),
                ["from"] = windowId,
            });
        }

        public void GameContinue(EGameMode mode, EWebType webType, EPersonType personType)
        {
            LogEvent(GAME_CONTINUE, new Dictionary<string, object>
            {
                ["mode"] = mode.ToString().ToLowerSnakeCase(),
                ["web_type"] = webType.ToString().ToLowerSnakeCase(),
                ["person_type"] = personType.ToString().ToLowerSnakeCase()
            });
        }

        public void GameComplete(EGameMode mode, EWebType webType, EPersonType personType, int result, int clicks,
            float time)
        {
            LogEvent(GAME_COMPLETE, new Dictionary<string, object>
            {
                ["mode"] = mode.ToString().ToLowerSnakeCase(),
                ["web_type"] = webType.ToString().ToLowerSnakeCase(),
                ["person_type"] = personType.ToString().ToLowerSnakeCase(),
                ["result"] = result,
                ["clicks"] = clicks,
                ["time"] = time
            });
        }

        public void GameDrop(EGameMode mode, EWebType webType, EPersonType personType, int clicks, float time)
        {
            LogEvent(GAME_DROP, new Dictionary<string, object>
            {
                ["mode"] = mode.ToString().ToLowerSnakeCase(),
                ["web_type"] = webType.ToString().ToLowerSnakeCase(),
                ["person_type"] = personType.ToString().ToLowerSnakeCase(),
                ["clicks"] = clicks,
                ["time"] = time
            });
        }

    #endregion

    #region Settings

        public void SettingsOpen(string from)
        {
            LogEvent(SETTINGS_OPEN, new Dictionary<string, object>
            {
                ["from"] = from
            });
        }

        public void SettingsMusic(bool enabled)
        {
            LogEvent(SETTINGS_MUSIC, new Dictionary<string, object>
            {
                ["enabled"] = enabled
            });
            SetUserProperty(SETTINGS_MUSIC, enabled.ToString().ToLowerInvariant());
        }

        public void SettingsSound(bool enabled)
        {
            LogEvent(SETTINGS_SOUND, new Dictionary<string, object>
            {
                ["enabled"] = enabled
            });
            SetUserProperty(SETTINGS_SOUND, enabled.ToString().ToLowerInvariant());
        }

    #endregion

    #region Rate Us

        public void RateUsShown()
        {
            LogEvent(RATE_US_SHOWN);
        }

        public void RateUsClosed(RateUsResult result)
        {
            LogEvent(RATE_US_CLOSED, new Dictionary<string, object>
            {
                ["result"] = result.ToString().ToLowerSnakeCase()
            });
        }

    #endregion

        public void NotificationsEnabled(bool enabled)
        {
            SetUserProperty(NOTIFICATIONS_ENABLED, enabled);
        }
    }
}