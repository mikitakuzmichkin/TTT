using DG.Tweening;
using TMPro;
using UnityEngine;

namespace TicTacToe.UI.Windows.OptionsWindow
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TitleSettingAnimation : MonoBehaviour
    {
        private const float ANIM_DURATION = 0.3f;
        private TextMeshProUGUI _text;
        private Tween _tween;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void SetText(string value, bool animate)
        {
            _tween?.Kill();

            if (animate)
            {
                _tween = DOTween.Sequence()
                                .Append(_text.DOFade(0, ANIM_DURATION / 2f))
                                .AppendCallback(() => _text.text = value)
                                .Append(_text.DOFade(1, ANIM_DURATION / 2f));
            }
            else
            {
                var color = _text.color;
                color.a = 1;
                _text.color = color;

                _text.text = value;
            }
        }
    }
}