using System;
using Newtonsoft.Json;
using TicTacToe.Mechanics.Alternative;
using TicTacToe.Mechanics.Base;
using TicTacToe.Mechanics.Classic;
using UnityEngine;

namespace TicTacToe.Mechanics.Strategy
{
    public class StrategyBoardModel : ABoardModel
    {
        private const int _STRATEGYCOLUMNS = 3;
        private const int _STRATEGYROWS = 3;
        private const int _ALTERNATIVECOLUMNS = 3;
        private const int _ALTERNATIVEROWS = 3;
        private const int _COLUMNS = 3;
        private const int _ROWS = 3;
        private const int _LENGTH = 3;

        [JsonProperty] private StrategyBoard<ETttType> _board;
        private bool _gameOver;

        //todo unit test
        public event Action<ETttType, Vector3Int[]> GameOverStrategyBoardWithIndexes;
        public event Action<Vector3Int, ETttType, Vector3Int[]> GameOverAlternativeBoardWithIndexes;
        public event Action<Vector3Int, Vector3Int, ETttType, Vector3Int[]> GameOverClassicBoardWithIndexes;
        public event Action ModelUpdate;
        public event Action<Vector3Int> StrategyCellChanged;
        public event Action<Vector3Int, Vector3Int> AlternativeCellChanged;
        public event Action<Vector3Int, Vector3Int, Vector3Int> ClassicCellChanged;
        public event Action BlockChanged;
        public override event Action<ETttType> GameOver;

        public StrategyBoardModel()
        {
            _board = new StrategyBoard<ETttType>(_STRATEGYROWS, _STRATEGYCOLUMNS, _ALTERNATIVEROWS, _ALTERNATIVECOLUMNS,
                                                 _ROWS, _COLUMNS);
            _gameOver = false;
            Turn = ETttType.Cross;
        }

        public StrategyBoardModel(StrategyBoard<ETttType> board)
        {
            _board = board;
            _gameOver = false;
            Turn = ETttType.Cross;
        }

        public StrategyBoardModel(StrategyBoard<ETttType> board, ETttType turn)
        {
            _board = board;
            _gameOver = false;
            Turn = turn;
        }

        public override void Uninitialize()
        {
            GameOverStrategyBoardWithIndexes = null;
            GameOverAlternativeBoardWithIndexes = null;
            GameOverClassicBoardWithIndexes = null;
            ModelUpdate = null;
            StrategyCellChanged = null;
            AlternativeCellChanged = null;
            ClassicCellChanged = null;
            BlockChanged = null;
            GameOver = null;
            base.Uninitialize();
        }

#if DEV
        public override void RunCheatAction(ETttType winSign, CheatAction cheatAction)
        {
            ETttType type;
            switch (cheatAction)
            {
                case CheatAction.Win:
                    type = winSign;
                    break;
                case CheatAction.Lose:
                    type = winSign == ETttType.Cross ? ETttType.Noughts : ETttType.Cross;
                    break;
                case CheatAction.Draw:
                    type = ETttType.Draw;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cheatAction), cheatAction, null);
            }

            GameOverBoard(type, new[] {new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1), new Vector3Int(2, 2, 2)});
        }
