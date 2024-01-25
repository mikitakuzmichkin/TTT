using System;
using Dainty.UI.WindowBase;
using DG.Tweening;
using UnityEngine;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows
{
    public class SubWindowAnimation : AWindowAnimation
    {
        private const float ANIM_DURATION = 0.3f;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _panel;

        private Tween _tween;

        private void OnDestroy()
        {
            _tween?.Kill();
        }

        public override void ShowImmediate()
        {
            _tween?.Kill();

            _canvas.enabled = true;

            var anchoredPosition = _panel.anchoredPosition;
            anchoredPosition.x = 0;
            _panel.anchoredPosition = anchoredPosition;
        }

        public override void PlayShowAnimation(bool push, Action animationFinished = null)
        {
            _tween?.Kill();

            var border = GetHorizontalAreaBorder(!push);
            var anchoredPosition = _panel.anchoredPosition;
            anchoredPosition.x = border;
            _panel.anchoredPosition = anchoredPosition;

            _canvas.enabled = true;

            _tween = DOTween.Sequence()
                            .Join(_panel.DOAnchorPosX(0, ANIM_DURATION))
                            .OnComplete(() => animationFinished?.Invoke());
        }

        public override void CloseImmediate()
        {
            _tween?.Kill();
            _canvas.enabled = false;
        }

        public override void PlayCloseAnimation(bool pop, Action animationFinished = null)
        {
            _tween?.Kill();

            var border = GetHorizontalAreaBorder(!pop);
            _tween = DOTween.Sequence()
                            .Join(_panel.DOAnchorPosX(border, ANIM_DURATION))
                            .OnComplete(() =>
                            {
                                animationFinished?.Invoke();
                                _canvas.enabled = false;
                            });
        }

        private float GetHorizontalAreaBorder(bool toRight)
        {
            var safeArea = UIRoot.GetSafeArea();
            var result = _panel.rect.width + safeArea.x;
            return toRight ? -result : result;
        }
    }
}