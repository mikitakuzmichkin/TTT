using System;

namespace TicTacToe.Mechanics
{
    public class BoardModelException : Exception
    {
        public BoardModelException(string message) : base(message)
        {
        }
    }
}