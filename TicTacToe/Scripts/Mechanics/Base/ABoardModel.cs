using System;

namespace TicTacToe.Mechanics.Base
{
    public abstract class ABoardModel
    {
        protected ETttType _turn;

        public ETttType Turn
        {
            get => _turn;
            set
            {
                _turn = value;
                TurnChanged?.Invoke();
            }
        }

        public event Action TurnChanged;
        public abstract event Action<ETttType> GameOver;

        public virtual void Uninitialize()
        {
            TurnChanged = null;
        }

#if DEV
        public abstract void RunCheatAction(ETttType winSign, CheatAction cheatAction);
#endif
    }
}