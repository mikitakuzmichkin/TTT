using System;
using UnityEngine;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.Options
{
    public class OptionsMainSubWindowView : ASubWindowView
    {
        [SerializeField] private PanelButton _toSetting;
        [SerializeField] private PanelButton _toRules;
        [SerializeField] private PanelButton _toAbout;

#if UNITY_EDITOR || DEV
        [Header("DEBUG")] [SerializeField] private Transform _itemsContainer;
        [SerializeField] private GameObject _spacingPrefab;
        [SerializeField] private OptionsDebugPanel _optionsDebugPanelPrefab;

        public OptionsDebugPanel OptionsDebugPanel { get; private set; }
#endif

        public event Action ToSetting;
        public event Action ToRules;
        public event Action ToAbout;

        private void Awake()
        {
            _toSetting.Click += () => ToSetting?.Invoke();
            _toRules.Click += () => ToRules?.Invoke();
            _toAbout.Click += () => ToAbout?.Invoke();

#if DEV
            Instantiate(_spacingPrefab, _itemsContainer);
            OptionsDebugPanel = Instantiate(_optionsDebugPanelPrefab, _itemsContainer);
#endif
        }
    }
}