#endif

        public bool CheckCellEmpty(Vector3Int strategyPos, Vector3Int alternativePos, Vector3Int classicPos)
        {
            return _board[strategyPos.x, strategyPos.y, alternativePos.x, alternativePos.y, classicPos.x,
                          classicPos.y] == ETttType.None;
        }

        public void SetClassicCell(ETttType type, Vector3Int strategyPos, Vector3Int alternativePos,
            Vector3Int classicPos)
        {
            if (CheckCellEmpty(strategyPos, alternativePos, classicPos) == false)
            {
                throw new BoardModelException("this cell allready full");
            }

            //Classic
            _board[strategyPos.x, strategyPos.y, alternativePos.x, alternativePos.y, classicPos.x, classicPos.y] = type;

            ClassicCellChanged?.Invoke(strategyPos, alternativePos, classicPos);

            //Check Alternative
            var alternativeCell = ETttType.None;
            Action<ETttType, Vector3Int[]> func = delegate(ETttType locType, Vector3Int[] array)
            {
                alternativeCell = locType;
                GameOverClassicBoardWithIndexes?.Invoke(strategyPos, alternativePos, locType, array);
            };

            ClassicGameOverChecker.GameOverCheck(
                _board.GetClassicBoard(strategyPos.x, strategyPos.y, alternativePos.x, alternativePos.y), _COLUMNS,
                _ROWS, _LENGTH, func);

            if (alternativeCell != ETttType.None)
            {
                //Alternative
                _board[strategyPos.x, strategyPos.y, alternativePos.x, alternativePos.y] = alternativeCell;
                AlternativeCellChanged(strategyPos, alternativePos);

                //Check Strategy
                var strategyCell = ETttType.None;
                Action<ETttType, Vector3Int[]> funcStrategy = delegate(ETttType locType, Vector3Int[] array)
                {
                    strategyCell = locType;
                    GameOverAlternativeBoardWithIndexes?.Invoke(strategyPos, locType, array);
                };

                AlternativeGameOverChecker.GameOverCheck(_board.GetAlternativeBoard(strategyPos.x, strategyPos.y),
                                                         _ALTERNATIVECOLUMNS, _ALTERNATIVEROWS, _LENGTH, funcStrategy);

                if (strategyCell != ETttType.None)
                {
                    _board[strategyPos.x, strategyPos.y] = strategyCell;
                    StrategyCellChanged(strategyPos);

                    GameOverCheck();
                }
            }

            SetBlock(alternativePos, classicPos);

            ModelUpdate?.Invoke();

            if (Turn == ETttType.Cross)
            {
                Turn = ETttType.Noughts;
            }
            else
            {
                Turn = ETttType.Cross;
            }
        }

        public bool GetAlternativeBlock(Vector3Int strategyPos, Vector3Int alternativePos)
        {
            return _board.IsAlternativeCellBlocked(strategyPos.x, strategyPos.y, alternativePos.x, alternativePos.y);
        }

        public bool GetStrategyBlock(Vector3Int strategyPos)
        {
            return _board.IsStrategyCellBlocked(strategyPos.x, strategyPos.y);
        }

        public ETttType GetStrategyCell(Vector3Int strategyPos)
        {
            return _board[strategyPos.x, strategyPos.y];
        }

        public ETttType GetAlternativeCell(Vector3Int strategyPos, Vector3Int alternativePos)
        {
            return _board[strategyPos.x, strategyPos.y, alternativePos.x, alternativePos.y];
        }

        public ETttType GetClassicCell(Vector3Int strategyPos, Vector3Int alternativePos, Vector3Int classicPos)
        {
            return _board[strategyPos.x, strategyPos.y, alternativePos.x, alternativePos.y, classicPos.x, classicPos.y];
        }

        protected void GameOverCheck()
        {
            StrategyGameOverChecker.GameOverCheck(_board, _COLUMNS, _ROWS, _LENGTH, GameOverBoard);
        }

        private void SetBlock(Vector3Int alternativePos, Vector3Int classicPos)
        {
            if (_gameOver)
            {
                return;
            }

            if (_board[alternativePos.x, alternativePos.y] == ETttType.None &&
                _board[alternativePos.x, alternativePos.y, classicPos.x, classicPos.y] == ETttType.None)
            {
                _board.SetAllBlock(true);
                _board.SetStrategyBlock(false, alternativePos.x, alternativePos.y);
                _board.SetAllAlternativeBlocksInStrategyCell(true, alternativePos.x, alternativePos.y);
                _board.SetAlternativeBlock(false, alternativePos.x, alternativePos.y, classicPos.x, classicPos.y);
            }
            else
            {
                _board.SetAllBlock(false);
            }

            BlockChanged?.Invoke();
        }

        private void GameOverBoard(ETttType type, Vector3Int[] indexes)
        {
            _gameOver = true;

            _board.SetAllBlock(false);
            BlockChanged?.Invoke();

            GameOverStrategyBoardWithIndexes?.Invoke(type, indexes);
            GameOver?.Invoke(type);

            Uninitialize();
        }
    }
}