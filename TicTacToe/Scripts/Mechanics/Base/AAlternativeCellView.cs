using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Mechanics.Base
{
    public abstract class AAlternativeCellView : ACellView
    {
        [SerializeField] private SymbolSprite _draw;
        [SerializeField] private GameObject _selectBorder;
        [SerializeField] private FrameAnimationComponent _selectAnimation;

        protected bool _block;

        private ImageDisable[] _images;

        public void SetBlock(bool block, bool select)
        {
            foreach (var image in Images)
            {
                image.isEnable = !block;
            }

            Select = select;
            _block = block;
        }

        public void SetBlock(bool block)
        {
            SetBlock(block, !block);
        }

        public bool GetBlock() => _block;

        public bool Select
        {
            set
            {
                if (value)
                {
                    _selectAnimation.StartAnimation();
                    return;
                }
                _selectBorder.SetActive(value);
            }
        }

        private ImageDisable[] Images
        {
            get
            {
                if (_images == null)
                {
                    _images = gameObject.GetComponentsInChildren<ImageDisable>(true);
                }

                return _images;
            }
        }

        private void Awake()
        {
            OnInitialize();
        }

        private void OnDestroy()
        {
            UnInitialize();
        }

        public abstract event Action<AAlternativeCellView, Vector3Int> Click;

        protected virtual void OnInitialize()
        {
            SetCellClick();
        }

        protected override void ChangeState(ETttType state, bool forceState = false)
        {
            base.ChangeState(state, forceState);
            if (state == ETttType.Draw)
            {
                if (forceState)
                {
                    _draw.FrameAnimationComponent.ForceAnimation();
                }
                else
                {
                    BoardAnimator.AddAnimation(_draw.FrameAnimationComponent.ReturnAnimation());
                }
                _symbolImage.color = _draw.Color;
            }
        }

        protected abstract void SetCellClick();

        protected virtual void UnInitialize()
        {
        }
    }
}