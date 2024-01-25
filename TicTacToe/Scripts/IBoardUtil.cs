using TicTacToe.Mechanics.Base;

namespace TicTacToe
{
    public interface IBoardUtil
    {
        ABoardController CreateController(EGameMode mode, out ABoardView viewPrefab);

        ABoardModel CreateModelByMode(EGameMode mode);
    }
}