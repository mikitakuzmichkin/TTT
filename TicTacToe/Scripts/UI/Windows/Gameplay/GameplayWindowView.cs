using System;
using Dainty.UI.WindowBase;
using TicTacToe.InteractiveTutorial;
using TicTacToe.Players;
using TicTacToe.UI.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.Windows.Gameplay
{
    public class GameplayWindowView : AWindowView
    {
#pragma warning disable 649
        [Space] [SerializeField] private Button _backButton;
        [SerializeField] private Button _themesButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private RectTransform _gameArea;
        [SerializeField] private RectTransform _contentRect;

        [Space] [SerializeField] private PlayerInGameView _yourPlayerView;
        [SerializeField] private PlayerInGameView _enemyPlayerView;

        [Space] [SerializeField] private InteractiveTutorialManager _interactiveTutorial;

#if UNITY_EDITOR || DEV
        [Header("DEBUG")] [SerializeField] private WinLoseDrawButtonsPanel _cheatButtonsPrefab;
        [SerializeField] private Transform _cheatButtonsContainer;
#endif
#pragma warning restore 649

        public RectTransform GameArea => _gameArea;

        public InteractiveTutorialManager InteractiveTutorialManager => _interactiveTutorial;

        public event Action BackButtonClick;
        public event Action ThemeButtonClick;
        public event Action SettingsButtonClick;
#if DEV
        public event Action<CheatAction> CheatButtonClick;
#endif

        private void Start()
        {
            _backButton.onClick.AddListener(() => BackButtonClick?.Invoke());
            _themesButton.onClick.AddListener(() => ThemeButtonClick?.Invoke());
            _settingsButton.onClick.AddListener(() => SettingsButtonClick?.Invoke());

#if DEV
            var cheatButtons = Instantiate(_cheatButtonsPrefab, _cheatButtonsContainer);
            cheatButtons.Subscribe(action => CheatButtonClick?.Invoke(action));
#endif
        }

        public void Initialize(IPlayerModel yourPlayerModel, IPlayerModel enemyPlayerModel)
        {
            _yourPlayerView.Initialize(yourPlayerModel);
            _enemyPlayerView.Initialize(enemyPlayerModel);
        }

        public void SetOffsetForBanner(float unscaledOffset)
        {
            var offsetMin = _contentRect.offsetMin;
            offsetMin.y = unscaledOffset * Screen.dpi / 160;
            _contentRect.offsetMin = offsetMin;
        }

        public void SetTurn(bool your)
        {
            _yourPlayerView.TurnTimerActive = your;
            _enemyPlayerView.TurnTimerActive = !your;
        }
    }
}