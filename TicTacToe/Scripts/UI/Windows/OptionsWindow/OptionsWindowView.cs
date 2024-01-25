using System;
using Dainty.UI;
using Dainty.UI.WindowBase;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.Windows.OptionsWindow
{
    public class OptionsWindowView : AWindowView
    {
        [SerializeField] private Button _backToParameters;
        [SerializeField] private Button _doneButton;

        [SerializeField] private TitleSettingAnimation _titleSetting;

        [Space] [SerializeField] private UiManagerSettings _slidesUiManagerSettings;
        [SerializeField] private UiRoot _slidesUiRoot;

        [Header("SafeArea")] [SerializeField] private SafeArea _safeArea;
        [SerializeField] private RectTransform _topPanelBackground;

        public UiManagerSettings SlidesUiManagerSettings => _slidesUiManagerSettings;
        public UiRoot SlidesUiRoot => _slidesUiRoot;

        public event Action BackToParameters;
        public event Action DoneButton;

        private void Start()
        {
            _backToParameters.onClick.AddListener(() => BackToParameters?.Invoke());
            _doneButton.onClick.AddListener(() => DoneButton?.Invoke());

            _safeArea.Changed += SafeAreaOnChanged;
            SafeAreaOnChanged();
        }

        public void SetTitle(string text, bool animate)
        {
            _titleSetting.SetText(text, animate);
        }

        private void SafeAreaOnChanged()
        {
            var safeArea = UiRoot.GetSafeArea();
            var offsetMax = _topPanelBackground.offsetMax;
            offsetMax.y = UiRoot.CanvasSize.y - safeArea.y - safeArea.height;
            _topPanelBackground.offsetMax = offsetMax;
        }
    }
}