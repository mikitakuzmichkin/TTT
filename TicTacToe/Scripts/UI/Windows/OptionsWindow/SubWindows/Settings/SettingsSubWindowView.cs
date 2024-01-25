using System;
using TicTacToe.Settings;
using UnityEngine;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.Settings
{
    public class SettingsSubWindowView : ASubWindowView
    {
        [SerializeField] private PanelButtonSwitch _switchMusic;
        [SerializeField] private PanelButtonSwitch _switchSound;

        public event Action<bool> SwitchMusic;
        public event Action<bool> SwitchSound;

        private void Start()
        {
            _switchMusic.Changed += value => SwitchMusic?.Invoke(value);
            _switchSound.Changed += value => SwitchSound?.Invoke(value);
        }

        public void SetValues(ISettingsManager settingsManager)
        {
            _switchMusic.IsOn = settingsManager.MusicEnabled;
            _switchSound.IsOn = settingsManager.SoundEnabled;
        }
    }
}