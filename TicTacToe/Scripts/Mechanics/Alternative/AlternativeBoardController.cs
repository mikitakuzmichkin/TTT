using System;
using TicTacToe.Mechanics.Base;
using UnityEngine;

namespace TicTacToe.Mechanics.Alternative
{
    public class AlternativeBoardController : ABoardController
    {
        private AlternativeBoardModel _model;
        private AlternativeBoardView _view;

        public override Type ViewType => typeof(AlternativeBoardView);

        public override ABoardModel Model => _model;

        public override ABoardView View => _view;

        public override void Initialize(ABoardModel model, ABoardView view)
        {
            base.Initialize(model, view);

            if (!(model is AlternativeBoardModel) || !(view is AlternativeBoardView))
            {
                //todo custom exception
                throw new ArgumentException("Is not classic components");
            }

            _model = model as AlternativeBoardModel;

            _view = view as AlternativeBoardView;
            _view.Initialize(_model);
            _view.CellClick += ViewOnCellClick;

            _model.GameOverAlternativeBoardWithIndexes += GameOver;
        }

        public override void Uninitialize()
        {
            if (_model != null)
            {
                _model.GameOverAlternativeBoardWithIndexes -= GameOver;
            }

            if (_view != null)
            {
                _view.CellClick -= ViewOnCellClick;
            }
            
            base.Uninitialize();
        }

        private void ViewOnCellClick(Vector3Int alternativePos, Vector3Int classicPos)
        {
            if (_model.CheckCellEmpty(alternativePos, classicPos) && _model.GetBlock(alternativePos) == false)
            {
                _model.SetClassicCell(_model.Turn, alternativePos, classicPos);
            }
        }

        private void GameOver(ETttType type, Vector3Int[] positions)
        {
            _view.ShowFinish(type, positions);
        }
    }
}