using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Base;
using TicTacToe.Players;

namespace TicTacToe.Services.Saving
{
    public interface ISaveManager
    {
        IGameSaveProvider CurrentSave { get; }

        bool SaveExists(EGameMode gameMode);

        IGameSaveProvider CreateSave(EGameMode gameMode, EPersonType personType, ABoardModel boardModel,
            ETttType yourSign);
        IGameSaveProvider Load(EGameMode gameMode);

        void RemoveSave(EGameMode mode);
    }
}