using System;
using Dainty.UI;
using Dainty.UI.WindowBase;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.Windows.MainMenu.GameWebTypeSelector
{
    public class GameTypeSelectorWindowAnimation : AWindowAnimation
    {
        private const float ANIM_DURATION = 0.3f;

#pragma warning disable 649
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Image _backingBackground;
        [SerializeField] private Color _backingBackgroundDestColor = Color.white;
        [SerializeField] private RectTransform _contentRect;
        [SerializeField] private SafeArea _safeArea;
        [SerializeField] private RectTransform _bottomEdgeRect;
#pragma warning restore 649

        private Vector3 _contentRectLowPos;
        private Tween _tween;

        protected override void OnInitialized()
        {
            _safeArea.Changed += SafeAreaOnChanged;
            SafeAreaOnChanged();
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }

        public override void ShowImmediate()
        {
            _tween?.Kill();

            _canvas.enabled = true;

            _backingBackground.color = _backingBackgroundDestColor;
            _contentRect.anchoredPosition = Vector3.zero;
        }

        public override void PlayShowAnimation(bool push, Action animationFinished = null)
        {
            _tween?.Kill();

            _canvas.enabled = true;
            var initColor = _backingBackgroundDestColor;
            initColor.a = 0;
            _backingBackground.color = initColor;
            _contentRect.anchoredPosition = _contentRectLowPos;

            _tween = DOTween.Sequence()
                            .Join(_backingBackground.DOColor(_backingBackgroundDestColor, ANIM_DURATION))
                            .Join(_contentRect.DOAnchorPos(Vector3.zero, ANIM_DURATION)
                                              .SetEase(Ease.OutCubic))
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

            _tween = DOTween.Sequence()
                            .Join(_backingBackground.DOFade(0, ANIM_DURATION))
                            .Join(_contentRect.DOAnchorPos(_contentRectLowPos, ANIM_DURATION)
                                              .SetEase(Ease.OutQuart))
                            .OnComplete(() =>
                            {
                                _canvas.enabled = false;
                                animationFinished?.Invoke();
                            });
        }

        private void SafeAreaOnChanged()
        {
            var safeArea = UIRoot.GetSafeArea();
            _contentRectLowPos = Vector3.down * (_contentRect.rect.height + _bottomEdgeRect.rect.height + safeArea.y);
        }
    }
}