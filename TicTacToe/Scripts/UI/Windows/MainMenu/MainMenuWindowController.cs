using System;
using System.Linq;
using Dainty.Ads;
using Dainty.UI.Interfaces;
using Dainty.UI.WindowBase;
using TicTacToe.DI;
using TicTacToe.Players;
using TicTacToe.SceneControllers.Params;
using TicTacToe.Services.Analytics;
using TicTacToe.Services.Saving;
using TicTacToe.UI.Windows.MainMenu.GameWebTypeSelector;
using TicTacToe.UI.Windows.OptionsWindow;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TicTacToe.UI.Windows.MainMenu
{
    public class MainMenuWindowController : AWindowController<MainMenuWindowView>, IConfigurableWindow<EGameMode>
    {
        private readonly ITicTacToeAnalytics _analytics;

        private EGameMode _selectedGameMode;
        private readonly IAdsController _ads;

        public override string WindowId => WindowsId.MAIN_MENU;

        public MainMenuWindowController()
        {
            _ads = ProjectContext.GetInstance<IAdsController>();
            _analytics = ProjectContext.GetInstance<ITicTacToeAnalytics>();
        }

        public void Initialize(EGameMode data)
        {
            _selectedGameMode = data;

            var index = view.GameModeSlideDictionary.First(pair => pair.Value == data).Key;
            view.GameModeScrollSnap.SetCurrentPage(index, false);
            RefreshArrowButtons(index);
        }

        protected override void OnInitialize()
        {
            view.GameModeScrollSnap.SetCurrentPage(0, false);
            RefreshArrowButtons(0);
            _selectedGameMode = view.GameModeSlideDictionary[0];
        }

        protected override void OnSubscribe()
        {
            view.LeftArrowButtonClick += OnLeftArrowButtonClick;
            view.RightArrowButtonClick += OnRightArrowButtonClick;
            view.GameModeScrollSnap.PageChanged += GameModeScrollSnapOnPageChanged;

            view.SettingsButtonClick += OnSettingButtonClick;
            
            view.NewGameButtonClick += OnNewGameButtonClick;
            view.ContinueButtonClick += OnContinueButtonClick;
        }

        protected override void OnUnSubscribe()
        {
            view.LeftArrowButtonClick -= OnLeftArrowButtonClick;
            view.RightArrowButtonClick -= OnRightArrowButtonClick;
            view.GameModeScrollSnap.PageChanged -= GameModeScrollSnapOnPageChanged;

            view.SettingsButtonClick -= OnSettingButtonClick;
            
            view.NewGameButtonClick -= OnNewGameButtonClick;
            view.ContinueButtonClick -= OnContinueButtonClick;
        }

        protected override void OnEscape()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnSettingButtonClick()
        {
            uiManager.Open<OptionsWindowController>(true);
            _analytics.SettingsOpen(WindowId);
        }

        public override void BeforeShow()
        {
            base.BeforeShow();

            RefreshContinueButton();
        }

        private void OnLeftArrowButtonClick()
        {
            var gameModeScrollSnap = view.GameModeScrollSnap;
            gameModeScrollSnap.CurrentPage--;
            _analytics.MenuGameModeChanged(view.GameModeSlideDictionary[gameModeScrollSnap.CurrentPage]);
        }

        private void OnRightArrowButtonClick()
        {
            var gameModeScrollSnap = view.GameModeScrollSnap;
            gameModeScrollSnap.CurrentPage++;
            _analytics.MenuGameModeChanged(view.GameModeSlideDictionary[gameModeScrollSnap.CurrentPage]);
        }

        private void GameModeScrollSnapOnPageChanged(int page)
        {
            RefreshArrowButtons(page);

            _selectedGameMode = view.GameModeSlideDictionary[page];
            PlayerPrefs.SetInt(PlayerPrefsConsts.LAST_GAME_MODE, (int) _selectedGameMode);

            RefreshContinueButton();
        }

        private void RefreshArrowButtons(int page)
        {
            view.LeftArrowButtonEnabled = page > 0;
            view.RightArrowButtonEnabled = page + 1 < view.GameModeScrollSnap.PagesCount;
        }

        private void RefreshContinueButton()
        {
            var saveManager = ProjectContext.GetInstance<ISaveManager>();
            view.ContinueButtonEnabled = saveManager.SaveExists(_selectedGameMode);
        }

        private void OnNewGameButtonClick()
        {
            var settings = new GameTypeSelectorWindowSettings(OnWebTypeSelected);
            uiManager.Open<GameTypeSelectorWindowController, GameTypeSelectorWindowSettings>(settings, true);
        }

        private void OnContinueButtonClick()
        {
            _ads.ShowInterstitial(false);
            var gameplayDataBuilder = GameplayDataBuilder.Continue(_selectedGameMode);
            NewGameBase(gameplayDataBuilder.GetResult());
        }

        private void OnWebTypeSelected(EWebType webType, EPersonType personType)
        {
            if (webType == EWebType.Offline)
            {
                var saveManager = ProjectContext.GetInstance<ISaveManager>();
                if (saveManager.SaveExists(_selectedGameMode))
                {
                    var save = saveManager.Load(_selectedGameMode);
                    saveManager.RemoveSave(_selectedGameMode);
                    //todo replace clicks and time to valid values
                    _analytics.GameDrop(save.GameMode, EWebType.Offline, save.PersonType, 0, 0);
                }

                var gameplayDataBuilder = GameplayDataBuilder.NewOffline(_selectedGameMode, personType);
                NewGameBase(gameplayDataBuilder.GetResult());
            }
            else
            {
                throw new ArgumentException("WebType not supported", nameof(webType));
            }
        }

        private void NewGameBase(GameplayData gameplayData)
        {
            var @params = ProjectContext.GetInstance<SceneStartupParams>();
            @params.Gameplay.GameplayData = gameplayData;

            ProjectContext.GetInstance<SceneManagerWrapper>().LoadScene("Game");

            if (gameplayData.Continue)
            {
                _analytics.GameContinue(gameplayData.Mode, gameplayData.WebType, gameplayData.PersonType);
            }
            else
            {
                _analytics.GameStart(gameplayData.Mode, gameplayData.WebType, gameplayData.PersonType, WindowId);
            }
        }
    }
}