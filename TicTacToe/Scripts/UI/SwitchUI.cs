using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI
{
    public class SwitchUI : MonoBehaviour
    {
        [SerializeField] private Image _onImage;
        [SerializeField] private Image _offImage;

        [SerializeField] private bool _isInteractable;
        [SerializeField] private bool _isOn;

        public bool IsOn
        {
            get => _isOn;
            set
            {
                _isOn = value;
                _onImage.gameObject.SetActive(value);
                _offImage.gameObject.SetActive(!value);
            }
        }

        public bool IsInteractable
        {
            get => _isInteractable;
            set
            {
                _isInteractable = value;
                _onImage.raycastTarget = value;
                _offImage.raycastTarget = value;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            IsOn = _isOn;
            IsInteractable = _isInteractable;
        }
#endif
    }
}