using Dainty.UI;
using Dainty.UI.WindowBase;
using TicTacToe.UI.Windows.OptionsWindow.SubWindows.Options;
using TicTacToe.UI.Windows.OptionsWindow.SubWindows.Tutorial;

namespace TicTacToe.UI.Windows.OptionsWindow
{
    public class OptionsWindowController : AWindowController<OptionsWindowView>,
        IConfigurableWindow<OptionsWindowSettings>
    {
        private UiManager _uiManager;
        public override string WindowId => WindowsId.OPTIONS;

        protected override void OnInitialize()
        {
            _uiManager = new SubUiManager(view.SlidesUiRoot, view.SlidesUiManagerSettings,
                                          (value, animate) => view.SetTitle(value, animate));
            _uiManager.Open<OptionsMainSubWindowController>(WindowTransition.None);
        }

        public void Initialize(OptionsWindowSettings data)
        {
            if (data.Params == OptionsWindowSettings.StartupParams.Tutorial)
            {
                _uiManager.Back(WindowTransition.None);
                _uiManager.Open<TutorialSubWindowController, TutorialSubWindowSettings>(
                    new TutorialSubWindowSettings(data.TutorialType, data.IsManualTutorialOpen));
            }
        }

        protected override void OnSubscribe()
        {
            view.DoneButton += DoneButtonClick;
            view.BackToParameters += BackButtonClick;
        }

        protected override void OnUnSubscribe()
        {
            view.DoneButton -= DoneButtonClick;
            view.BackToParameters -= BackButtonClick;
        }

        private void BackButtonClick()
        {
            if (_uiManager.WindowsCount > 1)
            {
                _uiManager.Back(WindowTransition.AnimateOpening | WindowTransition.AnimateClosing);
            }
            else
            {
                DoneButtonClick();
            }
        }

        private void DoneButtonClick()
        {
            uiManager.Back(() =>
            {
                while (_uiManager.Back(WindowTransition.None))
                {
                }
            });
        }

        protected override void OnEscape()
        {
            if (_uiManager.WindowsCount > 1)
            {
                return;
            }

            DoneButtonClick();
        }
    }
}