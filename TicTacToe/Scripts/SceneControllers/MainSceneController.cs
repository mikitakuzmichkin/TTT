using TicTacToe.DI;
using TicTacToe.SceneControllers.Params;
using TicTacToe.Sound;
using TicTacToe.UI.Windows.MainMenu;
using UnityEngine;

namespace TicTacToe.SceneControllers
{
    public class MainSceneController : ASceneController
    {
        protected override void Initialize(SceneStartupParams @params)
        {
            var soundManager = ProjectContext.GetInstance<ISoundManager>();
            soundManager.MusicStart(GameSound.MenuMusic);

            var gameMode = (EGameMode) PlayerPrefs.GetInt(PlayerPrefsConsts.LAST_GAME_MODE, (int) EGameMode.Alternative);
            uiManager.Open<MainMenuWindowController, EGameMode>(gameMode);
        }
    }
}