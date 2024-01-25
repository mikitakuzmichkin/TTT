using System;
using System.Collections.Generic;
using Dainty.UI;
using Dainty.UI.WindowBase;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.Windows.MainMenu
{
    public class MainMenuWindowView : AWindowView
    {
#pragma warning disable 649
        [SerializeField] private Button _awardsButton;
        [SerializeField] private Button _settingsButton;

        [Space] [SerializeField] private GameModeScrollSnap _gameModeScroll;
        [SerializeField] private Button _leftArrowButton;
        [SerializeField] private Button _rightArrowButton;
        [SerializeField] private GameModeSlideDict _gameModeSlideDict;

        [Space] [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueButton;

        [Space] [SerializeField] private SafeArea _safeArea;
        [SerializeField] private RectTransform _bottomTabsContainer;
        [SerializeField] private GameObject _bottomTabsBottomEdge;
#pragma warning restore 649

        private float _bottomTabsContainerOriginHeight;

        public GameModeScrollSnap GameModeScrollSnap => _gameModeScroll;

        public bool LeftArrowButtonEnabled
        {
            get => _leftArrowButton.gameObject.activeSelf;
            set => _leftArrowButton.gameObject.SetActive(value);
        }

        public bool RightArrowButtonEnabled
        {
            get => _rightArrowButton.gameObject.activeSelf;
            set => _rightArrowButton.gameObject.SetActive(value);
        }

        public bool ContinueButtonEnabled
        {
            get => _continueButton.gameObject.activeSelf;
            set => _continueButton.gameObject.SetActive(value);
        }

        public IDictionary<int, EGameMode> GameModeSlideDictionary => _gameModeSlideDict;

        public event Action AwardsButtonClick;
        public event Action SettingsButtonClick;

        public event Action LeftArrowButtonClick;
        public event Action RightArrowButtonClick;

        public event Action NewGameButtonClick;
        public event Action ContinueButtonClick;

        private void Start()
        {
            _bottomTabsContainerOriginHeight = _bottomTabsContainer.sizeDelta.y;
            _safeArea.Changed += SafeAreaOnChanged;
            SafeAreaOnChanged();

            _awardsButton.onClick.AddListener(() => AwardsButtonClick?.Invoke());
            _settingsButton.onClick.AddListener(() => SettingsButtonClick?.Invoke());

            _leftArrowButton.onClick.AddListener(() => LeftArrowButtonClick?.Invoke());
            _rightArrowButton.onClick.AddListener(() => RightArrowButtonClick?.Invoke());

            _newGameButton.onClick.AddListener(() => NewGameButtonClick?.Invoke());
            _continueButton.onClick.AddListener(() => ContinueButtonClick?.Invoke());
        }

        protected override void OnSubscribe()
        {
            _gameModeScroll.Interactable = true;
        }

        protected override void OnUnSubscribe()
        {
            _gameModeScroll.Interactable = false;
        }

        private void SafeAreaOnChanged()
        {
            var safeArea = UiRoot.GetSafeArea();
            var sizeDelta = _bottomTabsContainer.sizeDelta;
            sizeDelta.y = _bottomTabsContainerOriginHeight + safeArea.y;
            _bottomTabsContainer.sizeDelta = sizeDelta;

            _bottomTabsBottomEdge.SetActive(safeArea.y != 0);
        }

        [Serializable]
        private class GameModeSlideDict : SerializableDictionaryBase<int, EGameMode>
        {
        }
    }
}