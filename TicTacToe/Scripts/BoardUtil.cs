using System;
using TicTacToe.Mechanics.Alternative;
using TicTacToe.Mechanics.Base;
using TicTacToe.Mechanics.Classic;
using TicTacToe.Mechanics.Strategy;

namespace TicTacToe
{
    public class BoardUtil : IBoardUtil
    {
        private readonly BoardViewsSettings _settings;

        public BoardUtil(BoardViewsSettings settings)
        {
            _settings = settings;
        }

        public ABoardController CreateController(EGameMode mode, out ABoardView viewPrefab)
        {
            var controller = GetControllerByMode(mode);

            viewPrefab = _settings.GetByType(controller.ViewType);
            if (viewPrefab == null)
            {
                throw new ArgumentException($"View for \"{controller}\" not found!");
            }

            return controller;
        }

        public ABoardModel CreateModelByMode(EGameMode mode)
        {
            switch (mode)
            {
                case EGameMode.Classic:
                    return new ClassicBoardModel();
                case EGameMode.Alternative:
                    return new AlternativeBoardModel();
                case EGameMode.Strategy:
                    return new StrategyBoardModel();
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private static ABoardController GetControllerByMode(EGameMode mode)
        {
            switch (mode)
            {
                case EGameMode.Classic:
                    return new ClassicBoardController();
                case EGameMode.Alternative:
                    return new AlternativeBoardController();
                case EGameMode.Strategy:
                    return new StrategyBoardController();
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}