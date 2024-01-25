using System;
using TicTacToe.DI;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Mechanics.Base
{
    public abstract class ACellView : MonoBehaviour
    {
        [SerializeField] protected Image _symbolImage;
        [SerializeField] private SymbolSprite _cross;
        [SerializeField] private SymbolSprite _nought;
        private ETttType _state;
        private BoardAnimatorManager _boardAnimatorManager;

        public ETttType State
        {
            get => _state;
            set
            {
                _state = value;

                ChangeState(value);
            }
        }

        public virtual void ForceSetState(ETttType state)
        {
            ChangeState(state, true);
        }

        public void SetSprite(Sprite sprite, Color color)
        {
            _symbolImage.gameObject.SetActive(true);
            _symbolImage.color = color;
            _symbolImage.sprite = sprite;
        }

        protected BoardAnimatorManager BoardAnimator
        {
            get
            {
                if (_boardAnimatorManager == null)
                {
                    _boardAnimatorManager = ProjectContext.GetInstance<BoardAnimatorManager>();
                }
                return _boardAnimatorManager;
            }
        }

        protected virtual void ChangeState(ETttType state, bool forceState = false)
        {
            if (state == ETttType.None)
            {
                _symbolImage.gameObject.SetActive(false);
            }

            switch (state)
            {
                case ETttType.Cross:
                    if (forceState)
                    {
                        _cross.FrameAnimationComponent.ForceAnimation();
                    }
                    else
                    {
                        BoardAnimator.AddAnimation(_cross.FrameAnimationComponent.ReturnAnimation());
                    }
                    
                    _symbolImage.color = _cross.Color;
                    break;
                case ETttType.Noughts:
                    if (forceState)
                    {
                        _nought.FrameAnimationComponent.ForceAnimation();
                    }
                    else
                    {
                        BoardAnimator.AddAnimation(_nought.FrameAnimationComponent.ReturnAnimation());
                    }

                    _symbolImage.color = _nought.Color;
                    break;
            }
        }

        [Serializable]
        protected class SymbolSprite
        {
            public FrameAnimationComponent FrameAnimationComponent;
            public Color Color;
        }
    }
}