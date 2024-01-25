using Newtonsoft.Json;

namespace TicTacToe.Mechanics.Alternative
{
    public class AlternativeBoard<T>
    {
        [JsonProperty] private T[,][,] _board;
        [JsonProperty] private T[,] _alternativeBoard;
        [JsonProperty] private bool[,] _alternativeBoardBlockedMap;
        [JsonProperty] private int _rows;
        [JsonProperty] private int _columns;

        public int AlternativeRows => _rows;
        public int AlternativeColumns => _columns;

        public AlternativeBoard(int alternativeRows, int alternativeColumn, int rows, int column)
        {
            _columns = alternativeColumn;
            _rows = alternativeRows;

            _board = new T[alternativeColumn, alternativeRows][,];
            _alternativeBoard = new T[alternativeColumn, alternativeRows];
            _alternativeBoardBlockedMap = new bool[alternativeColumn, alternativeRows];
            for (var x = 0; x < alternativeColumn; x++)
            {
                for (var y = 0; y < alternativeRows; y++)
                {
                    _board[x, y] = new T[column, rows];
                    _alternativeBoardBlockedMap[x, y] = false;
                }
            }
        }

        public T this[int alternativeX, int alternativeY, int x, int y]
        {
            get => _board[alternativeX, alternativeY][x, y];
            set => _board[alternativeX, alternativeY][x, y] = value;
        }

        public T this[int alternativeX, int alternativeY]
        {
            get => _alternativeBoard[alternativeX, alternativeY];
            set => _alternativeBoard[alternativeX, alternativeY] = value;
        }

        public void SetCell(T value, int alternativeX, int alternativeY, int x, int y)
        {
            _board[alternativeX, alternativeY][x, y] = value;
        }

        public T GetCell(int alternativeX, int alternativeY, int x, int y)
        {
            return _board[alternativeX, alternativeY][x, y];
        }

        public void SetBigCell(T value, int alternativeX, int alternativeY)
        {
            _alternativeBoard[alternativeX, alternativeY] = value;
        }

        public T GetBigCell(int alternativeX, int alternativeY)
        {
            return _alternativeBoard[alternativeX, alternativeY];
        }

        public T[,] GetBoard(int alternativeX, int alternativeY)
        {
            return _board[alternativeX, alternativeY];
        }

        public void SetBlock(bool value, int alternativeX, int alternativeY)
        {
            _alternativeBoardBlockedMap[alternativeX, alternativeY] = value;
        }

        public void SetAllBlock(bool value)
        {
            for (var x = 0; x < AlternativeColumns; x++)
            {
                for (var y = 0; y < AlternativeRows; y++)
                {
                    _alternativeBoardBlockedMap[x, y] = value;
                }
            }
        }

        public bool IsBlocked(int alternativeX, int alternativeY)
        {
            return _alternativeBoardBlockedMap[alternativeX, alternativeY];
        }
    }
}