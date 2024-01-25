using System;
using Newtonsoft.Json;
using TicTacToe.Mechanics.Base;
using UnityEngine;

namespace TicTacToe.Mechanics.Classic
{
    public class ClassicBoardModel : ABoardModel
    {
        private const int _COLUMNS = 3;
        private const int _ROWS = 3;
        private const int _LENGTH = 3;

        [JsonProperty] private readonly ETttType[,] _board;

        public ClassicBoardModel()
        {
            _board = new ETttType[_COLUMNS, _ROWS];
            for (var x = 0; x < _COLUMNS; x++)
            {
                for (var y = 0; y < _ROWS; y++)
                {
                    _board[x, y] = ETttType.None;
                }
            }

            Turn = ETttType.Cross;
        }

        public ClassicBoardModel(ETttType[,] board)
        {
            _board = board;
        }

        //todo unit test
        public event Action<ETttType, Vector3Int[]> GameOverWithIndexes;
        public event Action ModelUpdate;
        public event Action<Vector3Int> CellChanged;
        public override event Action<ETttType> GameOver;

        public bool CheckCellEmpty(Vector3Int position)
        {
            return _board[position.x, position.y] == ETttType.None;
        }

        public void SetCell(ETttType type, Vector3Int position)
        {
            if (CheckCellEmpty(position) == false)
            {
                throw new BoardModelException("this cell allready full");
            }

            _board[position.x, position.y] = type;

            CellChanged?.Invoke(position);
            ModelUpdate?.Invoke();

            GameOverCheck();

            if (Turn == ETttType.Cross)
            {
                Turn = ETttType.Noughts;
            }
            else
            {
                Turn = ETttType.Cross;
            }
        }

        public ETttType GetCell(Vector3Int position)
        {
            return _board[position.x, position.y];
        }

        protected void GameOverCheck()
        {
            ClassicGameOverChecker.GameOverCheck(_board, _COLUMNS, _ROWS, _LENGTH, GameOverBoard);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
            GameOverWithIndexes = null;
            ModelUpdate = null;
            CellChanged = null;
            GameOver = null;
        }

        private void GameOverBoard(ETttType type, Vector3Int[] indexes)
        {
            GameOverWithIndexes?.Invoke(type, indexes);
            GameOver?.Invoke(type);
            Uninitialize();
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
    }
}