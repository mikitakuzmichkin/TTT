using System;
using System.Collections;
using DG.Tweening;
using TicTacToe.DI;
using TicTacToe.Mechanics.Base;
using TicTacToe.Mechanics.Strategy.Zoom;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Mechanics.Strategy
{
    public class StrategyBoardView : ABoardView
    {
        private const int _WIDTH = 3;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;

        [SerializeField] private StrategyCellView[] _cells;

        [Space] [SerializeField] private GameObject _winline;
        [SerializeField] private FrameAnimationComponent _winlineAnim;
        [Space]
        [SerializeField] private ZoomManager _zoomManager;
        private bool _isInteractable;

        private StrategyBoardModel _model;
        private BoardAnimatorManager _boardAnimatorManager;
        
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

        private void Awake()
        {
            Close();
        }

        public event Action<Vector3Int, Vector3Int, Vector3Int> CellClickWithPos;

        public event Action CellClick;

        public override void Initialize(ABoardModel model)
        {
            if (!(model is StrategyBoardModel))
            {
                //todo custom exception
                throw new ArgumentException("is not classic element");
            }

            Uninitialize();
            
            _boardAnimatorManager = ProjectContext.GetInstance<BoardAnimatorManager>();
            _boardAnimatorManager.Initialize(this);

            _winline.SetActive(false);

            _model = model as StrategyBoardModel;
            _model.ClassicCellChanged += ModelOnClassicCellChanged;
            _model.AlternativeCellChanged += ModelOnAlternativeCellChanged;
            _model.StrategyCellChanged += ModelOnStrategyCellChanged;
            _model.GameOverClassicBoardWithIndexes += GameOverClassicBoard;
            _model.GameOverAlternativeBoardWithIndexes += GameOverAlternativeBoard;
            _model.BlockChanged += CheckBlocked;

            for (var i = 0; i < _cells.Length; i++)
            {
                var cell = _cells[i];
                var index = new Vector3Int(i % _WIDTH, i / _WIDTH, 0);
                cell.InitializeAlternativeBoard(_model, index);
                cell.ForceSetState(_model.GetStrategyCell(index));
                cell.Click += CellOnClick;
            }

            CheckBlocked();

            foreach (var cell in _cells)
            {
                cell.AlternativeBoard.Interactable = false;
            }
        }

        public override void Uninitialize()
        {
            if (_model != null)
            {
                _model.ClassicCellChanged -= ModelOnClassicCellChanged;
                _model.AlternativeCellChanged -= ModelOnAlternativeCellChanged;
                _model.StrategyCellChanged -= ModelOnStrategyCellChanged;
                _model.GameOverClassicBoardWithIndexes -= GameOverClassicBoard;
                _model.GameOverAlternativeBoardWithIndexes -= GameOverAlternativeBoard;
                _model.BlockChanged -= CheckBlocked;
                _model = null;
            }

            foreach (var cell in _cells)
            {
                cell.Click -= CellOnClick;
            }
        }

        public override void Show()
        {
            _canvas.enabled = true;
        }

        public override void Close()
        {
            _canvas.enabled = false;
        }

        public void ShowFinish(ETttType type, Vector3Int[] positions)
        {
            // if (_zoomManager.IsZoomClosed == false)
            // {
            //     Action action = null;
            //     action = delegate
            //     {
            //         _zoomManager.ZoomClosed -= action;
            //         foreach (var cell in _cells)
            //         {
            //             cell.AlternativeBoard.ForceUnblock();
            //         }
            //         ShowFinish(type, positions);
            //     };
            //     _zoomManager.ZoomClosed += action;
            //     return;
            // }
            foreach (var cell in _cells)
            {
                cell.AlternativeBoard.ForceUnblock();
            }
            
            if (type != ETttType.Draw)
            {
                ShowWinLine(positions);
            }
        }

        private void ShowWinLine(Vector3Int[] positions)
        {
            var middlePos = positions[1];

            var cell = _cells[middlePos.y * 3 + middlePos.x];
            _winline.transform.position = cell.transform.position;
            if (positions[0].x == positions[2].x)
            {
                _winline.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                if (positions[0].y == positions[2].y)
                {
                    _winline.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else
                {
                    if (positions[0].y < positions[2].y)
                    {
                        _winline.transform.rotation = Quaternion.Euler(0, 0, 45);
                    }
                    else
                    {
                        _winline.transform.rotation = Quaternion.Euler(0, 0, -45);
                    }
                }
            }

            _winline.transform.SetAsLastSibling();
            _boardAnimatorManager.AddAnimation(_winlineAnim.ReturnAnimation());
        }

        private void ModelOnClassicCellChanged(Vector3Int strategyPos, Vector3Int alternativePos, Vector3Int classicPos)
        {
            var state = _model.GetClassicCell(strategyPos, alternativePos, classicPos);
            var flatAlternativeIndex = strategyPos.x + strategyPos.y * _WIDTH;
            _cells[flatAlternativeIndex].AlternativeBoard.SetClassicCell(state, alternativePos, classicPos);
            _zoomManager.CloseTwoLevelZoom();
        }

        private void CheckBlocked()
        {
            _boardAnimatorManager.AddAnimationCallback(() =>
            {
                if (_model == null)
                {
                    return;
                }
                
                for (var i = 0; i < _cells.Length; i++)
                {
                    var cell = _cells[i];
                    var index = new Vector3Int(i % _WIDTH, i / _WIDTH, 0);

                    cell.Block = _model.GetStrategyBlock(index);
                }
            });
        }

        private void ModelOnAlternativeCellChanged(Vector3Int strategyPos, Vector3Int alternativePos)
        {
            var state = _model.GetAlternativeCell(strategyPos, alternativePos);
            var flatIndex = strategyPos.x + strategyPos.y * _WIDTH;
            _cells[flatIndex].AlternativeBoard.SetAlternativeCell(state, alternativePos);
        }

        private void ModelOnStrategyCellChanged(Vector3Int pos)
        {
            var state = _model.GetStrategyCell(pos);
            var flatIndex = pos.x + pos.y * _WIDTH;
            _cells[flatIndex].State = state;
        }

        private void GameOverClassicBoard(Vector3Int strategyPos, Vector3Int alternativePos, ETttType type,
            Vector3Int[] indexes)
        {
            var flatIndex = strategyPos.x + strategyPos.y * _WIDTH;
            _cells[flatIndex].AlternativeBoard.GameOverClassicBoard(alternativePos, type, indexes);
        }

        private void GameOverAlternativeBoard(Vector3Int strategyPos, ETttType type, Vector3Int[] indexes)
        {
            _zoomManager.CloseOneLevelZoom();
            var flatIndex = strategyPos.x + strategyPos.y * _WIDTH;
            _cells[flatIndex].AlternativeBoard.ShowFinish(type, indexes);
            _zoomManager.CloseTwoLevelZoom(false);
            _cells[flatIndex].FrontButtonGraphic.enabled = false;
        }

        private void CellOnClick(StrategyCellView cell, Vector3Int alternativePos, Vector3Int classicPos)
        {
            var flatIndex = Array.IndexOf(_cells, cell);
            if (flatIndex == -1)
            {
                return;
            }

            var pos = new Vector3Int(flatIndex % _WIDTH, flatIndex / _WIDTH, 0);
            CellClickWithPos?.Invoke(pos, alternativePos, classicPos);
            CellClick?.Invoke();
        }

        public IEnumerator GetEnumerator()
        {
            return _cells.GetEnumerator();
        }
    }
}