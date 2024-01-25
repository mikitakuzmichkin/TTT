using System;
using Newtonsoft.Json;
using TicTacToe.Mechanics.Base;
using TicTacToe.Mechanics.Classic;
using UnityEngine;

namespace TicTacToe.Mechanics.Alternative
{
    public class AlternativeBoardModel : ABoardModel
    {
        private const int _ALTERNATIVECOLUMNS = 3;
        private const int _ALTERNATIVEROWS = 3;
        private const int _COLUMNS = 3;
        private const int _ROWS = 3;
        private const int _LENGTH = 3;

        [JsonProperty] private readonly AlternativeBoard<ETttType> _board;
        [JsonProperty] private bool _gameOver;

        public AlternativeBoardModel()
        {
            _board = new AlternativeBoard<ETttType>(_ALTERNATIVEROWS, _ALTERNATIVECOLUMNS, _ROWS, _COLUMNS);
            _gameOver = false;
            Turn = ETttType.Cross;
        }

        public AlternativeBoardModel(AlternativeBoard<ETttType> board)
        {
            _board = board;
            _gameOver = false;
        }
        
        public AlternativeBoardModel(AlternativeBoard<ETttType> board, ETttType turn)
        {
            _board = board;
            Turn = turn;
            _gameOver = false;
        }

        //todo unit test
        public event Action<ETttType, Vector3Int[]> GameOverAlternativeBoardWithIndexes;
        public event Action<Vector3Int, ETttType, Vector3Int[]> GameOverClassicBoardWithIndexes;
        public event Action ModelUpdate;
        public event Action<Vector3Int> AlternativeCellChanged;
        public event Action<Vector3Int, Vector3Int> ClassicCellChanged;
        public event Action BlockChanged;
        public override event Action<ETttType> GameOver;

        public override void Uninitialize()
        {
            GameOverAlternativeBoardWithIndexes = null;
            GameOverClassicBoardWithIndexes = null;
            ModelUpdate = null;
            AlternativeCellChanged = null;
            ClassicCellChanged = null;
            BlockChanged = null;
            GameOver = null;
            base.Uninitialize();
        }

        public bool CheckCellEmpty(Vector3Int alternativePos, Vector3Int classicPos)
        {
            return _board[alternativePos.x, alternativePos.y, classicPos.x, classicPos.y] == ETttType.None;
        }

        public void SetClassicCell(ETttType type, Vector3Int alternativePos, Vector3Int classicPos)
        {
            if (CheckCellEmpty(alternativePos, classicPos) == false)
            {
                throw new BoardModelException("this cell is already full");
            }

            _board[alternativePos.x, alternativePos.y, classicPos.x, classicPos.y] = type;

            ClassicCellChanged?.Invoke(alternativePos, classicPos);

            var alternativeCell = ETttType.None;
            Action<ETttType, Vector3Int[]> func = (locType, array) =>
            {
                alternativeCell = locType;
                GameOverClassicBoardWithIndexes?.Invoke(alternativePos, locType, array);
            };

            ClassicGameOverChecker.GameOverCheck(_board.GetBoard(alternativePos.x, alternativePos.y), _COLUMNS, _ROWS,
                                                 _LENGTH, func);

            _board[alternativePos.x, alternativePos.y] = alternativeCell;
            AlternativeCellChanged?.Invoke(alternativePos);

            if (alternativeCell != ETttType.None)
            {
                GameOverCheck();
            }

            SetBlock(classicPos);

            ModelUpdate?.Invoke();

            Turn = Turn == ETttType.Cross ? ETttType.Noughts : ETttType.Cross;
        }

        public bool GetBlock(Vector3Int alternativePos)
        {
            return _board.IsBlocked(alternativePos.x, alternativePos.y);
        }

        public ETttType GetAlternativeCell(Vector3Int position)
        {
            return _board[position.x, position.y];
        }

        public ETttType GetClassicCell(Vector3Int alternativePos, Vector3Int classicPos)
        {
            return _board[alternativePos.x, alternativePos.y, classicPos.x, classicPos.y];
        }

        protected void GameOverCheck()
        {
            AlternativeGameOverChecker.GameOverCheck(_board, _COLUMNS, _ROWS, _LENGTH, GameOverBoard);
        }

        private void SetBlock(Vector3Int classicPos)
        {
            if (_gameOver)
            {
                return;
            }

            if (_board[classicPos.x, classicPos.y] == ETttType.None)
            {
                _board.SetAllBlock(true);
                _board.SetBlock(false, classicPos.x, classicPos.y);
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

            GameOverAlternativeBoardWithIndexes?.Invoke(type, indexes);
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