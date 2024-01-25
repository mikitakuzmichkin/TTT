using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TicTacToe.Extensions
{
    public static class RandomExtension
    {
        public static Vector2Int GetRandomSameElementsIndex<T>(this T[,] array, T element)
        {
            var rows = array.GetUpperBound(0) + 1;
            var columns = array.Length / rows;
            Vector2Int? index = null;
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    if (array[i, j].Equals(element))
                    {
                        if (index == null)
                        {
                            index = new Vector2Int(i, j);
                        }
                        else
                        {
                            if (Random.value > 0.5f)
                            {
                                index = new Vector2Int(i, j);
                            }
                        }
                    }
                }
            }

            if (index == null)
            {
                throw new RandomExtensionsException("element not found");
            }

            return (Vector2Int) index;
        }

        public class RandomExtensionsException : Exception
        {
            public RandomExtensionsException(string message) : base(message)
            {
            }
        }
    }
}