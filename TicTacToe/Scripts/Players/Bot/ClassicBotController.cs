using System.Linq;
using DG.Tweening;
using TicTacToe.Extensions;
using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Base;
using TicTacToe.Mechanics.Classic;
using UnityEngine;

namespace TicTacToe.Players.Bot
{
    public class ClassicBotController : ABotPlayerController<ClassicBoardModel, ClassicBoardController>
    {
        private const int DOUBLE_YOUR_TYPE = 100;
        private const int DOUBLE_ENEMY_TYPE = 50;
        private const int UNO_YOUR_UNO_EMPTY_TYPE = 10;
        private const int ALL_EMPTY = 5;
        private const int ETC_PRIORITY = 1;

        private static readonly Vector3Int[] _FirstStepVariants =
        {
            new Vector3Int(0, 0, 0), new Vector3Int(0, 2, 0), new Vector3Int(2, 2, 0), new Vector3Int(2, 0, 0),
            new Vector3Int(1, 1, 0)
        };

        private readonly ClassicBoardModel _model;
        private readonly ClassicBoardController _controller;
        private bool _firstStep;

        public ClassicBotController(IPlayerModel playerModel, ABoardModel model, ABoardController controller, bool firstStep) : base(
            playerModel, model, controller)
        {
            _model = (ClassicBoardModel) model;
            _controller = (ClassicBoardController) controller;
            _firstStep = firstStep;
        }

        public override void SetTurn()
        {
            base.SetTurn();
            BoardAnimatorManager.AddAnimationCallback( () =>
            {
                var type = _model.Turn;
                _model.SetCell(type, FindCell(type));
            }, 1f);
        }

        private Vector3Int FindCell(ETttType yourType)
        {
            if (_firstStep)
            {
                _firstStep = false;
                return FindCellFirstStep(_model);
            }

            var enemyType = yourType == ETttType.Noughts ? ETttType.Cross : ETttType.Noughts;

            var board = new int[3, 3];

            CheckVerticals(board, yourType, enemyType);
            CheckHorizontals(board, yourType, enemyType);
            CheckDiagonals(board, yourType, enemyType, true, 3);
            CheckDiagonals(board, yourType, enemyType, false, 3);

            var max = board.Cast<int>().Max();
            return board.GetRandomSameElementsIndex(max).ToVector3Int();
        }

        private void CheckVerticals(int[,] board, ETttType yourType, ETttType enemyType)
        {
            for (var x = 0; x < 3; x++)
            {
                var yourTypeCount = 0;
                var enemyTypeCount = 0;
                var emptyTypeCount = 0;

                for (var y = 0; y < 3; y++)
                {
                    var cellType = _model.GetCell(new Vector3Int(x, y, 0));
                    if (cellType == yourType)
                    {
                        yourTypeCount++;
                        continue;
                    }

                    if (cellType == enemyType)
                    {
                        enemyTypeCount++;
                        continue;
                    }

                    if (cellType == ETttType.None)
                    {
                        emptyTypeCount++;
                    }
                }

                if (emptyTypeCount == 0)
                {
                    continue;
                }

                var priority = GetPriority(yourTypeCount, enemyTypeCount, emptyTypeCount);

                for (var y = 0; y < 3; y++)
                {
                    if (_model.GetCell(new Vector3Int(x, y, 0)) == ETttType.None)
                    {
                        board[x, y] += priority;
                    }
                }
            }
        }

        private void CheckHorizontals(int[,] board, ETttType yourType, ETttType enemyType)
        {
            for (var y = 0; y < 3; y++)
            {
                var yourTypeCount = 0;
                var enemyTypeCount = 0;
                var emptyTypeCount = 0;

                for (var x = 0; x < 3; x++)
                {
                    var cellType = _model.GetCell(new Vector3Int(x, y, 0));
                    if (cellType == yourType)
                    {
                        yourTypeCount++;
                        continue;
                    }

                    if (cellType == enemyType)
                    {
                        enemyTypeCount++;
                        continue;
                    }

                    if (cellType == ETttType.None)
                    {
                        emptyTypeCount++;
                    }
                }

                if (emptyTypeCount == 0)
                {
                    continue;
                }

                var priority = GetPriority(yourTypeCount, enemyTypeCount, emptyTypeCount);

                for (var x = 0; x < 3; x++)
                {
                    if (_model.GetCell(new Vector3Int(x, y, 0)) == ETttType.None)
                    {
                        board[x, y] += priority;
                    }
                }
            }
        }

        private void CheckDiagonals(int[,] board, ETttType yourType, ETttType enemyType, bool secondary, int length)
        {
            var yourTypeCount = 0;
            var enemyTypeCount = 0;
            var emptyTypeCount = 0;

            var positions = new Vector3Int[Mathf.Abs(length)];
            var yStart = secondary ? length - 1 : 0;
            var yIncrement = secondary ? -1 : 1;
            for (int y = yStart, x = 0; x < length - 1; y += yIncrement, x++)
            {
                var cellType = _model.GetCell(new Vector3Int(x, y, 0));
                if (cellType == yourType)
                {
                    yourTypeCount++;
                    continue;
                }

                if (cellType == enemyType)
                {
                    enemyTypeCount++;
                    continue;
                }

                if (cellType == ETttType.None)
                {
                    emptyTypeCount++;
                    continue;
                }

                positions[x].Set(x, y, 0);
            }

            if (emptyTypeCount == 0)
            {
                return;
            }

            var priority = GetPriority(yourTypeCount, enemyTypeCount, emptyTypeCount);

            foreach (var position in positions)
            {
                if (_model.GetCell(position) == ETttType.None)
                {
                    board[position.x, position.y] += priority;
                }
            }
        }

        private int GetPriority(int yourTypeCount, int enemyTypeCount, int emptyCount)
        {
            if (yourTypeCount == 2)
            {
                return DOUBLE_YOUR_TYPE;
            }

            if (enemyTypeCount == 2)
            {
                return DOUBLE_ENEMY_TYPE;
            }

            if (yourTypeCount == 1 && emptyCount == 2)
            {
                return UNO_YOUR_UNO_EMPTY_TYPE;
            }

            if (emptyCount == 3)
            {
                return ALL_EMPTY;
            }

            return ETC_PRIORITY;
        }

        private static Vector3Int FindCellFirstStep(ClassicBoardModel model)
        {
            var filteredVariants = _FirstStepVariants.Where(i => model.GetCell(i) == ETttType.None);
            var count = filteredVariants.Count();
            return filteredVariants.ElementAt(Random.Range(0, count));
        }
    }
}