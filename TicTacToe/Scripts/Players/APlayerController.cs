using System;
using TicTacToe.Mechanics;

namespace TicTacToe.Players
{
    public abstract class APlayerController
    {
        protected IPlayerModel _modelPlayer;
        protected ETttType _symbol;

        public IPlayerModel Model => _modelPlayer;

        public APlayerController(IPlayerModel modelPlayer)
        {
            _modelPlayer = modelPlayer;
        }

        // todo Icontroller
        protected ETttType Symbol
        {
            get => _symbol;
            set => _symbol = value;
        }

        public event Action StartTurn;

        public virtual void SetTurn()
        {
            StartTurn?.Invoke();
        }
    }
}