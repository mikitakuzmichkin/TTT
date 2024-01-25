using Dainty.UI.WindowBase;
using TicTacToe.Players;

namespace TicTacToe.UI.Windows.MainMenu.GameWebTypeSelector
{
    public class GameTypeSelectorWindowController : AWindowController<GameTypeSelectorWindowView>,
        IConfigurableWindow<GameTypeSelectorWindowSettings>
    {
        private GameTypeSelectorWindowSettings _settings;

        public override string WindowId => WindowsId.GAME_WEB_TYPE_SELECTOR;

        public void Initialize(GameTypeSelectorWindowSettings data)
        {
            _settings = data;
        }

        protected override void OnSubscribe()
        {
            view.CloseButtonClick += OnCloseButtonClick;
            view.PlayWithBotButtonClick += OnPlayWithBotButtonClick;
            view.PlayWithFriendButtonClick += OnPlayWithFriendButtonClick;
            view.PlayOnlineButtonClick += OnPlayOnlineButtonClick;
        }

        protected override void OnUnSubscribe()
        {
            view.CloseButtonClick -= OnCloseButtonClick;
            view.PlayWithBotButtonClick -= OnPlayWithBotButtonClick;
            view.PlayWithFriendButtonClick -= OnPlayWithFriendButtonClick;
            view.PlayOnlineButtonClick -= OnPlayOnlineButtonClick;
        }

        private void OnCloseButtonClick()
        {
            uiManager.Back();
        }

        private void OnPlayWithBotButtonClick()
        {
            _settings?.OnGameTypeSelected?.Invoke(EWebType.Offline, EPersonType.Bot);
            uiManager.Back();
        }

        private void OnPlayWithFriendButtonClick()
        {
            _settings?.OnGameTypeSelected?.Invoke(EWebType.Offline, EPersonType.Human);
            uiManager.Back();
        }

        private void OnPlayOnlineButtonClick()
        {
            _settings?.OnGameTypeSelected?.Invoke(EWebType.Online, EPersonType.Human);
            uiManager.Back();
        }
    }
}