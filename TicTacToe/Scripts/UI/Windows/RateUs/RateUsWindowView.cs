using System;
using Dainty.UI.WindowBase;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.Windows.RateUs
{
    public class RateUsWindowView : AWindowView
    {
        [SerializeField] private Button _rateNowButton;
        [SerializeField] private Button _remindLaterButton;

        public event Action RateNowButtonClick;
        public event Action RemindLaterButtonClick;

        protected override void OnSubscribe()
        {
            _rateNowButton.onClick.AddListener(() => RateNowButtonClick?.Invoke());
            _remindLaterButton.onClick.AddListener(() => RemindLaterButtonClick?.Invoke());
        }
    }
}