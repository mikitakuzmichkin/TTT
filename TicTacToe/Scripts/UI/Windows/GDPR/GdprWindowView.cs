using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.Windows.GDPR
{
    public class GdprWindowView : MonoBehaviour
    {
        [SerializeField] private GameObject[] _popupObjects;

        [Header("Slide 1")] [SerializeField] private GameObject _slide1;
        [SerializeField] private TMP_Text _slide1Text;
        [SerializeField] private TMPLinkHandler _slide1TextLinkHandler;
        [SerializeField] private Button _acceptButton;

        [Header("Slide 2")] [SerializeField] private GameObject _slide2;
        [SerializeField] private TMP_Text _slide2Text;
        [SerializeField] private TMPLinkHandler _slide2TextLinkHandler;
        [SerializeField] private Button _closeButton;

        public event Action AcceptButtonClick;
        public event Action CloseButtonClick;
        public event Func<string, bool> LinkClick;

        private void Awake()
        {
            _acceptButton.onClick.AddListener(() => AcceptButtonClick?.Invoke());
            _closeButton.onClick.AddListener(() => CloseButtonClick?.Invoke());

            Func<string, bool> onLinkClick = OnLinkClick;
            _slide1TextLinkHandler.LinkClick += onLinkClick;
            _slide2TextLinkHandler.LinkClick += onLinkClick;

            SetSecondSlideActive(false);

            _slide1Text.text = string.Format(_slide1Text.text, Constants.URL_EULA, Constants.URL_PRIVACY_POLICY);
            _slide2Text.text = string.Format(_slide2Text.text, Constants.URL_EULA, Constants.URL_PRIVACY_POLICY);
        }

        public void SetSecondSlideActive(bool active)
        {
            _slide1.SetActive(!active);
            _slide2.SetActive(active);
        }

        public void HidePopup()
        {
            foreach (var popupObject in _popupObjects)
            {
                popupObject.SetActive(false);
            }
        }

        private bool OnLinkClick(string url)
        {
            return LinkClick == null || LinkClick.Invoke(url);
        }
    }
}