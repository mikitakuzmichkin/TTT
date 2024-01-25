using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TicTacToe.Mechanics.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Mechanics.Alternative
{
    public class AlternativeCellView : AAlternativeCellView
    {
#pragma warning disable 649
        [SerializeField] private Classic4AlternativeBoardView _classicBoard;        
#pragma warning restore 649

        public void Unselect()
        {
            Select = false;
        }

        public Classic4AlternativeBoardView ClassicBoard => _classicBoard;

        public override event Action<AAlternativeCellView, Vector3Int> Click;

        public void InitializeClassicBoard(AlternativeBoardModel model, Vector3Int alternativePos)
        {
            ClassicBoard.Initialize(model, alternativePos);
        }

        protected override void ChangeState(ETttType state, bool forceState = false)
        {
            if (forceState)
            {
                ClassicBoard.gameObject.SetActive(state == ETttType.None);
            }
            else
            {
                BoardAnimator.AddAnimationCallback(() => ClassicBoard.gameObject.SetActive(state == ETttType.None));
            }
            base.ChangeState(state, forceState);
        }

        protected override void SetCellClick()
        {
            ClassicBoard.CellClick += classikPos => Click?.Invoke(this, classikPos);
        }

    }
}