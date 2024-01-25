using System;
using TicTacToe.Mechanics.Base;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Mechanics.Strategy
{
    public class StrategyCellView : ACellView
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private SymbolSprite _draw;
        [SerializeField] private BoardViewAlternative4Strategy _alternativeBoard;

        [Space]
        [Header("Zoom")]
        [SerializeField]
        private Image _zoomBackGround;
        [SerializeField] private Button _frontButton;
        [SerializeField] private Graphic _frontButtonGraphic;
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Graphic _backgroundButtonGraphic;
        [SerializeField] private RectTransform _rectTransform;

        private bool _block;

        public BoardViewAlternative4Strategy AlternativeBoard => _alternativeBoard;

        public bool Block
        {
            set
            {
                _block = value;
                _alternativeBoard.Block = value;
            }
        }
        public bool Interactable
        {
            set
            {
                _frontButtonGraphic.raycastTarget = value;
                if (value)
                {
                    return;
                }

                AlternativeBoard.Interactable = value;
            }
        }
        public Image ZoomBackGround => _zoomBackGround;
        public Graphic BackgroundButtonGraphic => _backgroundButtonGraphic;
        public Graphic FrontButtonGraphic => _frontButtonGraphic;
        public RectTransform RectTransform => _rectTransform;

        private void Awake()
        {
            _frontButton.onClick.AddListener(() => FrontButtonClick?.Invoke());
            _backgroundButton.onClick.AddListener(() => BackgroundButtonClick?.Invoke());
            SetCellClick();
        }

        private void OnDestroy()
        {
            _frontButton.onClick.RemoveAllListeners();
            _backgroundButton.onClick.RemoveAllListeners();
        }

        public event Action<StrategyCellView, Vector3Int, Vector3Int> Click;
        public event Action FrontButtonClick;
        public event Action BackgroundButtonClick;

        protected override void ChangeState(ETttType state, bool forceState = false)
        {
            if (forceState)
            {
                AlternativeBoard.gameObject.SetActive(state == ETttType.None);
            }
            else
            {
                BoardAnimator.AddAnimationCallback(() => AlternativeBoard.gameObject.SetActive(state == ETttType.None));
            }
            base.ChangeState(state, forceState);
            if (state == ETttType.Draw)
            {
                if (forceState)
                {
                    _draw.FrameAnimationComponent.ForceAnimation();
                }
                else
                {
                    BoardAnimator.AddAnimation(_draw.FrameAnimationComponent.ReturnAnimation());
                }
                _symbolImage.color = _draw.Color;
            }
            
        }

        public void InitializeAlternativeBoard(StrategyBoardModel model, Vector3Int strategyPos)
        {
            AlternativeBoard.Initialize(model, strategyPos);
        }

        protected void SetCellClick()
        {
            AlternativeBoard.CellClick +=
                (alternativePos, classicPos) => Click?.Invoke(this, alternativePos, classicPos);
        }
    }
}