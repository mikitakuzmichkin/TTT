using System;
using TicTacToe.Mechanics.Base;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Mechanics.Strategy
{
    public class CellViewAlternative4Strategy : AAlternativeCellView
    {
        [SerializeField] private BoardViewClassic4Strategy _classicBoard;
        [Header("Zoom")] [SerializeField] private Button _frontButtonForZoom;
        [SerializeField] private Graphic _frontButtonForZoomGraphic;

        public BoardViewClassic4Strategy ClassicBoard => _classicBoard;
        public Graphic FrontButtonForZoomGraphic => _frontButtonForZoomGraphic;
        public event Action FrontButtonForZoomClick;

        public override event Action<AAlternativeCellView, Vector3Int> Click;

        public bool Interactable
        {
            set
            {
                _frontButtonForZoomGraphic.raycastTarget = !_block && value;
                if(value) return;
                ClassicBoard.Interactable = false;
            }
        }
        
        public void InitializeClassicBoard(StrategyBoardModel model, Vector3Int strategyPos, Vector3Int alternativePos)
        {
            ClassicBoard.Initialize(model, strategyPos, alternativePos);
        }

        public void Unselect()
        {
            Select = false;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _frontButtonForZoom.onClick.AddListener(() => FrontButtonForZoomClick?.Invoke());
        }

        protected override void UnInitialize()
        {
            base.UnInitialize();
            _frontButtonForZoom.onClick.RemoveAllListeners();
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
            ClassicBoard.CellClick += classicPos => Click?.Invoke(this, classicPos);
        }
    }
}