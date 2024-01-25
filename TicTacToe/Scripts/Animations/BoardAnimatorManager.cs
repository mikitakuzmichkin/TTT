using System;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using TicTacToe.DI;
using TicTacToe.Mechanics.Base;
using TicTacToe;

namespace TicTacToe
{
    public class BoardAnimatorManager : MonoBehaviour
    {
        private ListQueue<AnimationsSequence> _animations;
        private ABoardView _board;
        private bool _isAnimated;

        public void Initialize(ABoardView board)
        {
            _board = board;
            _isAnimated = false;
            _animations = new ListQueue<AnimationsSequence>();
        }

        public void AddAnimationCallback(Action action, float delay = 0)
        {
            var seq = DOTween.Sequence();
            seq.AppendInterval(delay);
            seq.AppendCallback(() => action?.Invoke());
            AddAnimation(seq);
        }
        
        public void AddAnimation(Sequence seq, bool autoBoardControl = true)
        {
            seq.Pause();
            var anim = new AnimationsSequence() {Sequence = seq, AutoBoardControl = autoBoardControl};
            seq.onKill += () => KillAnimation(anim);
            if (_animations.Count < 1 && _isAnimated == false)
            {
                if (autoBoardControl)
                {
                    _board.Interactable = false;
                }

                _isAnimated = true;
                seq.Play();
                return;
            }
            _animations.Enqueue(anim);
        }

        private void NextAnimation(AnimationsSequence lastAnimation)
        {
            if (_animations.Count < 1)
            {
                if (lastAnimation.AutoBoardControl)
                {
                    _board.Interactable = true;
                }

                _isAnimated = false;
                return;
            }

            var anim = _animations.Dequeue();
            var seq = anim.Sequence;
            _isAnimated = true;
            seq.Play();
        }

        private void KillAnimation(AnimationsSequence anim)
        {
            if (_animations.Contains(anim))
            {
                _animations.Remove(anim);
            }
            else
            {
                NextAnimation(anim);
            }
        }

        private class AnimationsSequence
        {
            public Sequence Sequence;
            public bool AutoBoardControl;
        }
    }
}