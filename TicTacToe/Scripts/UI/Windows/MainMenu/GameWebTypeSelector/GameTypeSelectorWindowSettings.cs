using System;
using TicTacToe.Players;

namespace TicTacToe.UI.Windows.MainMenu.GameWebTypeSelector
{
    public class GameTypeSelectorWindowSettings
    {
        public readonly Action<EWebType, EPersonType> OnGameTypeSelected;

        public GameTypeSelectorWindowSettings(Action<EWebType, EPersonType> onGameTypeSelected)
        {
            OnGameTypeSelected = onGameTypeSelected;
        }
    }
}