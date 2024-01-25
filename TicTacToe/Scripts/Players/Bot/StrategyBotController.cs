using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Base;
using TicTacToe.Mechanics.Strategy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TicTacToe.Players.Bot
{
    public class StrategyBotController : ABotPlayerController<StrategyBoardModel, StrategyBoardController>
    {
        private readonly StrategyBoardModel _model;
        private readonly StrategyBoardController _controller;

        public StrategyBotController(IPlayerModel playerModel, ABoardModel model, ABoardController controller) :
            base(playerModel, model, controller)
        {
            _model = (StrategyBoardModel) model;
            _controller = (StrategyBoardController) controller;
        }

        public override void SetTurn()
        {
            base.SetTurn();
            BoardAnimatorManager.AddAnimationCallback( () =>
            {
                var type = _model.Turn;
                var position = FindCell(type);
                _model.SetClassicCell(type, position.strategy, position.alternative, position.classic);
            }, 1f);
        }

        private (Vector3Int strategy, Vector3Int alternative, Vector3Int classic) FindCell(ETttType yourType)
        {
            var enemyType = yourType == ETttType.Noughts ? ETttType.Cross : ETttType.Noughts;

            var allCellsPriority = new List<IndexPriority>();
            var alternativeBoardPriority = new EPriority[3, 3][,];
            var alternativeCellStatus = new EAlternativeCellStatus[3, 3][,];
            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    var strategyPos = new Vector3Int(x, y, 0);
                    alternativeCellStatus[x, y] = new EAlternativeCellStatus[3, 3];

                    if (_model.GetStrategyCell(strategyPos) != ETttType.None)
                    {
                        continue;
                    }
                    
                    Func<Vector3Int, ETttType> funcGetCell = alternativePos =>
                        _model.GetAlternativeCell(strategyPos, alternativePos);

                    alternativeBoardPriority[x, y] = new EPriority[3, 3];
                    CheckVerticals(alternativeBoardPriority[x, y], yourType, enemyType, funcGetCell);
                    CheckHorizontals(alternativeBoardPriority[x, y], yourType, enemyType, funcGetCell);
                    CheckDia(funcGetCell, alternativeBoardPriority[x, y], yourType, enemyType, true, 3);
                    CheckDia(funcGetCell, alternativeBoardPriority[x, y], yourType, enemyType, false, 3);

                    CreateAlternativeStatusAndGetClassicPriority(strategyPos, alternativeCellStatus[x, y],
                                                                 allCellsPriority,
                                                                 alternativeBoardPriority[x, y], yourType, enemyType);
                }
            }

            var allCellFilteredDangerous = DeleteDangerousCells(allCellsPriority, alternativeCellStatus);
            if (allCellFilteredDangerous.Count == 0)
            {
                allCellFilteredDangerous = allCellsPriority;
            }

            var allCellFilteredFull = DeleteFullCells(allCellFilteredDangerous, alternativeCellStatus);
            if (allCellFilteredFull.Count == 0)
            {
                allCellFilteredFull = allCellFilteredDangerous;
            }

            var dictionary = SortToDictionary(allCellFilteredFull);

            foreach (var list in dictionary.OrderByDescending(key => key.Key))
            {
                var emptyWay = FindWayToEmpty(list.Value, alternativeCellStatus);
                if (emptyWay != null)
                {
                    return (emptyWay.StrategyPos, emptyWay.AlternativePos, emptyWay.ClassicPos);
                }

                var notDangerousWay = FindWayToNotDangerous(list.Value, alternativeCellStatus);
                if (notDangerousWay != null)
                {
                    return (notDangerousWay.StrategyPos, notDangerousWay.AlternativePos, notDangerousWay.ClassicPos);
                }

                var wayToLoseClassicForWinAlternative =
                    FindWayToLoseClassicForWinAlternative(list.Value, alternativeCellStatus, yourType, enemyType);
                if (wayToLoseClassicForWinAlternative != null)
                {
                    return (wayToLoseClassicForWinAlternative.StrategyPos,
                            wayToLoseClassicForWinAlternative.AlternativePos,
                            wayToLoseClassicForWinAlternative.ClassicPos);
                }
            }

            var max = (EPriority) allCellsPriority.Max(ip => (int) ip.Priority);
            var maxPriorityList = allCellsPriority.Where(ip => ip.Priority == max).ToList();
            var result = maxPriorityList[Random.Range(0, maxPriorityList.Count)];
            return (result.StrategyPos, result.AlternativePos, result.ClassicPos);
        }

        private IndexPriority FindWayToNotDangerous(List<IndexPriority> indexPriorities,
            EAlternativeCellStatus[,][,] alternativeCellStatusInStrategy)
        {
            var filteredList = indexPriorities
                               .Where(ip => alternativeCellStatusInStrategy[ip.AlternativePos.x, ip.AlternativePos.y]
                                          [ip.ClassicPos.x, ip.ClassicPos.y] == EAlternativeCellStatus.Not_Dangerous)
                               .ToList();
            if (filteredList.Count > 0)
            {
                return filteredList[Random.Range(0, filteredList.Count)];
            }

            return null;
        }

        private IndexPriority FindWayToLoseClassicForWinAlternative(List<IndexPriority> indexPriorities,
            EAlternativeCellStatus[,][,] alternativeCellStatusInStrategy, ETttType yourType, ETttType enemyType)
        {
            var enemyWinsList = indexPriorities.Where(
                ip =>
                {
                    var classicPos = ip.ClassicPos;
                    var alternativePos = ip.AlternativePos;
                    return alternativeCellStatusInStrategy[alternativePos.x, alternativePos.y][
                               classicPos.x, classicPos.y] ==
                           EAlternativeCellStatus.DOUBLE_YOUR_AND_ENEMY_TYPE ||
                           alternativeCellStatusInStrategy[alternativePos.x, alternativePos.y][
                               classicPos.x, classicPos.y] ==
                           EAlternativeCellStatus.DOUBLE_ENEMY_TYPE;
                });
            var enemyWinsForYouWinsList = enemyWinsList.Where(enemyIndexPriority =>
            {
                return FindPriority(enemyIndexPriority.AlternativePos, enemyIndexPriority.ClassicPos, yourType,
                                    enemyType)
                    .Any(nextIndexPriority =>
                    {
                        var classicPos = nextIndexPriority.ClassicPos;
                        var alternativePos = nextIndexPriority.AlternativePos;
                        return alternativeCellStatusInStrategy[alternativePos.x, alternativePos.y][
                                   classicPos.x, classicPos.y] ==
                               EAlternativeCellStatus.DOUBLE_YOUR_TYPE;
                    });
            }).ToList();

            if (enemyWinsForYouWinsList.Count > 0)
            {
                return enemyWinsForYouWinsList[Random.Range(0, enemyWinsForYouWinsList.Count)];
            }

            return null;
        }

        private IndexPriority FindWayToEmpty(List<IndexPriority> indexPriorities,
            EAlternativeCellStatus[,][,] alternativeCellStatusInStrategy)
        {
            var filteredList = indexPriorities
                               .Where(ip => alternativeCellStatusInStrategy[ip.AlternativePos.x, ip.AlternativePos.y]
                                                [ip.ClassicPos.x, ip.ClassicPos.y] ==
                                            EAlternativeCellStatus.Empty).ToList();
            if (filteredList.Count > 0)
            {
                return filteredList[Random.Range(0, filteredList.Count)];
            }

            return null;
        }

        private List<IndexPriority> DeleteFullCells(List<IndexPriority> allCellsPriority,
            EAlternativeCellStatus[,][,] alternativeCellStatusInStrategy)
        {
            var list = new List<IndexPriority>();
            for (var i = 0; i < allCellsPriority.Count; i++)
            {
                var classicPos = allCellsPriority[i].ClassicPos;
                var alternativePos = allCellsPriority[i].AlternativePos;
                if (alternativeCellStatusInStrategy[alternativePos.x, alternativePos.y][classicPos.x, classicPos.y] !=
                    EAlternativeCellStatus.Full)
                {
                    list.Add(allCellsPriority[i]);
                }
            }

            return list;
        }

        private List<IndexPriority> DeleteDangerousCells(List<IndexPriority> allCellsPriority,
            EAlternativeCellStatus[,][,] alternativeCellStatusInStrategy)
        {
            var list = new List<IndexPriority>();
            for (var i = 0; i < allCellsPriority.Count; i++)
            {
                var classicPos = allCellsPriority[i].ClassicPos;
                var alternativePos = allCellsPriority[i].AlternativePos;
                if (alternativeCellStatusInStrategy[alternativePos.x, alternativePos.y][classicPos.x, classicPos.y] !=
                    EAlternativeCellStatus.Dangerous)
                {
                    list.Add(allCellsPriority[i]);
                }
            }

            return list;
        }

        private Dictionary<EPriority, List<IndexPriority>> SortToDictionary(List<IndexPriority> cells)
        {
            var dictionary = new Dictionary<EPriority, List<IndexPriority>>();
            foreach (var cell in cells)
            {
                if (!dictionary.ContainsKey(cell.Priority))
                {
                    dictionary.Add(cell.Priority, new List<IndexPriority>());
                }

                dictionary[cell.Priority].Add(cell);
            }

            return dictionary;
        }

        private void CreateAlternativeStatusAndGetClassicPriority(Vector3Int strategyPos,
            EAlternativeCellStatus[,] alternativeCellStatus,
            List<IndexPriority> allCellsPriority, EPriority[,] alternativeBoardPriority, ETttType yourType,
            ETttType enemyType)
        {
            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    var alternativeIndex = new Vector3Int(x, y, 0);
                    if (_model.GetAlternativeCell(strategyPos, alternativeIndex) != ETttType.None)
                    {
                        alternativeCellStatus[x, y] = EAlternativeCellStatus.Full;
                        continue;
                    }

                    var listPriority = FindPriority(strategyPos, alternativeIndex, yourType, enemyType);

                    if (!(_model.GetStrategyBlock(strategyPos) ||
                          _model.GetAlternativeBlock(strategyPos, alternativeIndex)))
                    {
                        allCellsPriority.AddRange(listPriority);
                    }

                    if (alternativeBoardPriority[x, y] == EPriority.DOUBLE_ENEMY_TYPE)
                    {
                        alternativeCellStatus[x, y] = EAlternativeCellStatus.Dangerous;
                    }

                    if (listPriority.TrueForAll(p => p.Priority == EPriority.ALL_EMPTY))
                    {
                        alternativeCellStatus[x, y] = EAlternativeCellStatus.Empty;
                        continue;
                    }

                    if (alternativeCellStatus[x, y] == EAlternativeCellStatus.None)
                    {
                        var enemyDoubleType = listPriority.Any(p => p.Priority == EPriority.DOUBLE_ENEMY_TYPE);
                        var yourDoubleType = listPriority.Any(p => p.Priority == EPriority.DOUBLE_YOUR_TYPE);
                        if (enemyDoubleType && yourDoubleType)
                        {
                            alternativeCellStatus[x, y] = EAlternativeCellStatus.DOUBLE_YOUR_AND_ENEMY_TYPE;
                            continue;
                        }

                        if (enemyDoubleType)
                        {
                            alternativeCellStatus[x, y] = EAlternativeCellStatus.DOUBLE_ENEMY_TYPE;
                            continue;
                        }

                        if (yourDoubleType)
                        {
                            alternativeCellStatus[x, y] = EAlternativeCellStatus.DOUBLE_YOUR_TYPE;
                            continue;
                        }

                        alternativeCellStatus[x, y] = EAlternativeCellStatus.Not_Dangerous;
                    }
                }
            }
        }

        private List<IndexPriority> FindPriority(Vector3Int strategyPos, Vector3Int alternativePos, ETttType yourType,
            ETttType enemyType)
        {
            var boardPriority = new EPriority[3, 3];
            var listIndexPriority = new List<IndexPriority>();
            Func<Vector3Int, ETttType> funcGetCell = index => _model.GetClassicCell(strategyPos, alternativePos, index);

            CheckVerticals(boardPriority, yourType, enemyType, funcGetCell);
            CheckHorizontals(boardPriority, yourType, enemyType, funcGetCell);
            CheckDia(funcGetCell, boardPriority, yourType, enemyType, true, 3);
            CheckDia(funcGetCell, boardPriority, yourType, enemyType, false, 3);

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    if (boardPriority[x, y] > 0)
                    {
                        listIndexPriority.Add(new IndexPriority
                        {
                            StrategyPos = strategyPos,
                            AlternativePos = alternativePos,
                            ClassicPos = new Vector3Int(x, y, 0),
                            Priority = boardPriority[x, y]
                        });
                    }
                }
            }

            return listIndexPriority;
        }

        private void CheckVerticals(EPriority[,] boardPriority, ETttType yourType, ETttType enemyType,
            Func<Vector3Int, ETttType> funcGetCell)
        {
            for (var x = 0; x < 3; x++)
            {
                var yourTypeCount = 0;
                var enemyTypeCount = 0;
                var emptyTypeCount = 0;

                for (var y = 0; y < 3; y++)
                {
                    var cellType = funcGetCell(new Vector3Int(x, y, 0));
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
                    if (funcGetCell(new Vector3Int(x, y, 0)) == ETttType.None)
                    {
                        if ((int) boardPriority[x, y] < (int) priority)
                        {
                            boardPriority[x, y] = priority;
                        }
                    }
                }
            }
        }

        private void CheckHorizontals(EPriority[,] boardPriority, ETttType yourType, ETttType enemyType,
            Func<Vector3Int, ETttType> funcGetCell)
        {
            for (var y = 0; y < 3; y++)
            {
                var yourTypeCount = 0;
                var enemyTypeCount = 0;
                var emptyTypeCount = 0;

                for (var x = 0; x < 3; x++)
                {
                    var cellType = funcGetCell(new Vector3Int(x, y, 0));
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
                    if (funcGetCell(new Vector3Int(x, y, 0)) == ETttType.None)
                    {
                        if ((int) boardPriority[x, y] < (int) priority)
                        {
                            boardPriority[x, y] = priority;
                        }
                    }
                }
            }
        }

        private void CheckDia(Func<Vector3Int, ETttType> funcGetCell, EPriority[,] boardPriority, ETttType yourType,
            ETttType enemyType, bool secondary, int length)
        {
            var yourTypeCount = 0;
            var enemyTypeCount = 0;
            var emptyTypeCount = 0;

            var positions = new Vector3Int[Mathf.Abs(length)];
            var yStart = secondary ? length - 1 : 0;
            var yIncrement = secondary ? -1 : 1;
            for (int y = yStart, x = 0; x < length; y += yIncrement, x++)
            {
                var cellType = funcGetCell(new Vector3Int(x, y, 0));
                positions[x] = new Vector3Int(x, y, 0);
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
            //positions[length - 1] = new Vector3Int(length -1, (yStart + yIncrement * (length - 1)) , 0);

            if (emptyTypeCount == 0)
            {
                return;
            }

            var priority = GetPriority(yourTypeCount, enemyTypeCount, emptyTypeCount);

            foreach (var position in positions)
            {
                if (funcGetCell(position) == ETttType.None)
                {
                    if ((int) boardPriority[position.x, position.y] < (int) priority)
                    {
                        boardPriority[position.x, position.y] = priority;
                    }
                }
            }
        }

        private EPriority GetPriority(int yourTypeCount, int enemyTypeCount, int emptyCount)
        {
            if (yourTypeCount == 2)
            {
                return EPriority.DOUBLE_YOUR_TYPE;
            }

            if (enemyTypeCount == 2)
            {
                return EPriority.DOUBLE_ENEMY_TYPE;
            }

            if (yourTypeCount == 1 && emptyCount == 2)
            {
                return EPriority.UNO_YOUR_UNO_EMPTY_TYPE;
            }

            if (emptyCount == 3)
            {
                return EPriority.ALL_EMPTY;
            }

            return EPriority.ETC_PRIORITY;
        }

        private enum EPriority
        {
            None = 0,
            ETC_PRIORITY = 1,
            ALL_EMPTY = 2,
            UNO_YOUR_UNO_EMPTY_TYPE = 3,
            DOUBLE_ENEMY_TYPE = 4,
            DOUBLE_YOUR_TYPE = 5
        }

        private enum EAlternativeCellStatus
        {
            None,
            Empty,
            Full,
            Not_Dangerous,
            DOUBLE_YOUR_TYPE,
            DOUBLE_ENEMY_TYPE,
            DOUBLE_YOUR_AND_ENEMY_TYPE,
            Dangerous
        }

        private class IndexPriority
        {
            public Vector3Int StrategyPos;
            public Vector3Int AlternativePos;
            public Vector3Int ClassicPos;
            public EPriority Priority;
        }
    }
}