using Newtonsoft.Json;
using TicTacToe.Mechanics.Alternative;

namespace TicTacToe.Mechanics.Strategy
{
    public class StrategyBoard<T>
    {
        [JsonProperty] private AlternativeBoard<T>[,] _board;
        [JsonProperty] private T[,] _strategyBoard;
        [JsonProperty] private bool[,] _strategyBoardBlockedMap;
        [JsonProperty] private int _rows;
        [JsonProperty] private int _columns;

        public int StrategyRows => _rows;
        public int StrategyColumns => _columns;

        public StrategyBoard(int strategyRows, int strategyColumns, int alternativeRows, int alternativeColumn,
            int rows, int column)
        {
            _columns = strategyRows;
            _rows = strategyColumns;

            _board = new AlternativeBoard<T>[strategyColumns, strategyRows];
            _strategyBoard = new T[strategyColumns, strategyRows];
            _strategyBoardBlockedMap = new bool[strategyColumns, strategyRows];
            for (var x = 0; x < strategyColumns; x++)
            {
                for (var y = 0; y < strategyRows; y++)
                {
                    _board[x, y] = new AlternativeBoard<T>(alternativeRows, alternativeColumn, rows, column);
                    _strategyBoardBlockedMap[x, y] = false;
                }
            }
        }

        public T this[int strategyX, int strategyY, int alternativeX, int alternativeY, int x, int y]
        {
            get => _board[strategyX, strategyY][alternativeX, alternativeY, x, y];
            set => _board[strategyX, strategyY][alternativeX, alternativeY, x, y] = value;
        }

        public T this[int strategyX, int strategyY, int alternativeX, int alternativeY]
        {
            get => _board[strategyX, strategyY][alternativeX, alternativeY];
            set => _board[strategyX, strategyY][alternativeX, alternativeY] = value;
        }

        public T this[int strategyX, int strategyY]
        {
            get => _strategyBoard[strategyX, strategyY];
            set => _strategyBoard[strategyX, strategyY] = value;
        }

        public void SetStrategyBlock(bool value, int strategyX, int strategyY)
        {
            _strategyBoardBlockedMap[strategyX, strategyY] = value;
        }

        public void SetAlternativeBlock(bool value, int strategyX, int strategyY, int alternativeX, int alternativeY)
        {
            _board[strategyX, strategyY].SetBlock(value, alternativeX, alternativeY);
        }

        public void SetAllBlock(bool value)
        {
            for (var x = 0; x < StrategyColumns; x++)
            {
                for (var y = 0; y < StrategyRows; y++)
                {
                    _strategyBoardBlockedMap[x, y] = value;
                    if (value == false)
                    {
                        _board[x, y].SetAllBlock(value);
                    }
                }
            }
        }

        public void SetAllAlternativeBlocksInStrategyCell(bool value, int strategyX, int strategyY)
        {
            _board[strategyX, strategyY].SetAllBlock(value);
        }

        public bool IsStrategyCellBlocked(int strategyX, int strategyY)
        {
            return _strategyBoardBlockedMap[strategyX, strategyY];
        }

        public bool IsAlternativeCellBlocked(int strategyX, int strategyY, int alternativeX, int alternativeY)
        {
            return _board[strategyX, strategyY].IsBlocked(alternativeX, alternativeY);
        }

        public T[,] GetClassicBoard(int strategyX, int strategyY, int alternativeX, int alternativeY)
        {
            return _board[strategyX, strategyY].GetBoard(alternativeX, alternativeY);
        }

        public AlternativeBoard<T> GetAlternativeBoard(int strategyX, int strategyY)
        {
            return _board[strategyX, strategyY];
        }
    }
}