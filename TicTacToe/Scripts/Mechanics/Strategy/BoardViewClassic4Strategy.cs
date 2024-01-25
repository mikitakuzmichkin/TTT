using TicTacToe.Mechanics.Base;
using UnityEngine;

namespace TicTacToe.Mechanics.Strategy
{
    public class BoardViewClassic4Strategy : AClassic4AnotherBoardView
    {
        private bool _isInteractable;
        public override bool Interactable
        {
            get => _isInteractable;
            set
            {
                _isInteractable = value;
                foreach (var cell in _cells)
                {
                    cell.Interactable = value;
                }
            }
        }

        public void Initialize(StrategyBoardModel model, Vector3Int strategyPos, Vector3Int alternativePos)
        {
            Uninitialize();
            
            base.Initialize();

            _winline.SetActive(false);

            for (var i = 0; i < _cells.Length; i++)
            {
                var cell = _cells[i];
                cell.ForceSetState(
                    model.GetClassicCell(strategyPos, alternativePos, new Vector3Int(i % _WIDTH, i / _WIDTH, 0)));
                cell.Click += CellOnClick;
            }
        }
    }
}