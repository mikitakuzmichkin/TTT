using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Base;
using TicTacToe.Players;

namespace TicTacToe.Services.Saving
{
    public interface IGameSaveProvider
    {
        EGameMode GameMode { get; }
        EPersonType PersonType { get; }
        ABoardModel BoardModel { get; }
        ETttType YourSign { get; }
#if UNITY_EDITOR
        void ForceSave();
#endif
    }
}