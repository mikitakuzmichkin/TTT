using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Base;
using UnityEngine;

namespace TicTacToe.Players
{
    public class PlayerTurnManager
    {
        private readonly APlayerController _crossPlayer;
        private readonly APlayerController _noughtPlayer;

        private readonly ABoardModel _model;

        public PlayerTurnManager(ABoardModel model, APlayerController yourController, APlayerController enemyController,
            out ETttType yourSign)
        {
            _model = model;
            var turn = model.Turn;

            model.TurnChanged += OnTurnChanged;

            var isCross = turn == ETttType.Cross;
            if (Random.value > 0.5)
            {
                _crossPlayer = isCross ? yourController : enemyController;
                _noughtPlayer = isCross ? enemyController : yourController;
                yourSign = turn;
            }
            else
            {
                _crossPlayer = isCross ? enemyController : yourController;
                _noughtPlayer = isCross ? yourController : enemyController;
                yourSign = isCross ? ETttType.Noughts : ETttType.Cross;
            }

            OnTurnChanged();
        }

        public PlayerTurnManager(ABoardModel model, APlayerController yourController, APlayerController enemyController,
            ETttType yourSign)
        {
            _model = model;
            model.TurnChanged += OnTurnChanged;

            _crossPlayer = yourSign == ETttType.Cross ? yourController : enemyController;
            _noughtPlayer = yourSign == ETttType.Cross ? enemyController : yourController;

            OnTurnChanged();
        }

        public void Unitialize()
        {
            _model.TurnChanged -= OnTurnChanged;
        }

        private void OnTurnChanged()
        {
            if (_model.Turn == ETttType.Cross)
            {
                _crossPlayer.SetTurn();
            }
            else
            {
                _noughtPlayer.SetTurn();
            }
        }
    }
}