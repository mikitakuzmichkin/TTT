using TicTacToe.DI;
using TicTacToe.SceneControllers.Params;
using TicTacToe.Sound;
using TicTacToe.UI.Windows.Gameplay;

namespace TicTacToe.SceneControllers
{
    public class GameplaySceneController : ASceneController
    {
        protected override void Initialize(SceneStartupParams @params)
        {
            var soundManager = ProjectContext.GetInstance<ISoundManager>();
            soundManager.MusicStart(GameSound.InGameMusic);

            uiManager.Open<GameplayWindowController, GameplayData>(@params.Gameplay.GameplayData);
        }
    }
}