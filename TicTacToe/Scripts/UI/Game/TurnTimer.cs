using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.Game
{
    public class TurnTimer : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private Image _circleImage;
        [SerializeField] private RectTransform _headTransform;
        [SerializeField] private Graphic[] _paintable;
        [SerializeField] private float _radius;
        [Range(0, 1)] [SerializeField] private float _minPos;
        [Range(0, 1)] [SerializeField] private float _maxPos;
        [Space] [Range(0, 1)] [SerializeField] private float _position;
        [SerializeField] private Gradient _color;
#pragma warning restore 649

        public Gradient Color
        {
            get => _color;
            set => SetColor(value);
        }

        public float Position
        {
            get => _position;
            set => SetPosition(value);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_circleImage != null && _headTransform != null)
            {
                SetPosition(_position);
            }

            if (_paintable != null)
            {
                SetColor(_color);
            }
        }
#endif

        public void SetColor(Gradient value)
        {
            foreach (var graphic in _paintable)
            {
                graphic.color = value.Evaluate(_position);
            }
        }

        public void SetPosition(float position)
        {
            position = _minPos + (_maxPos - _minPos) * position;

            _circleImage.fillAmount = position;
            _headTransform.anchoredPosition = _radius * new Vector2(
                Mathf.Sin((1 - position) * (360 * Mathf.Deg2Rad)),
                Mathf.Cos(position * (360 * Mathf.Deg2Rad))
            );

            _headTransform.rotation = Quaternion.Euler(0, 0, position * 360);
        }
    }
}