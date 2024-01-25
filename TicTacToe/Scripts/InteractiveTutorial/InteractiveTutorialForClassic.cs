using System.Collections;
using System.Collections.Generic;
using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Base;
using UnityEngine;

namespace TicTacToe.InteractiveTutorial
{
    public class InteractiveTutorialForClassic : AInteractiveTutorial
    {
        [SerializeField] private Sprite _emptyCellSprite;
        [SerializeField] private Color _emptyCellColor;

        private CellView[] _cells;
        
        private int _countRepeat = 3;

        public void Initialize(CellView[] cells)
        {
            base.Initialize();
            _cells = cells;
        }

        public void MarkAllEmptyCells()
        {
            if (_IsInitialized == false || _countRepeat <= 0)
            {
                return;
            }
            
            ShowTutorial();
            
            foreach (var cell in _cells)
            {
                if (cell.State == ETttType.None)
                {
                    cell.SetSprite(_emptyCellSprite, _emptyCellColor);
                }
            }

            _countRepeat--;
        }

        public void For2Players()
        {
            _countRepeat *= 2;
        }

        public override void TutorialHide()
        {
            foreach (var cell in _cells)
            {
                if (cell.State == ETttType.None)
                {
                    cell.State = ETttType.None;
                }
            }
        }
    }
}