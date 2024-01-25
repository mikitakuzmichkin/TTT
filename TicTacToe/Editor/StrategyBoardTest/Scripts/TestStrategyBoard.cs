using System;
using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Base;
using TicTacToe.Mechanics.Strategy;
using TicTacToe.Players;
using UnityEngine;

[CreateAssetMenu(fileName = "StrategyTestBoard", menuName = "Test/create StrategyTestBoard")]
public class TestStrategyBoard : ScriptableObject
{
    [HideInInspector] public ETttType[] Board;
    [HideInInspector] public AlternativeBoard[] AlternativeBoards;
    [HideInInspector] public ETttType Turn;
    [HideInInspector] public ETttType YourSign;
    public EPersonType YourPersonType;
    [HideInInspector] public bool IsInitialized;

    public ETttType this[int strategyX, int strategyY]
    {
        set => Board[strategyX + strategyY * 3] = value;
        get => Board[strategyX + strategyY * 3];
    }

    public ETttType this[int strategyX, int strategyY, int alternativeX, int alternativeY]
    {
        set => AlternativeBoards[strategyX + strategyY * 3].Board[alternativeX + alternativeY * 3] = value;
        get => AlternativeBoards[strategyX + strategyY * 3].Board[alternativeX + alternativeY * 3];
    }

    public ETttType this[int strategyX, int strategyY, int alternativeX, int alternativeY, int classicX, int classicY]
    {
        set =>
            AlternativeBoards[strategyX + strategyY * 3].ClassicBoards[alternativeX + alternativeY * 3]
                                                        .Board[classicX + classicY * 3] = value;
        get =>
            AlternativeBoards[strategyX + strategyY * 3].ClassicBoards[alternativeX + alternativeY * 3]
                                                        .Board[classicX + classicY * 3];
    }

    public void Initialize()
    {
        AlternativeBoards = new AlternativeBoard[9];
        Board = new ETttType[9];
        for (var i = 0; i < 9; i++)
        {
            AlternativeBoards[i] = new AlternativeBoard();
            ;
            var alternative = AlternativeBoards[i];
            alternative.ClassicBoards = new ClassicBoard[9];
            alternative.Board = new ETttType[9];
            for (var j = 0; j < 9; j++)
            {
                alternative.ClassicBoards[j] = new ClassicBoard();
                var classic = alternative.ClassicBoards[j];
                classic.Board = new ETttType[9];
            }
        }

        IsInitialized = true;
    }

    public StrategyBoard<ETttType> GetStrategyBoard()
    {
        var strategyBoard = new StrategyBoard<ETttType>(3, 3, 3, 3, 3, 3);
        for (var xs = 0; xs < 3; xs++)
        {
            for (var ys = 0; ys < 3; ys++)
            {
                strategyBoard[xs, ys] = this[xs, ys];
                if (this[xs, ys] == ETttType.None)
                {
                    for (var xa = 0; xa < 3; xa++)
                    {
                        for (var ya = 0; ya < 3; ya++)
                        {
                            strategyBoard[xs, ys, xa, ya] = this[xs, ys, xa, ya];
                            if (this[xs, ys, xa, ya] == ETttType.None)
                            {
                                for (var xc = 0; xc < 3; xc++)
                                {
                                    for (var yc = 0; yc < 3; yc++)
                                    {
                                        strategyBoard[xs, ys, xa, ya, xc, yc] = this[xs, ys, xa, ya, xc, yc];
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return strategyBoard;
    }

    public ABoardModel GetStrategyModel(ETttType turn)
    {
        return new StrategyBoardModel(GetStrategyBoard(), turn);
    }

    [Serializable]
    public class ClassicBoard
    {
        public ETttType[] Board;
    }

    [Serializable]
    public class AlternativeBoard
    {
        public ClassicBoard[] ClassicBoards;
        public ETttType[] Board;
    }
}