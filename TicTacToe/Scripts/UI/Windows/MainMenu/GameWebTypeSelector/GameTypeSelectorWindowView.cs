using System;
using Dainty.UI;
using Dainty.UI.WindowBase;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.Windows.MainMenu.GameWebTypeSelector
{
    public class GameTypeSelectorWindowView : AWindowView
    {
#pragma warning disable 649
        [SerializeField] private Button[] _closeButtons;

        [Space] [SerializeField] private Button _playWithBotButton;
        [SerializeField] private Button _playWithFriendButton;
        [SerializeField] private Button _playOnlineButton;

        [Header("Safe Area")] [SerializeField] private SafeArea _safeArea;
        [SerializeField] private RectTransform _safeAreaRectTransform;
        [SerializeField] private RectTransform _backgroundRect;
#pragma warning restore 649

        public event Action CloseButtonClick;
        public event Action PlayWithBotButtonClick;
        public event Action PlayWithFriendButtonClick;
        public event Action PlayOnlineButtonClick;

        private void Start()
        {
            _safeArea.Changed += SafeAreaOnChanged;
            SafeAreaOnChanged();

            foreach (var closeButton in _closeButtons)
            {
                closeButton.onClick.AddListener(() => CloseButtonClick?.Invoke());
            }

            _playWithBotButton.onClick.AddListener(() => PlayWithBotButtonClick?.Invoke());
            _playWithFriendButton.onClick.AddListener(() => PlayWithFriendButtonClick?.Invoke());
            _playOnlineButton.onClick.AddListener(() => PlayOnlineButtonClick?.Invoke());
        }

        private void SafeAreaOnChanged()
        {
            var offset = _backgroundRect.offsetMin;
            var safeArea = UiRoot.GetSafeArea();
            offset.y = -safeArea.y;
            _backgroundRect.offsetMin = offset;

            var anchorMax = _safeAreaRectTransform.anchorMax;
            anchorMax.y = 1;
            _safeAreaRectTransform.anchorMax = anchorMax;
        }
    }
}