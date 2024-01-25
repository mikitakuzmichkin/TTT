using System;
using UnityEngine;

namespace TicTacToe.Mechanics.Alternative
{
    public static class AlternativeGameOverChecker
    {
        //todo unit test
        public static void GameOverCheck(AlternativeBoard<ETttType> board, int columns, int rows, int length,
            Action<ETttType, Vector3Int[]> gameOver)
        {
            var xResult = GameOverCheckType(ETttType.Cross, board, columns, rows, length);
            var oResult = GameOverCheckType(ETttType.Noughts, board, columns, rows, length);

            if (xResult != null && oResult != null)
            {
                gameOver?.Invoke(ETttType.Draw, null);
                return;
            }

            if (xResult != null)
            {
                gameOver?.Invoke(ETttType.Cross, xResult);
                return;
            }

            if (oResult != null)
            {
                gameOver?.Invoke(ETttType.Noughts, oResult);
                return;
            }
            
            if (CheckDraw(board))
            {
                gameOver?.Invoke(ETttType.Draw, null);
                return;
            }
        }

        public static Vector3Int[] GameOverCheckType(ETttType type, AlternativeBoard<ETttType> board, int columns,
            int rows, int length)
        {
            //Check vertical lines
            for (var x = 0; x < columns; x++)
            {
                var checker = true;
                for (var y = 0; y < length; y++)
                {
                    if (board[x, y] != type && board[x, y] != ETttType.Draw)
                    {
                        checker = false;
                        break;
                    }
                }

                if (checker == false)
                {
                    continue;
                }

                var positions = new Vector3Int[length];
                for (var y = 0; y < length; y++)
                {
                    positions[y] = new Vector3Int(x, y, 0);
                }

                return positions;
            }

            //check horizontal lines
            for (var y = 0; y < rows; y++)
            {
                var checker = true;
                for (var x = 0; x < length; x++)
                {
                    if (board[x, y] != type && board[x, y] != ETttType.Draw)
                    {
                        checker = false;
                        break;
                    }
                }

                if (checker == false)
                {
                    continue;
                }

                var positions = new Vector3Int[length];
                for (var x = 0; x < length; x++)
                {
                    positions[x] = new Vector3Int(x, y, 0);
                }

                return positions;
            }

            Vector3Int[] indexes;

            if (CheckDio(board, false, length, out indexes, type))
            {
                return indexes;
            }

            if (CheckDio(board, true, length, out indexes, type))
            {
                return indexes;
            }

            return null;
        }

        //todo unit tests
        //Check diagonal winline 
        private static bool CheckDio(AlternativeBoard<ETttType> board, bool secondary, int length,
            out Vector3Int[] indexes, ETttType type)
        {
            indexes = null;
            var checkerDio = true;

            var positions = new Vector3Int[Mathf.Abs(length)];
            var yStart = secondary ? length - 1 : 0;
            var yIncrement = secondary ? -1 : 1;
            for (int y = yStart, x = 0; x < length; y += yIncrement, x++)
            {
                if (board[x, y] != type && board[x, y] != ETttType.Draw)
                {
                    return false;
                }

                positions[x] = new Vector3Int(x, y, 0);
            }

            positions[length - 1] = new Vector3Int(length - 1, yStart + yIncrement * (length - 1), 0);

            if (checkerDio == false)
            {
                return false;
            }

            indexes = positions;
            return true;
        }

        private static bool CheckDraw(AlternativeBoard<ETttType> board)
        {
            var width = board.AlternativeColumns;
            var height = board.AlternativeRows;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var cell = board[x, y];
                    if (cell != ETttType.Cross && cell != ETttType.Noughts && cell != ETttType.Draw)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}