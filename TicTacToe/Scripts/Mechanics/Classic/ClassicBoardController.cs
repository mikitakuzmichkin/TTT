using TicTacToe.Mechanics.Base;
using UnityEngine;

namespace TicTacToe.Mechanics.Classic
{
    public class ClassicBoardController : ABoardController<ClassicBoardView, ClassicBoardModel>
    {
        public override void Initialize(ABoardModel model, ABoardView view)
        {
            base.Initialize(model, view);

            this.view.CellClick += ViewOnCellClick;

            this.model.GameOverWithIndexes += GameOver;
        }

        public override void Uninitialize()
        {
            if (model != null)
            {
                model.GameOverWithIndexes -= GameOver;
            }

            if (view != null)
            {
                view.CellClick -= ViewOnCellClick;
            }
            
            base.Uninitialize();
        }

        private void ViewOnCellClick(Vector3Int position)
        {
            if (model.CheckCellEmpty(position))
            {
                model.SetCell(model.Turn, position);
            }
        }

        private void GameOver(ETttType type, Vector3Int[] positions)
        {
            view.ShowFinish(type, positions);
        }
    }
}