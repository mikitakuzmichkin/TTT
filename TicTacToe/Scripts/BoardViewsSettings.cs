using System;
using TicTacToe.Mechanics.Base;
using UnityEngine;

namespace TicTacToe
{
    [CreateAssetMenu(menuName = "TicTacToe/BoardViewsSettings")]
    public class BoardViewsSettings : ScriptableObject
    {
#pragma warning disable 649
        [SerializeField] private ABoardView[] _views;
#pragma warning restore 649

        public ABoardView[] Views => _views;

        public ABoardView GetByType(Type type)
        {
            foreach (var view in _views)
            {
                if (view.GetType() == type)
                {
                    return view;
                }
            }

            return null;
        }
    }
}