using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Alternative;
using TicTacToe.Mechanics.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TicTacToe.Players.Bot
{
    public class AlternativeBotController : ABotPlayerController<AlternativeBoardModel, AlternativeBoardController>
    {
        private readonly AlternativeBoardModel _model;
        private readonly AlternativeBoardController _controller;

        public AlternativeBotController(IPlayerModel playerModel, ABoardModel model, ABoardController controller) :
            base(playerModel, model, controller)
        {
            _model = (AlternativeBoardModel) model;
            _controller = (AlternativeBoardController) controller;
        }

        public override void SetTurn()
        {
            base.SetTurn();
            BoardAnimatorManager.AddAnimationCallback( () =>
            {
                var type = _model.Turn;
                var position = FindCell(type);
                _model.SetClassicCell(type, position.alternative, position.classic);
            }, 1f);
        }

        private (Vector3Int alternative, Vector3Int classic) FindCell(ETttType yourType)
        {
            var enemyType = yourType == ETttType.Noughts ? ETttType.Cross : ETttType.Noughts;

            var alternativeBoardPriority = new EPriority[3, 3];
            CheckVerticals(alternativeBoardPriority, yourType, enemyType, _model.GetAlternativeCell);
            CheckHorizontals(alternativeBoardPriority, yourType, enemyType, _model.GetAlternativeCell);
            CheckDia(_model.GetAlternativeCell, alternativeBoardPriority, yourType, enemyType, true, 3);
            CheckDia(_model.GetAlternativeCell, alternativeBoardPriority, yourType, enemyType, false, 3);

            var alternativeCellStatus = new EAlternativeCellStatus[3, 3];

            var allCellsPriority = new List<IndexPriority>();

            CreateAlternativeStatusAndGetClassicPriority(alternativeCellStatus, allCellsPriority,
                                                         alternativeBoardPriority, yourType, enemyType);

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
                    return (emptyWay.AlternativePos, emptyWay.ClassicPos);
                }

                var notDangerousWay = FindWayToNotDangerous(list.Value, alternativeCellStatus);
                if (notDangerousWay != null)
                {
                    return (notDangerousWay.AlternativePos, notDangerousWay.ClassicPos);
                }

                var wayToLoseClassicForWinAlternative =
                    FindWayToLoseClassicForWinAlternative(list.Value, alternativeCellStatus, yourType, enemyType);
                if (wayToLoseClassicForWinAlternative != null)
                {
                    return (wayToLoseClassicForWinAlternative.AlternativePos, wayToLoseClassicForWinAlternative.ClassicPos);
                }
            }

            var max = (EPriority) allCellsPriority.Max(ip => (int) ip.Priority);
            var maxPriorityList = allCellsPriority.Where(ip => ip.Priority == max).ToList();
            var result = maxPriorityList[Random.Range(0, maxPriorityList.Count)];
            return (result.AlternativePos, result.ClassicPos);
        }

        private IndexPriority FindWayToNotDangerous(List<IndexPriority> indexPriorities,
            EAlternativeCellStatus[,] alternativeCellStatus)
        {
            var filteredList = indexPriorities
                               .Where(ip => alternativeCellStatus[ip.ClassicPos.x, ip.ClassicPos.y] ==
                                            EAlternativeCellStatus.Not_Dangerous)
                               .ToList();
            if (filteredList.Count > 0)
            {
                return filteredList[Random.Range(0, filteredList.Count())];
            }

            return null;
        }

        private IndexPriority FindWayToLoseClassicForWinAlternative(List<IndexPriority> indexPriorities,
            EAlternativeCellStatus[,] alternativeCellStatus, ETttType yourType, ETttType enemyType)
        {
            var enemyWinsList = indexPriorities.Where(
                ip =>
                {
                    var classicPos = ip.ClassicPos;
                    return alternativeCellStatus[classicPos.x, classicPos.y] ==
                           EAlternativeCellStatus.DOUBLE_YOUR_AND_ENEMY_TYPE ||
                           alternativeCellStatus[classicPos.x, classicPos.y] ==
                           EAlternativeCellStatus.DOUBLE_ENEMY_TYPE;
                }).ToList();
            var enemyWinsForYouWinsList = enemyWinsList.Where(enemyIndexPriority =>
            {
                return FindPriority(enemyIndexPriority.ClassicPos, yourType, enemyType)
                    .Any(nextIndexPriority =>
                    {
                        var classicPos = nextIndexPriority.ClassicPos;
                        return alternativeCellStatus[classicPos.x, classicPos.y] ==
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
            EAlternativeCellStatus[,] alternativeCellStatus)
        {
            var filteredList = indexPriorities
                                .Where(ip => alternativeCellStatus[ip.ClassicPos.x, ip.ClassicPos.y] ==
                                             EAlternativeCellStatus.Empty).ToList();
            if (filteredList.Count > 0)
            {
                return filteredList[Random.Range(0, filteredList.Count)];
            }

            return null;
        }

        private List<IndexPriority> DeleteFullCells(List<IndexPriority> allCellsPriority,
            EAlternativeCellStatus[,] alternativeCellStatus)
        {
            var list = new List<IndexPriority>();
            for (var i = 0; i < allCellsPriority.Count; i++)
            {
                var classicPos = allCellsPriority[i].ClassicPos;
                if (alternativeCellStatus[classicPos.x, classicPos.y] != EAlternativeCellStatus.Full)
                {
                    list.Add(allCellsPriority[i]);
                }
            }

            return list;
        }

        private List<IndexPriority> DeleteDangerousCells(List<IndexPriority> allCellsPriority,
            EAlternativeCellStatus[,] alternativeCellStatus)
        {
            var list = new List<IndexPriority>();
            for (var i = 0; i < allCellsPriority.Count; i++)
            {
                var classicPos = allCellsPriority[i].ClassicPos;
                if (alternativeCellStatus[classicPos.x, classicPos.y] != EAlternativeCellStatus.Dangerous)
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
                if (dictionary.ContainsKey(cell.Priority) == false)
                {
                    dictionary.Add(cell.Priority, new List<IndexPriority>());
                }

                dictionary[cell.Priority].Add(cell);
            }

            return dictionary;
        }

        private void CreateAlternativeStatusAndGetClassicPriority(EAlternativeCellStatus[,] alternativeCellStatus,
            List<IndexPriority> allCellsPriority, EPriority[,] alternativeBoardPriority, ETttType yourType,
            ETttType enemyType)
        {
            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    var alternativeIndex = new Vector3Int(x, y, 0);
                    if (_model.GetAlternativeCell(alternativeIndex) != ETttType.None)
                    {
                        alternativeCellStatus[x, y] = EAlternativeCellStatus.Full;
                        continue;
                    }

                    var listPriority = FindPriority(alternativeIndex, yourType, enemyType);

                    if (_model.GetBlock(alternativeIndex) == false)
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

        private List<IndexPriority> FindPriority(Vector3Int alternativePos, ETttType yourType, ETttType enemyType)
        {
            var boardPriority = new EPriority[3, 3];
            var listIndexPriority = new List<IndexPriority>();
            Func<Vector3Int, ETttType> funcGetCell = index => _model.GetClassicCell(alternativePos, index);

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
            public Vector3Int AlternativePos;
            public Vector3Int ClassicPos;
            public EPriority Priority;
        }
    }
}