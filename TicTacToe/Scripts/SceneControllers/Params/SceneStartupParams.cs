namespace TicTacToe.SceneControllers.Params
{
    public class SceneStartupParams
    {
        public GameplaySceneControllerParams Gameplay;

        public SceneStartupParams()
        {
            Gameplay = new GameplaySceneControllerParams();
        }

        public class GameplaySceneControllerParams
        {
            public GameplayData GameplayData;
        }
    }
}