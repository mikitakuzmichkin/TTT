using TicTacToe.Mechanics.Base;
using UnityEngine;

namespace TicTacToe.Mechanics.Alternative
{
    public class Classic4AlternativeBoardView : AClassic4AnotherBoardView
    {
        public void Initialize(AlternativeBoardModel model, Vector3Int alternativePos)
        {
            Uninitialize();
            
            base.Initialize();

            _winline.SetActive(false);

            for (var i = 0; i < _cells.Length; i++)
            {
                var cell = _cells[i];
                cell.ForceSetState(model.GetClassicCell(alternativePos, new Vector3Int(i % _WIDTH, i / _WIDTH, 0)));
                cell.Click += CellOnClick;
            }
        }
    }
}