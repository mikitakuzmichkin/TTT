using UnityEngine;

namespace TicTacToe.Extensions
{
    public static class Vector3IntExtensions
    {
        public static Vector3Int ToVector3Int(this Vector2Int vector)
        {
            return new Vector3Int(vector.x, vector.y, 0);
        }
    }
}