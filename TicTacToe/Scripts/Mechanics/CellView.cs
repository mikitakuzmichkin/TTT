using System;
using TicTacToe.Mechanics.Base;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Mechanics
{
    public class CellView : ACellView
    {
        [SerializeField] protected Button _button;
        [SerializeField] private Graphic _buttonGraphic;

        public event Action<CellView> Click;

        public bool Interactable
        {
            set => _buttonGraphic.raycastTarget = value;
        }

        private void Awake()
        {
            SetCellClick();
        }

        protected void SetCellClick()
        {
            _button.onClick.AddListener(() => Click?.Invoke(this));
        }
    }
}