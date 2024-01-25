using System;
using UnityEngine;

namespace TicTacToe.Mechanics.Classic
{
    public static class ClassicGameOverChecker
    {
        //to do unit tests
        public static void GameOverCheck(ETttType[,] board, int columns, int rows, int length,
            Action<ETttType, Vector3Int[]> gameOver)
        {
            //Check vertical lines
            for (var x = 0; x < columns; x++)
            {
                var checker = true;
                for (var y = 0; y < length - 1; y++)
                {
                    if (board[x, y] != ETttType.Cross && board[x, y] != ETttType.Noughts)
                    {
                        checker = false;
                        break;
                    }

                    checker &= board[x, y] == board[x, y + 1];
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

                gameOver?.Invoke(board[x, 0], positions);
                return;
            }

            //check horizontal lines
            for (var y = 0; y < rows; y++)
            {
                var checker = true;
                for (var x = 0; x < length - 1; x++)
                {
                    if (board[x, y] != ETttType.Cross && board[x, y] != ETttType.Noughts)
                    {
                        checker = false;
                        break;
                    }

                    checker &= board[x, y] == board[x + 1, y];
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

                gameOver?.Invoke(board[0, y], positions);
                return;
            }

            if (CheckDio(board, false, length, gameOver))
            {
                return;
            }

            if (CheckDio(board, true, length, gameOver))
            {
                return;
            }

            if (CheckDraw(board))
            {
                gameOver?.Invoke(ETttType.Draw, null);
            }
        }

        //todo unit tests
        //Check diagonal winline 
        private static bool CheckDio(ETttType[,] board, bool secondary, int length,
            Action<ETttType, Vector3Int[]> gameOver)
        {
            var checkerDio = true;

            var positions = new Vector3Int[Mathf.Abs(length)];
            var yStart = secondary ? length - 1 : 0;
            var yIncrement = secondary ? -1 : 1;
            for (int y = yStart, x = 0; x < length - 1; y += yIncrement, x++)
            {
                if (board[x, y] != ETttType.Cross && board[x, y] != ETttType.Noughts)
                {
                    return false;
                }

                checkerDio &= board[x, y] == board[x + 1, y + yIncrement];
                positions[x] = new Vector3Int(x, y, 0);
            }

            positions[length - 1] = new Vector3Int(length - 1, yStart + yIncrement * (length - 1), 0);

            if (checkerDio == false)
            {
                return false;
            }

            gameOver?.Invoke(board[0, yStart], positions);
            return true;
        }

        private static bool CheckDraw(ETttType[,] board)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var cell = board[x, y];
                    if (cell != ETttType.Cross && cell != ETttType.Noughts)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}