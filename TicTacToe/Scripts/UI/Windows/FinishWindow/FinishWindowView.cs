using System;
using Dainty.UI.WindowBase;
using DG.Tweening;
using TicTacToe.Mechanics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.Windows.FinishWindow
{
    public class FinishWindowView : AWindowView
    {
        private const string _WIN_TEXT_KEY = "YOU WIN!";
        private const string _LOSE_TEXT_KEY = "YOU LOSE!";
        private const string _DRAW_TEXT_KEY = "WE HAVE A DRAW!";
        private const string _WIN_SIGN_TEXT_KEY = "<sprite name=\"{0}\"> WIN!";

#pragma warning disable 649
        [Header("TopPanelFilling")] [SerializeField]
        private Image _topFillPanel;

        [SerializeField] private Color _winTopColor;
        [SerializeField] private Color _loseTopColor;
        [SerializeField] private Color _drawTopColor;
        [SerializeField] private Color _noughtTopPanelColor;
        [SerializeField] private Color _crossTopPanelColor;

        [Header("TopPanelText")] [SerializeField]
        private TextMeshProUGUI _textTopPanel;

        [Header("Samurai")] [SerializeField] private Image _samuraiImage;
        [SerializeField] private Sprite _defaultSamurai;
        [SerializeField] private Sprite _crySamurai;
        [SerializeField] private Sprite _dabSamurai;

        [Header("Buttons")] [SerializeField] private Button _restart;
        [SerializeField] private Button _toMenu;
#pragma warning restore 649

        public event Action Restart;
        public event Action ToMenu;

        private void Start()
        {
            _restart.onClick.AddListener(() => Restart?.Invoke());
            _toMenu.onClick.AddListener(() => ToMenu?.Invoke());
        }

        public void SetFinishScreenAfterGameOver(EFinishResult result)
        {
            switch (result)
            {
                case EFinishResult.Win:
                    _topFillPanel.color = _winTopColor;
                    _textTopPanel.text = _WIN_TEXT_KEY;
                    _samuraiImage.sprite = _defaultSamurai;
                    DOVirtual.DelayedCall(1.85f, () => _samuraiImage.sprite = _dabSamurai);
                    break;
                case EFinishResult.Lose:
                    _topFillPanel.color = _loseTopColor;
                    _textTopPanel.text = _LOSE_TEXT_KEY;
                    _samuraiImage.sprite = _crySamurai;
                    break;
                case EFinishResult.Draw:
                    _topFillPanel.color = _drawTopColor;
                    _textTopPanel.text = _DRAW_TEXT_KEY;
                    _samuraiImage.sprite = _defaultSamurai;
                    break;
            }
        }

        public void SetFinishScreenAfterGameOver(ETttType winSign)
        {
            switch (winSign)
            {
                case ETttType.Cross:
                    _topFillPanel.color = _crossTopPanelColor;
                    _textTopPanel.text = string.Format(_WIN_SIGN_TEXT_KEY, winSign.ToString("g"));
                    _samuraiImage.sprite = _defaultSamurai;
                    DOVirtual.DelayedCall(1.85f, () => _samuraiImage.sprite = _dabSamurai);
                    break;
                case ETttType.Noughts:
                    _topFillPanel.color = _noughtTopPanelColor;
                    _textTopPanel.text = string.Format(_WIN_SIGN_TEXT_KEY, winSign.ToString("g"));
                    _samuraiImage.sprite = _defaultSamurai;
                    DOVirtual.DelayedCall(1.85f, () => _samuraiImage.sprite = _dabSamurai);
                    break;
                case ETttType.Draw:
                    _topFillPanel.color = _drawTopColor;
                    _textTopPanel.text = _DRAW_TEXT_KEY;
                    _samuraiImage.sprite = _defaultSamurai;
                    break;
            }
        }
    }
}