using System;

namespace TicTacToe.UI.Windows.RateUs
{
    public class RateUsWindowSettings
    {
        public readonly Action OnClosed;

        public RateUsWindowSettings(Action onClosed)
        {
            OnClosed = onClosed;
        }
    }
}