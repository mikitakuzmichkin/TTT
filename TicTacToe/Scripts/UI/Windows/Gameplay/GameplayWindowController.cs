using System;
using System.Collections.Generic;
using Dainty.Ads;
using Dainty.UI;
using Dainty.UI.WindowBase;
using DG.Tweening;
using Newtonsoft.Json;
using TicTacToe.DI;
using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Base;
using TicTacToe.Players;
using TicTacToe.Services.Ads;
using TicTacToe.Services.Analytics;
using TicTacToe.Services.RateUs;
using TicTacToe.Services.Saving;
using TicTacToe.Sound;
using TicTacToe.UI.Windows.FinishWindow;
using TicTacToe.UI.Windows.OptionsWindow;
using TicTacToe.UI.Windows.OptionsWindow.SubWindows.Tutorial;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TicTacToe.UI.Windows.Gameplay
{
    public class GameplayWindowController : AWindowController<GameplayWindowView>, IConfigurableWindow<GameplayData>
    {
        private readonly IBoardUtil _boardUtil;
        private readonly IAdsController _ads;
        private readonly ITicTacToeAnalytics _analytics;
        private readonly ISoundManager _soundManager;

        private ABoardController _boardController;
        private PlayerTurnManager _playerTurnManager;
        private ETttType _yourSign;
        private GameplayData _gameplayData;
        private bool _showBanner;

        public override string WindowId => WindowsId.GAME;

        public GameplayWindowController()
        {
            _boardUtil = ProjectContext.GetInstance<IBoardUtil>();
            _ads = ProjectContext.GetInstance<IAdsController>();
            _analytics = ProjectContext.GetInstance<ITicTacToeAnalytics>();
            _soundManager = ProjectContext.GetInstance<ISoundManager>();
        }

        protected override void OnSubscribe()
        {
            view.SettingsButtonClick += ViewOnSettingButtonClick;
            view.BackButtonClick += ViewOnBackButtonClick;

            if (_showBanner)
            {
                ShowBanner();
            }

#if DEV
            view.CheatButtonClick += OnCheatButtonClick;
#endif
        }

        protected override void OnUnSubscribe()
        {
            view.BackButtonClick -= ViewOnBackButtonClick;
            view.SettingsButtonClick -= ViewOnSettingButtonClick;

#if DEV
            view.CheatButtonClick -= OnCheatButtonClick;
#endif
        }

        public override void BeforeShow()
        {
            base.BeforeShow();

            _boardController?.Show();
        }

        public override void Close(bool pop, bool animation = true, Action animationFinished = null)
        {
            base.Close(pop, animation, () =>
            {
                _boardController?.Close();
                animationFinished?.Invoke();
            });
        }

        public void Initialize(GameplayData gameplayData)
        {
            Uninitialize();

            _gameplayData = gameplayData;

            var yourPlayer = PlayerControllerManager.GetOfflinePlayerController(gameplayData.PlayerYour);
            APlayerController enemyController;

            var gameMode = gameplayData.Mode;
            var personType = gameplayData.PersonType;

            _boardController = _boardUtil.CreateController(gameMode, out var viewPrefab);
            ABoardModel model;

            if (gameplayData.WebType == EWebType.Offline)
            {
                var saveManager = ProjectContext.GetInstance<ISaveManager>();
                if (gameplayData.Continue)
                {
                    var save = saveManager.Load(gameMode);
                    model = save.BoardModel;
                    _yourSign = save.YourSign;
                }
                else
                {
                    model = _boardUtil.CreateModelByMode(gameMode);
                }

                var enemyPlayerModel = gameplayData.PlayerEnemy;
                if (personType == EPersonType.Human)
                {
                    enemyController = PlayerControllerManager.GetOfflinePlayerController(enemyPlayerModel);
                }
                else
                {
                    enemyController = PlayerControllerManager.GetOfflineBotController(
                        gameMode, enemyPlayerModel, model, _boardController, !gameplayData.Continue);
                }
            }
            else
            {
                throw new NotSupportedException($"WebType \"{gameplayData.WebType}\" not supported!");
            }

            var boardView = Object.Instantiate(viewPrefab, view.GameArea);

            _boardController.Initialize(model, boardView);
            _boardController.Model.TurnChanged += ModelOnTurnChanged;
            _boardController.Model.GameOver += ModelOnGameOver;

            if (gameplayData.WebType == EWebType.Offline && gameplayData.Continue)
            {
                _playerTurnManager = new PlayerTurnManager(model, yourPlayer, enemyController, _yourSign);
            }
            else
            {
                _playerTurnManager = new PlayerTurnManager(model, yourPlayer, enemyController, out _yourSign);
                var saveManager = ProjectContext.GetInstance<ISaveManager>();
                saveManager.CreateSave(gameMode, personType, model, _yourSign);
            }
            
            view.Initialize(yourPlayer.Model, enemyController.Model);
            
            view.InteractiveTutorialManager.Initialize(gameplayData, boardView, yourPlayer, enemyController);

            ModelOnTurnChanged();

            //banner
            view.SetOffsetForBanner(0);
            _showBanner = true;
            uiManager.WindowChanged += UiManagerOnWindowChanged;
        }

        public override void OnOpened()
        {
            OpenTutorialIfNeed(_gameplayData.Mode);
        }

        public void Uninitialize()
        {
            if (_boardController != null)
            {
                _boardController.Model.GameOver -= ModelOnGameOver;
                _boardController.Uninitialize();
            }

            if (_playerTurnManager != null)
            {
                _playerTurnManager.Unitialize();
            }

            uiManager.WindowChanged -= UiManagerOnWindowChanged;
        }

        public override void Dispose()
        {
            Uninitialize();
        }

        private void ModelOnTurnChanged()
        {
            view.SetTurn(_boardController.Model.Turn == _yourSign);
        }

        private void ViewOnBackButtonClick()
        {
            HideBanner();
            Uninitialize();
            ProjectContext.GetInstance<SceneManagerWrapper>().LoadScene("Main");
        }

        private void ViewOnSettingButtonClick()
        {
            Debug.Log("open setting");
            uiManager.Open<OptionsWindowController>(true);
            _analytics.SettingsOpen(WindowId);
        }

        private void ModelOnGameOver(ETttType type)
        {
#if DEV_LOG
            Debug.LogFormat("{0} Win!", type);
#endif

            if (_gameplayData.WebType == EWebType.Offline)
            {
                var saveManager = ProjectContext.GetInstance<ISaveManager>();
                saveManager.RemoveSave(_gameplayData.Mode);
            }

            var finishWindowSettings = new FinishWindowSettings
            {
                YouSign = _yourSign, WinSign = type,
                WebType = _gameplayData.WebType,
                PersonType = _gameplayData.PersonType
            };

            UnSubscribe();
            Uninitialize();


            ProjectContext.GetInstance<BoardAnimatorManager>().AddAnimationCallback(() =>
            {
                HideBanner();

                Action showFinishWindow = () =>
                {
                    _soundManager.MusicPause();
                    uiManager.Open<FinishWindowController, FinishWindowSettings>(finishWindowSettings, true);
                };

                if (_yourSign == type && ProjectContext.GetInstance<IRateUsResolver>().CanBeShown)
                {
                    showFinishWindow();
                }
                else if (AdDisplayLimiter.CanShow(TimeSpan.FromMinutes(5)))
                {
                    _ads.ShowInterstitial(false, failed =>
                    {
                        showFinishWindow();
                        if (!failed)
                        {
                            AdDisplayLimiter.TrackInterstitial();
                        }
                    });
                }
                else
                {
                    showFinishWindow();
                }
            }, 1f);

            int result;
            if (type == ETttType.Draw)
            {
                result = 2;
            }
            else
            {
                result = type == _yourSign ? 1 : 0;
            }

            //todo replace clicks and time to valid values
            _analytics.GameComplete(_gameplayData.Mode, _gameplayData.WebType, _gameplayData.PersonType, result, 0, 0);
        }

        private void ShowBanner()
        {
            _showBanner = true;
            _ads.ShowBanner(() => view.SetOffsetForBanner(_ads.BannerSize.y));
        }

        private void HideBanner(bool changeState = true)
        {
            _ads.HideBanner();
            if (changeState)
            {
                _showBanner = false;
            }
        }

        private void UiManagerOnWindowChanged(IWindowController controller)
        {
            if (controller != this)
            {
                HideBanner(false);
            }
        }

        private void OpenTutorialIfNeed(EGameMode gameMode)
        {
            var tutorialShownJson = PlayerPrefs.GetString(PlayerPrefsConsts.GAME_MODE_TUTORIAL_SHOWN);
            List<EGameMode> tutorialShownList;
            if (!string.IsNullOrEmpty(tutorialShownJson))
            {
                tutorialShownList = JsonConvert.DeserializeObject<List<EGameMode>>(tutorialShownJson);
                if (tutorialShownList.Contains(gameMode))
                {
                    return;
                }
            }
            else
            {
                tutorialShownList = new List<EGameMode>();
            }

            TutorialType type;
            switch (gameMode)
            {
                case EGameMode.Classic:
                    type = TutorialType.Classic;
                    break;
                case EGameMode.Alternative:
                    type = TutorialType.Alternative;
                    break;
                case EGameMode.Strategy:
                    type = TutorialType.Strategy;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var settings = OptionsWindowSettings.OpenTutorial(type, false);
            uiManager.Open<OptionsWindowController, OptionsWindowSettings>(settings, true, WindowTransition.None);

            tutorialShownList.Add(gameMode);
            PlayerPrefs.SetString(PlayerPrefsConsts.GAME_MODE_TUTORIAL_SHOWN,
                                  JsonConvert.SerializeObject(tutorialShownList));
        }

#if DEV
        private void OnCheatButtonClick(CheatAction cheatAction)
        {
            _boardController.Model.RunCheatAction(_yourSign, cheatAction);
        }
#endif
    }
}