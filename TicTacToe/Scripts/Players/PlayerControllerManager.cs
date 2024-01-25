using System;
using TicTacToe.Mechanics.Base;
using TicTacToe.Players.Bot;

namespace TicTacToe.Players
{
    public class PlayerControllerManager
    {
        public static APlayerController GetOfflinePlayerController(IPlayerModel model)
        {
            return new OfflinePlayerController(model);
        }

        public static APlayerController GetOfflineBotController(EGameMode mode, IPlayerModel modelPlayer,
            ABoardModel model, ABoardController controller, bool firstTurn)
        {
            switch (mode)
            {
                case EGameMode.Classic:
                    return new ClassicBotController(modelPlayer, model, controller, firstTurn);
                case EGameMode.Alternative:
                    return new AlternativeBotController(modelPlayer, model, controller);
                case EGameMode.Strategy:
                    return new StrategyBotController(modelPlayer, model, controller);
            }

            throw new PlayerControllerManagerException("this bot mode not released");
        }
    }

    public class PlayerControllerManagerException : Exception
    {
        public PlayerControllerManagerException(string message) : base(message)
        {
        }
    }
}