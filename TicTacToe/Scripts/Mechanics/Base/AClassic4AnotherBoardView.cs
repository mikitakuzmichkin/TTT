using System;
using TicTacToe.DI;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Mechanics.Base
{
    public abstract class AClassic4AnotherBoardView : MonoBehaviour
    {
        protected const int _WIDTH = 3;

#pragma warning disable 649
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;

        [Space] [SerializeField] private Image _gridImage;
        [SerializeField] protected CellView[] _cells;

        [Space] [SerializeField] protected GameObject _winline;
        [SerializeField] private FrameAnimationComponent _winlineAnim;
#pragma warning restore 649
        
        private BoardAnimatorManager _boardAnimatorManager;

        public virtual bool Interactable
        {
            get => _graphicRaycaster.enabled;
            set => _graphicRaycaster.enabled = value;
        }

        public event Action<Vector3Int> CellClick;

        public void SetCell(int index, ETttType state)
        {
            _cells[index].State = state;
        }

        protected void Initialize()
        {
            _boardAnimatorManager = ProjectContext.GetInstance<BoardAnimatorManager>();
        }

        public void Uninitialize()
        {
            //model.GameOverClassicBoardWithIndexes -= ShowFinish;
            foreach (var cell in _cells)
            {
                cell.Click -= CellOnClick;
            }
        }

        public virtual void ShowFinish(ETttType type, Vector3Int[] positions)
        {
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

        public void ModelOnCellChanged(Vector3Int pos, ETttType state)
        {
            var flatIndex = pos.x + pos.y * _WIDTH;
            _cells[flatIndex].State = state;
        }

        protected void CellOnClick(CellView cell)
        {
            var flatIndex = Array.IndexOf(_cells, cell);
            if (flatIndex == -1)
            {
                return;
            }

            var pos = new Vector3Int(flatIndex % _WIDTH, flatIndex / _WIDTH, 0);
            CellClick?.Invoke(pos);
        }
    }
}