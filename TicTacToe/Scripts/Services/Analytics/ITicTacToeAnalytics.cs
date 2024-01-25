using System;
using System.Collections.Generic;
using TicTacToe.Players;
using TicTacToe.Services.RateUs;
using TicTacToe.UI.Windows.OptionsWindow.SubWindows.Tutorial;

namespace TicTacToe.Services.Analytics
{
    public interface ITicTacToeAnalytics
    {
        void SetPropertiesOnce(bool musicEnabled, bool soundEnabled);

        void MenuGameModeChanged(EGameMode mode);

        void TutorialOpen(IEnumerable<TutorialType> types, bool isManualOpen);
        void TutorialStep(TutorialType type, int step, float time);
        void TutorialSkip(IEnumerable<TutorialType> types, int step, float time);
        void TutorialComplete(IEnumerable<TutorialType> types, float time);

        void GameStart(EGameMode mode, EWebType webType, EPersonType personType, string windowId);
        void GameContinue(EGameMode mode, EWebType webType, EPersonType personType);

        /// <param name="result">0 - loose, 1 - win, 2 - draw</param>
        /// <exception cref="ArgumentOutOfRangeException">Bad result value</exception>
        void GameComplete(EGameMode mode, EWebType webType, EPersonType personType, int result, int clicks, float time);

        void GameDrop(EGameMode mode, EWebType webType, EPersonType personType, int clicks, float time);

        void SettingsOpen(string from);
        void SettingsMusic(bool enabled);
        void SettingsSound(bool enabled);

        void RateUsShown();
        void RateUsClosed(RateUsResult result);

        void NotificationsEnabled(bool enabled);
    }
}