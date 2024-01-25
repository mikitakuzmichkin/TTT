using System;
using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Alternative;
using TicTacToe.Mechanics.Base;
using TicTacToe.Mechanics.Strategy;
using TicTacToe.Players;
using UnityEngine;

[CreateAssetMenu(fileName = "AlternativeTestBoard", menuName = "Test/create AlternativeTestBoard")]
public class TestAlternativeBoard : ScriptableObject
{
    [HideInInspector] public AlternativeBoardTest AlternativeBoards;
    [HideInInspector] public ETttType Turn;
    [HideInInspector] public ETttType YourSign;
    public EPersonType YourPersonType;
    [HideInInspector] public bool IsInitialized;

    public ETttType this[ int alternativeX, int alternativeY]
    {
        set => AlternativeBoards.Board[alternativeX + alternativeY * 3] = value;
        get => AlternativeBoards.Board[alternativeX + alternativeY * 3];
    }

    public ETttType this[int alternativeX, int alternativeY, int classicX, int classicY]
    {
        set =>
            AlternativeBoards.ClassicBoards[alternativeX + alternativeY * 3]
                                                        .Board[classicX + classicY * 3] = value;
        get =>
            AlternativeBoards.ClassicBoards[alternativeX + alternativeY * 3]
                                                        .Board[classicX + classicY * 3];
    }

    public void Initialize()
    {
        AlternativeBoards = new AlternativeBoardTest(); ;
        var alternative = AlternativeBoards;
        alternative.ClassicBoards = new ClassicBoard[9];
        alternative.Board = new ETttType[9];
        for (var j = 0; j < 9; j++)
        {
            alternative.ClassicBoards[j] = new ClassicBoard();
            var classic = alternative.ClassicBoards[j];
            classic.Board = new ETttType[9];
        }
        IsInitialized = true;
    }

    public AlternativeBoard<ETttType> GetAlternativeBoard()
    {
        var alternativeBoard = new AlternativeBoard<ETttType>(3,3,3,3);
        for (var xa = 0; xa < 3; xa++)
        {
            for (var ya = 0; ya < 3; ya++)
            {
                alternativeBoard[xa, ya] = this[xa, ya];
                if (this[xa, ya] == ETttType.None)
                {
                    for (var xc = 0; xc < 3; xc++)
                    {
                        for (var yc = 0; yc < 3; yc++)
                        {
                            alternativeBoard[xa, ya, xc, yc] = this[xa, ya, xc, yc];
                        }
                    }
                }
            }
        }
        return alternativeBoard;
    }

    public ABoardModel GetAlternativeModel(ETttType turn)
    {
        return new AlternativeBoardModel(GetAlternativeBoard(), turn);
    }

    [Serializable]
    public class ClassicBoard
    {
        public ETttType[] Board;
    }

    [Serializable]
    public class AlternativeBoardTest
    {
        public ClassicBoard[] ClassicBoards;
        public ETttType[] Board;
    }
}