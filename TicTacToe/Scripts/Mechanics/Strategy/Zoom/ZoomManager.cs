using System;
using DG.Tweening;
using TicTacToe.DI;
using UnityEngine;

namespace TicTacToe.Mechanics.Strategy.Zoom
{
    public class ZoomManager : MonoBehaviour
    {
        [SerializeField] private float _zoomFactor = 2f;
        [SerializeField] private float _zoomDuration = 0.4f;

        [SerializeField] private StrategyBoardView _strategyBoardView;

        private CellViewAlternative4Strategy _curAlternativeCell;
        private StrategyCellView _curStrategyCell;
        private bool _doubleZoom;
        private Vector3 _localScaleAlternativeBoard;

        private Vector3 _localStrategyPos;
        private Vector3 _localStrategyScale;
        private bool _zoom;
        private Sequence _zoomAnimation;
        private BoardAnimatorManager _boardAnimator;

        public bool IsZoomClosed => _curStrategyCell == null;
        
        public event Action ZoomClosed;

        private void Awake()
        {
            _boardAnimator = ProjectContext.GetInstance<BoardAnimatorManager>();
            foreach (StrategyCellView strategyCell in _strategyBoardView)
            {
                strategyCell.ZoomBackGround.enabled = false;
                strategyCell.FrontButtonClick += () => OpenFirstLevelZoom(strategyCell);
                strategyCell.BackgroundButtonClick += CloseOneLevelZoom;
                _localScaleAlternativeBoard = strategyCell.AlternativeBoard.transform.localScale;
                foreach (CellViewAlternative4Strategy alternativeCell in strategyCell.AlternativeBoard)
                {
                    alternativeCell.FrontButtonForZoomClick += () => OpenSecondLevelZoom(alternativeCell);
                }
            }
        }

        public void OpenFirstLevelZoom(StrategyCellView strategyCell)
        {
            if (_zoom)
            {
                return;
            }

            _curStrategyCell = strategyCell;

            _zoom = true;
            _zoomAnimation?.Kill();
            _zoomAnimation = DOTween.Sequence();
            _localStrategyPos = _curStrategyCell.transform.localPosition;
            _localStrategyScale = _curStrategyCell.transform.localScale;

            _strategyBoardView.Interactable = false;

            _curStrategyCell.BackgroundButtonGraphic.raycastTarget = true;
            _zoomAnimation
                .Append(_curStrategyCell.RectTransform.DOLocalMove(Vector3.zero, _zoomDuration)
                                        .SetEase(Ease.OutCubic))
                .Join(_curStrategyCell.RectTransform.DOScale(_localStrategyScale * _zoomFactor, _zoomDuration)
                                      .SetEase(Ease.OutCubic))
                .AppendCallback(() => _curStrategyCell.AlternativeBoard.Interactable = true);

            _curStrategyCell.transform.SetAsLastSibling();
            _curStrategyCell.ZoomBackGround.enabled = true;

            _boardAnimator.AddAnimation(_zoomAnimation, false);
        }

        public void OpenSecondLevelZoom(CellViewAlternative4Strategy curAlternativeCell)
        {
            if (_doubleZoom || _curStrategyCell == null) return;

            _curAlternativeCell = curAlternativeCell;
            _doubleZoom = true;
            _zoomAnimation?.Kill();
            _zoomAnimation = DOTween.Sequence();

            _curStrategyCell.AlternativeBoard.Interactable = false;

            _zoomAnimation
                .Append(_curStrategyCell.AlternativeBoard.transform
                                        .DOLocalMove(curAlternativeCell.transform.localPosition * -1 * _zoomFactor,
                                                     _zoomDuration)
                                        .SetEase(Ease.OutCubic))
                .Join(_curStrategyCell.AlternativeBoard.transform
                                      .DOScale(_localScaleAlternativeBoard * _zoomFactor, _zoomDuration)
                                      .SetEase(Ease.OutCubic))
                .AppendCallback(() => _curAlternativeCell.ClassicBoard.Interactable = true);
            
            _boardAnimator.AddAnimation(_zoomAnimation, false);
        }

        public void CloseOneLevelZoom()
        {
            CloseOneLevelZoom(true);
        }
        public void CloseOneLevelZoom(bool kill)
        {
            if (_doubleZoom)
            {
                if (_doubleZoom == false) return;
                if (kill)
                {
                    _zoomAnimation?.Kill();
                }

                _zoomAnimation = DOTween.Sequence();

                _curAlternativeCell.ClassicBoard.Interactable = false;

                _zoomAnimation
                    .Append(_curStrategyCell.AlternativeBoard.transform.DOLocalMove(Vector3.zero, _zoomDuration))
                    .Join(_curStrategyCell.AlternativeBoard.transform.DOScale(
                        _localScaleAlternativeBoard, _zoomDuration));
                _zoomAnimation.AppendCallback(() =>
                {
                    _curStrategyCell.AlternativeBoard.Interactable = true;
                    _doubleZoom = false;
                });
                _boardAnimator.AddAnimation(_zoomAnimation, false);
                return;
            }

            if (_zoom == false) return;
            
            if (kill)
            {
                _zoomAnimation?.Kill();
            }

            _zoomAnimation = DOTween.Sequence();
            _curStrategyCell.AlternativeBoard.Interactable = false;

            _zoomAnimation.Append(_curStrategyCell.RectTransform.DOLocalMove(_localStrategyPos, _zoomDuration))
                .Join(_curStrategyCell.RectTransform.DOScale(_localStrategyScale, _zoomDuration));
            _zoomAnimation.AppendCallback(() =>
            {
                _curStrategyCell.ZoomBackGround.enabled = false;
                _curStrategyCell.BackgroundButtonGraphic.raycastTarget = false;
                _curStrategyCell.FrontButtonGraphic.raycastTarget = true;
                
                _zoom = false;
                _curStrategyCell = null;
                ZoomClosed?.Invoke();
            });

            _boardAnimator.AddAnimation(_zoomAnimation);
        }

        public void CloseTwoLevelZoom(bool kill = true)
        {
            if (_zoom == false) return;

            if (kill)
            {
                _zoomAnimation?.Kill();
            }

            _zoomAnimation = DOTween.Sequence();

            _curStrategyCell.AlternativeBoard.Interactable = false;

            if (_doubleZoom)
            {
                _zoomAnimation
                    .Append(_curStrategyCell.AlternativeBoard.transform.DOLocalMove(Vector3.zero, _zoomDuration))
                    .Join(_curStrategyCell.AlternativeBoard.transform.DOScale(
                        _localScaleAlternativeBoard, _zoomDuration))
                    .AppendCallback(() => _doubleZoom = false);
            }
            
            _curStrategyCell.BackgroundButtonGraphic.raycastTarget = false;
            _curStrategyCell.FrontButtonGraphic.raycastTarget = false;

            _zoomAnimation.Join(_curStrategyCell.RectTransform.DOLocalMove(_localStrategyPos, _zoomDuration))
                .Join(_curStrategyCell.RectTransform.DOScale(_localStrategyScale, _zoomDuration));

            _zoomAnimation.AppendCallback(() =>
            {
                _curStrategyCell.ZoomBackGround.enabled = false;
                _curStrategyCell.BackgroundButtonGraphic.raycastTarget = false;
                _curStrategyCell.FrontButtonGraphic.raycastTarget = true;
                
                _curStrategyCell = null;
                _zoom = false;
                ZoomClosed?.Invoke();
            });

            _boardAnimator.AddAnimation(_zoomAnimation);
        }

        public void KillZoomAnimation()
        {
            _zoomAnimation?.Kill();
        }
    }
}