using System;
using Dainty.UI.WindowBase;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.Windows.FinishWindow
{
    public class FinishWindowAnimation : AWindowAnimation
    {
        private const float ANIM_DURATION = 1f;

#pragma warning disable 649
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _topPanel;
        [SerializeField] private Image _samurai;
        [SerializeField] private Image _backGround;
        [SerializeField] private RectTransform _buttonRestart;
        [SerializeField] private RectTransform _buttonMenu;
#pragma warning restore 649

        private Vector3 _contentRectHighPos;
        private Vector3 _contentRectLeftPos;
        private Vector3 _contentRectRightPos;
        private Tween _tween;

        protected override void OnInitialized()
        {
            InitAreaBorders();
        }

        private void OnDestroy()
        {
            _tween?.Kill();
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
                            .Join(_topPanel.DOAnchorPos(_contentRectHighPos, ANIM_DURATION))
                            .SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                _canvas.enabled = false;
                                animationFinished?.Invoke();
                            });
        }

        public override void PlayShowAnimation(bool push, Action animationFinished = null)
        {
            _tween?.Kill();

            _canvas.enabled = true;
            _topPanel.anchoredPosition = _contentRectHighPos;

            Color initColor = _samurai.color;
            initColor.a = 0;
            _samurai.color = initColor;
            initColor = _backGround.color;
            var backgroundFade = initColor.a;
            initColor.a = 0;
            _backGround.color = initColor;

            Vector3 buttonRestartAnchoredPos = _buttonRestart.anchoredPosition;
            Vector3 buttonMenuAnchoredPos = _buttonMenu.anchoredPosition;
            _buttonRestart.anchoredPosition = buttonRestartAnchoredPos + _contentRectLeftPos;
            _buttonMenu.anchoredPosition = buttonMenuAnchoredPos + _contentRectRightPos;

            _tween = DOTween.Sequence()
                            .Join(_topPanel.DOAnchorPos(Vector3.zero, ANIM_DURATION))
                            .Join(_backGround.DOFade(backgroundFade, ANIM_DURATION))
                            .Join(_samurai.DOFade(1, ANIM_DURATION))
                            .Join(_buttonRestart.DOAnchorPos(buttonRestartAnchoredPos, ANIM_DURATION))
                            .Join(_buttonMenu.DOAnchorPos(buttonMenuAnchoredPos, ANIM_DURATION))
                            .OnComplete(() => animationFinished?.Invoke());
        }

        public override void ShowImmediate()
        {
            _tween?.Kill();

            _canvas.enabled = true;

            _topPanel.anchoredPosition = Vector3.zero;
        }

        private void InitAreaBorders()
        {
            var safeArea = UIRoot.GetSafeArea();
            _contentRectHighPos = Vector3.up * (_topPanel.rect.height * 4 + safeArea.y);
            _contentRectLeftPos = Vector3.left * (_buttonRestart.rect.width + safeArea.x);
            _contentRectRightPos = Vector3.right * (_buttonRestart.rect.width + safeArea.x);
        }
    }
}