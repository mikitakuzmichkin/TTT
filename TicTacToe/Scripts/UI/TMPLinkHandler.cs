using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TicTacToe.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMPLinkHandler : MonoBehaviour, IPointerClickHandler
    {
        private TMP_Text _text;

        public event Func<string, bool> LinkClick;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var index = TMP_TextUtilities.FindIntersectingLink(_text, eventData.pressPosition, Camera.main);
            if (index == -1)
            {
                return;
            }

            var linkInfo = _text.textInfo.linkInfo[index];
            var url = linkInfo.GetLinkID();

            if (LinkClick == null || LinkClick.Invoke(url))
            {
                Application.OpenURL(url);
            }
        }
    }
}