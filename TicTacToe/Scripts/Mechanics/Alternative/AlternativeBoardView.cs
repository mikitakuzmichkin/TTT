using System;
using DG.Tweening;
using TicTacToe.DI;
using TicTacToe.Mechanics.Base;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Mechanics.Alternative
{
    public class AlternativeBoardView : ABoardView
    {
        private const int _WIDTH = 3;

#pragma warning disable 649
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;


        [Space] [SerializeField] private ImageDisable _gridImage;
        [SerializeField] private AlternativeCellView[] _cells;

        [Space] [SerializeField] private GameObject _winline;
        [SerializeField] private FrameAnimationComponent _winlineAnim;
#pragma warning restore 649

        private AlternativeBoardModel _model;
        private BoardAnimatorManager _boardAnimatorManager;

        public override bool Interactable
        {
            get => _graphicRaycaster.enabled;
            set
            {
                _graphicRaycaster.enabled = value;
                foreach (var cell in _cells)
                {
                    cell.ClassicBoard.Interactable = value;
                }
            }
        }

        public event Action<Vector3Int, Vector3Int> CellClick;

        private void Awake()
        {
            Close();
        }

        public override void Initialize(ABoardModel model)
        {
            if (!(model is AlternativeBoardModel))
            {
                //todo custom exception
                throw new ArgumentException("is not classic element");
            }

            Uninitialize();
            
            _boardAnimatorManager = ProjectContext.GetInstance<BoardAnimatorManager>();
            _boardAnimatorManager.Initialize(this);

            _winline.SetActive(false);

            _model = model as AlternativeBoardModel;
            _model.ClassicCellChanged += ModelOnClassicCellChanged;
            _model.AlternativeCellChanged += ModelOnAlternativeCellChanged;
            _model.GameOverClassicBoardWithIndexes += GameOverClassicBoard;
            _model.BlockChanged += CheckBlocked;

            for (var i = 0; i < _cells.Length; i++)
            {
                var cell = _cells[i];
                var index = new Vector3Int(i % _WIDTH, i / _WIDTH, 0);
                cell.InitializeClassicBoard(_model, index);
                cell.ForceSetState(_model.GetAlternativeCell(index));
                cell.Click += CellOnClick;
            }

            CheckBlocked();
        }

        public override void Uninitialize()
        {
            if (_model != null)
            {
                _model.ClassicCellChanged -= ModelOnClassicCellChanged;
                _model.AlternativeCellChanged -= ModelOnAlternativeCellChanged;
                _model.GameOverClassicBoardWithIndexes -= GameOverClassicBoard;
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
            for (var i = 0; i < _cells.Length; i++)
            {
                _cells[i].SetBlock(false, false);
            }
            _gridImage.isEnable = true;
            if (type != ETttType.Draw)
            {
                ShowWinLine(positions);
            }
        }

        private void ShowWinLine(Vector3Int[] positions)
        {
            var middlePos = positions[1];
            Debug.Log(middlePos);
            _winline.transform.position = _cells[middlePos.y * 3 + middlePos.x].transform.position;
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

            _boardAnimatorManager.AddAnimation(_winlineAnim.ReturnAnimation());
        }

        private void ModelOnClassicCellChanged(Vector3Int alternativePos, Vector3Int classicPos)
        {
            var state = _model.GetClassicCell(alternativePos, classicPos);
            var flatAlternativeIndex = alternativePos.x + alternativePos.y * _WIDTH;
            var flatClassicIndex = classicPos.x + classicPos.y * _WIDTH;
            _cells[flatAlternativeIndex].ClassicBoard.SetCell(flatClassicIndex, state);
        }

        private void CheckBlocked()
        {
            _boardAnimatorManager.AddAnimationCallback(() =>
            {
                bool isAllUnblocked = true;
                AlternativeCellView unblockedCell = null;
                for (var i = 0; i < _cells.Length; i++)
                {
                    var cell = _cells[i];
                    var index = new Vector3Int(i % _WIDTH, i / _WIDTH, 0);

                    var block = _model.GetBlock(index);
                    
                    if (block)
                    {
                        cell.SetBlock(true);
                    }
                    else
                    {
                        unblockedCell = cell;
                    }
                    
                    isAllUnblocked &= !block;
                }

                if (isAllUnblocked)
                {
                    for (var i = 0; i < _cells.Length; i++)
                    {
                        _cells[i].SetBlock(false, false);
                    }

                    _gridImage.isEnable = true;
                }
                else
                {
                    unblockedCell.SetBlock(false);
                    _gridImage.isEnable = false;
                }
            });
        }

        private void ModelOnAlternativeCellChanged(Vector3Int pos)
        {
            var state = _model.GetAlternativeCell(pos);
            var flatIndex = pos.x + pos.y * _WIDTH;
            _cells[flatIndex].State = state;
        }

        private void GameOverClassicBoard(Vector3Int alternativePos, ETttType type, Vector3Int[] indexes)
        {
            var flatIndex = alternativePos.x + alternativePos.y * _WIDTH;
            _cells[flatIndex].ClassicBoard.ShowFinish(type, indexes);
        }

        private void CellOnClick(AAlternativeCellView cell, Vector3Int classicPos)
        {
            var flatIndex = Array.IndexOf(_cells, cell);
            if (flatIndex == -1)
            {
                return;
            }

            var pos = new Vector3Int(flatIndex % _WIDTH, flatIndex / _WIDTH, 0);
            CellClick?.Invoke(pos, classicPos);
        }
    }
}