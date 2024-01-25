using System;
using TicTacToe.DI;
using TicTacToe.InteractiveTutorial;
using TicTacToe.Mechanics.Base;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Mechanics.Classic
{
    public class ClassicBoardView : ABoardView
    {
        private const int _WIDTH = 3;

#pragma warning disable 649
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;

        [Space] [SerializeField] private Image _gridImage;
        [SerializeField] private CellView[] _cells;

        [Space] [SerializeField] private GameObject _winline;
        [SerializeField] private FrameAnimationComponent _winlineAnim;
#pragma warning restore 649

        private ClassicBoardModel _model;
        private BoardAnimatorManager _boardAnimatorManager;

        public override bool Interactable
        {
            get => _graphicRaycaster.enabled;
            set => _graphicRaycaster.enabled = value;
        }

        public event Action<Vector3Int> CellClick;

        private void Awake()
        {
            Close();
        }

        public override void Initialize(ABoardModel model)
        {
            if (!(model is ClassicBoardModel))
            {
                //todo custom exception
                throw new ArgumentException("is not classic element");
            }

            Uninitialize();

            _boardAnimatorManager = ProjectContext.GetInstance<BoardAnimatorManager>();
            _boardAnimatorManager.Initialize(this);

            _winline.SetActive(false);

            _model = model as ClassicBoardModel;
            _model.CellChanged += ModelOnCellChanged;

            for (var i = 0; i < _cells.Length; i++)
            {
                var cell = _cells[i];
                cell.ForceSetState(_model.GetCell(new Vector3Int(i % _WIDTH, i / _WIDTH, 0)));
                cell.Click += CellOnClick;
            }
            
        }

        public override void Uninitialize()
        {
            if (_model != null)
            {
                _model.CellChanged -= ModelOnCellChanged;
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

        public virtual void ShowFinish(ETttType type, Vector3Int[] positions)
        {
            if (type != ETttType.Draw)
            {
                ShowWinLine(positions);
            }
        }

        public override void SetInteractiveTutorial(AInteractiveTutorial interactiveTutorial)
        {
            var tutorial = (InteractiveTutorialForClassic) interactiveTutorial;
            tutorial.Initialize(_cells);
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

        private void ModelOnCellChanged(Vector3Int pos)
        {
            var state = _model.GetCell(pos);
            var flatIndex = pos.x + pos.y * _WIDTH;
            _cells[flatIndex].State = state;
        }

        private void CellOnClick(CellView cell)
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