using System;
using TicTacToe.Mechanics.Base;
using UnityEngine;

namespace TicTacToe.Mechanics.Strategy
{
    public class StrategyBoardController : ABoardController
    {
        private StrategyBoardModel _model;
        private StrategyBoardView _view;

        public override Type ViewType => typeof(StrategyBoardView);

        public override ABoardModel Model => _model;

        public override ABoardView View => _view;

        public override void Initialize(ABoardModel model, ABoardView view)
        {
            base.Initialize(model, view);

            if (!(model is StrategyBoardModel) || !(view is StrategyBoardView))
            {
                //todo custom exception
                throw new ArgumentException("Is not classic components");
            }

            _model = model as StrategyBoardModel;

            _view = view as StrategyBoardView;
            _view.Initialize(_model);
            _view.CellClickWithPos += ViewOnCellClickWithPos;

            _model.GameOverStrategyBoardWithIndexes += GameOver;
        }

        public override void Uninitialize()
        {
            if (_model != null)
            {
                _model.GameOverStrategyBoardWithIndexes -= GameOver;
            }

            if (_view != null)
            {
                _view.CellClickWithPos -= ViewOnCellClickWithPos;
            }
            
            base.Uninitialize();
        }

        private void ViewOnCellClickWithPos(Vector3Int strategyPos, Vector3Int alternativePos, Vector3Int classicPos)
        {
            if (_model.CheckCellEmpty(strategyPos, alternativePos, classicPos) &&
                _model.GetStrategyBlock(strategyPos) == false)
            {
                if (_model.GetAlternativeBlock(strategyPos, alternativePos))
                {
                    return;
                }

                _model.SetClassicCell(_model.Turn, strategyPos, alternativePos, classicPos);
            }
        }

        private void GameOver(ETttType type, Vector3Int[] positions)
        {
            _view.ShowFinish(type, positions);
        }
    }
}