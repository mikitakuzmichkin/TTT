using System;
using Dainty.Ads;
using Dainty.UI.WindowBase;
using TicTacToe.DI;
using TicTacToe.Mechanics;
using TicTacToe.Players;
using TicTacToe.SceneControllers.Params;
using TicTacToe.Services.Analytics;
using TicTacToe.Services.RateUs;
using TicTacToe.Sound;
using TicTacToe.UI.Windows.RateUs;
using UnityEngine.SceneManagement;

namespace TicTacToe.UI.Windows.FinishWindow
{
    public class FinishWindowController : AWindowController<FinishWindowView>, IConfigurableWindow<FinishWindowSettings>
    {
        private readonly ITicTacToeAnalytics _analytics;
        private readonly ISoundManager _soundManager;
        private readonly IRateUsResolver _rateUsResolver;

        private bool _isWin;

        public override string WindowId => WindowsId.GAME_FINISH;

        public FinishWindowController()
        {
            _analytics = ProjectContext.GetInstance<ITicTacToeAnalytics>();
            _soundManager = ProjectContext.GetInstance<ISoundManager>();
            _rateUsResolver = ProjectContext.GetInstance<IRateUsResolver>();
        }

        public void Initialize(FinishWindowSettings data)
        {
            _isWin = data.YouSign == data.WinSign;
            GameSound sound;

            if (data.WebType == EWebType.Offline && data.PersonType == EPersonType.Human)
            {
                view.SetFinishScreenAfterGameOver(data.WinSign);
                sound = GameSound.GameWin;
            }
            else
            {
                EFinishResult result;
                if (data.WinSign == ETttType.Draw)
                {
                    result = EFinishResult.Draw;
                    sound = GameSound.GameDraw;
                }
                else if (data.WinSign == data.YouSign)
                {
                    result = EFinishResult.Win;
                    sound = GameSound.GameWin;
                }
                else
                {
                    result = EFinishResult.Lose;
                    sound = GameSound.GameLose;
                }

                view.SetFinishScreenAfterGameOver(result);
            }

            _soundManager.Sound(sound);
        }

        protected override void OnSubscribe()
        {
            view.Restart += RestartGameClick;
            view.ToMenu += ToMenuClick;
        }

        protected override void OnUnSubscribe()
        {
            view.Restart -= RestartGameClick;
            view.ToMenu -= ToMenuClick;
        }

        protected override void OnEscape()
        {
            ToMenuClick();
        }

        private void RestartGameClick()
        {
            ButtonClickBase(RestartGame);
        }

        private void ToMenuClick()
        {
            ButtonClickBase(ToMenu);
        }

        private void ButtonClickBase(Action clickAction)
        {
            if (_isWin && _rateUsResolver.CanBeShown)
            {
                uiManager.Open<RateUsWindowController, RateUsWindowSettings>(
                    new RateUsWindowSettings(clickAction), true);
            }
            else
            {
                clickAction?.Invoke();
            }
        }

        private void RestartGame()
        {
            var gameplayData = ProjectContext.GetInstance<SceneStartupParams>().Gameplay.GameplayData;
            _analytics.GameStart(gameplayData.Mode, gameplayData.WebType, gameplayData.PersonType, WindowId);

            gameplayData.Continue = false;

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void ToMenu()
        {
            ProjectContext.GetInstance<SceneManagerWrapper>().LoadScene("Main");
        }
    }
}