using System;
using System.Collections;
using TicTacToe.DI;
using TicTacToe.Mechanics.Base;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Mechanics.Strategy
{
    public class BoardViewAlternative4Strategy : MonoBehaviour
    {
        private const int _WIDTH = 3;

        [Space] [SerializeField] private ImageDisable _gridImage;
        [SerializeField] private CellViewAlternative4Strategy[] _cells;

        [Space] [SerializeField] private GameObject _winline;
        [SerializeField] private FrameAnimationComponent _winlineAnim;

        private StrategyBoardModel _strategyModel;
        private BoardAnimatorManager _boardAnimatorManager;
        private Vector3Int _strategyPos;
        private bool _isInteractable;

        public bool Interactable
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

        public bool Block
        {
            set
            {
                if (value)
                {
                    BlockBoard();
                }
                else
                {
                    CheckBlocked(_strategyPos);
                }
            }
        }

        public event Action<Vector3Int, Vector3Int> CellClick;

        public void Initialize(StrategyBoardModel model, Vector3Int strategyPos)
        {
            Uninitialize();
            
            _boardAnimatorManager = ProjectContext.GetInstance<BoardAnimatorManager>();

            _strategyPos = strategyPos;

            _winline.SetActive(false);

            _strategyModel = model;

            for (var i = 0; i < _cells.Length; i++)
            {
                var cell = _cells[i];
                var index = new Vector3Int(i % _WIDTH, i / _WIDTH, 0);
                cell.InitializeClassicBoard(model, strategyPos, index);
                cell.ForceSetState(model.GetAlternativeCell(strategyPos, index));
                cell.Click += CellOnClick;
            }
        }

        public void Uninitialize()
        {
            foreach (var cell in _cells)
            {
                cell.Click -= CellOnClick;
            }
        }

        public void ShowFinish(ETttType type, Vector3Int[] positions)
        {
            ForceUnblock();
            if (type != ETttType.Draw)
            {
                ShowWinLine(positions);
            }
        }

        private void ShowWinLine(Vector3Int[] positions)
        {
            var middlePos = positions[1];

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

        public void SetClassicCell(ETttType state, Vector3Int alternativePos, Vector3Int classicPos)
        {
            var flatAlternativeIndex = alternativePos.x + alternativePos.y * _WIDTH;
            var flatClassicIndex = classicPos.x + classicPos.y * _WIDTH;
            _cells[flatAlternativeIndex].ClassicBoard.SetCell(flatClassicIndex, state);
        }

        private void CheckBlocked(Vector3Int strategyPos)
        {
            var isAllUnblocked = true;
            CellViewAlternative4Strategy unblockedCell = null;
            for (var i = 0; i < _cells.Length; i++)
            {
                var cell = _cells[i];
                var index = new Vector3Int(i % _WIDTH, i / _WIDTH, 0);

                var block = _strategyModel.GetAlternativeBlock(strategyPos, index);

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
        }

        private void BlockBoard()
        {
            for (var i = 0; i < _cells.Length; i++)
            {
                var cell = _cells[i];
                var index = new Vector3Int(i % _WIDTH, i / _WIDTH, 0);

                cell.SetBlock(true);
            }

            _gridImage.isEnable = false;
        }

        public void ForceUnblock()
        {
            for (var i = 0; i < _cells.Length; i++)
            {
                _cells[i].SetBlock(false, false);
            }
            _gridImage.isEnable = true;
        }

        public void SetAlternativeCell(ETttType state, Vector3Int pos)
        {
            var flatIndex = pos.x + pos.y * _WIDTH;
            _cells[flatIndex].State = state;
        }

        public void GameOverClassicBoard(Vector3Int alternativePos, ETttType type, Vector3Int[] indexes)
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

        public IEnumerator GetEnumerator()
        {
            return _cells.GetEnumerator();
        }
    }
}