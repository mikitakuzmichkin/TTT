using System;
using TicTacToe.DI;
using TicTacToe.Mechanics.Base;

namespace TicTacToe.Players.Bot
{
    public abstract class ABotPlayerController<TModel, TController> : APlayerController
        where TModel : ABoardModel
        where TController : ABoardController
    {
        protected BoardAnimatorManager BoardAnimatorManager;
        public ABotPlayerController(IPlayerModel modelPlayer, ABoardModel model, ABoardController controller)
            : base(modelPlayer)
        {
            if (!(model is TModel) || !(controller is TController))
            {
                //todo custom exception
                throw new ArgumentException("Components type is invalid!");
            }

            BoardAnimatorManager = ProjectContext.GetInstance<BoardAnimatorManager>();
        }
    }
}