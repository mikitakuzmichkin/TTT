using TicTacToe.DI;
using TicTacToe.Players;
using TicTacToe.Players.Avatars;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.Game
{
    public class PlayerInGameView : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TurnTimer _turnTimer;
#pragma warning restore 649

        private AvatarsSettings _avatarsSettings;

        public bool TurnTimerActive
        {
            get => _turnTimer.gameObject.activeSelf;
            set => _turnTimer.gameObject.SetActive(value);
        }

        private void Awake()
        {
            _avatarsSettings = ProjectContext.GetInstance<AvatarsSettings>();
        }

        public void Initialize(IPlayerModel model)
        {
            _nameText.text = model.Name;
            _avatarImage.sprite = _avatarsSettings.Avatars[model.Avatar];
        }
    }
}