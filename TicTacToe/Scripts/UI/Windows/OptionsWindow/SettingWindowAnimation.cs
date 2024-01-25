using System;
using Dainty.UI;
using Dainty.UI.WindowBase;
using DG.Tweening;
using UnityEngine;

namespace TicTacToe.UI.Windows.OptionsWindow
{
    public class SettingWindowAnimation : AWindowAnimation
    {
        private const float ANIM_DURATION = 0.4f;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private SafeArea _safeArea;

        private Vector3 _contentRectLowPos;
        private Vector3 _contentRectRightPos;
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
            _panel.anchoredPosition = Vector3.zero;
        }

        public override void PlayShowAnimation(bool push, Action animationFinished = null)
        {
            _tween?.Kill();

            _panel.anchoredPosition = _contentRectRightPos;
            _canvas.enabled = true;

            _tween = _panel.DOAnchorPos(Vector3.zero, ANIM_DURATION)
                           .SetEase(Ease.OutCubic)
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

            _tween = _panel.DOAnchorPos(_contentRectLowPos, ANIM_DURATION)
                           .SetEase(Ease.OutQuart)
                           .OnComplete(() => animationFinished?.Invoke());
        }

        private void SafeAreaOnChanged()
        {
            var safeArea = UIRoot.GetSafeArea();
            var panelSize = _panel.rect.size;

            _contentRectRightPos = Vector3.right * (panelSize.x + safeArea.x);
            _contentRectLowPos = Vector3.down * (panelSize.y + safeArea.y);
        }
    }
}