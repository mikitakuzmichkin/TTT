using System;
using System.Collections;
using System.Collections.Generic;
using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Alternative;
using TicTacToe.Mechanics.Base;
using UnityEngine;

namespace TicTacToe.InteractiveTutorial
{
    public class InteractiveTutorialForAlternative : AInteractiveTutorial
    {
        [SerializeField] private Sprite _emptyCellSprite;
        [SerializeField] private Color _emptyCellColor;

        private AlternativeBoard<ACellView> _alternativeBoard;
        
        private int _emptyCellCountRepeat = 1;

        public void Initialize(AlternativeBoard<ACellView> alternativeBoard)
        {
            base.Initialize();
            _alternativeBoard = alternativeBoard;
        }

        public void MarkAllEmptyCells()
        {
            if (_IsInitialized == false || _emptyCellCountRepeat <= 0)
            {
                return;
            }
            
            ShowTutorial();
            
            foreach (CellView cell in _alternativeBoard.GetAllClassicCells())
            {
                if (cell.State == ETttType.None)
                {
                    cell.SetSprite(_emptyCellSprite, _emptyCellColor);
                }
            }

            _emptyCellCountRepeat--;
        }

        public void For2Players()
        {
            
        }

        public void MarkAfterTurn()
        {
            if (_alternativeBoard.IsBlocked(0, 0) || _alternativeBoard.IsBlocked(0, 0))
            {
                
            }
            else
            {
                MarkAllEmptyCells();
            }
        }

        public void MarkAlternativeCell(Vector3Int alternativeCell, ETttType type, Action unSubscribe)
        {
            
        }

        public override void TutorialHide()
        {
            foreach (CellView cell in _alternativeBoard.GetAllClassicCells())
            {
                if (cell.State == ETttType.None)
                {
                    cell.State = ETttType.None;
                }
            }
        }
    }
}