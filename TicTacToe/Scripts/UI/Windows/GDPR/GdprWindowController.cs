using UnityEngine;
using UnityEngine.SceneManagement;

namespace TicTacToe.UI.Windows.GDPR
{
    public class GdprWindowController : MonoBehaviour
    {
        [SerializeField] private GdprWindowView _view;

        private void Awake()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsConsts.GDPR_ACCEPTED, 0) != 0)
            {
                OnAccepted();
                return;
            }

            _view.AcceptButtonClick += OnAcceptButtonClick;
            _view.CloseButtonClick += OnCloseButtonClick;
            _view.LinkClick += OnLinkClick;
        }

        private bool OnLinkClick(string url)
        {
            if (url != "slide_2")
            {
                return true;
            }

            _view.SetSecondSlideActive(true);
            return false;
        }

        private void OnCloseButtonClick()
        {
            _view.SetSecondSlideActive(false);
        }

        private void OnAcceptButtonClick()
        {
            PlayerPrefs.SetInt(PlayerPrefsConsts.GDPR_ACCEPTED, 1);
            OnAccepted();
        }

        private void OnAccepted()
        {
            _view.HidePopup();
            SceneManager.LoadScene(1);
        }
    }
}