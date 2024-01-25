using System;
using TMPro;
using UnityEngine;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.About
{
    public class AboutSubWindowView : ASubWindowView
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _versionText;

        [Space] [SerializeField] private PanelButton _toMoreGames;
        [SerializeField] private PanelButton _toTerms;
        [SerializeField] private PanelButton _toPrivacyPolicy;

        public event Action ToMoreGames;
        public event Action ToTerms;
        public event Action ToPrivacyPolicy;

        private void Start()
        {
            _toMoreGames.Click += () => ToMoreGames?.Invoke();
            _toTerms.Click += () => ToTerms?.Invoke();
            _toPrivacyPolicy.Click += () => ToPrivacyPolicy?.Invoke();
        }

        public void SetName(string name)
        {
            _nameText.text = name;
        }

        public void SetVersion(string value)
        {
            _versionText.text = $"Version: {value}";
        }
    }
}