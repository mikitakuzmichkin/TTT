using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.SceneAnimation
{
    public class SceneAnimationSolidFade : AChangeSceneAnimation
    {
        private const float ANIM_DURATION = 0.4f;

        [SerializeField] private Color _fadeColor = Color.white;
        [SerializeField] private Image _front;

        public override void PlayAfterSceneChanged()
        {
            _front.color = _fadeColor;
            DOTween.Sequence()
                   .Append(_front.DOFade(0, ANIM_DURATION))
                   .OnComplete(() => _front.raycastTarget = false);
        }

        public override void PlayBeforeSceneChanged(Action callback)
        {
            var color = _fadeColor;
            color.a = 0;
            _front.color = color;

            DOTween.Sequence()
                   .AppendCallback(() => _front.raycastTarget = true)
                   .Append(_front.DOFade(1, ANIM_DURATION))
                   .OnComplete(() => callback?.Invoke());
        }
    }
}