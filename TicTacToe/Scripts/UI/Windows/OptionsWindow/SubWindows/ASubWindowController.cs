using Dainty.UI;
using Dainty.UI.WindowBase;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows
{
    public abstract class ASubWindowController<T> : AWindowController<T>, ISubWindowController where T : ASubWindowView
    {
        public abstract string Title { get; }

        protected override void OnEscape()
        {
            if (uiManager.WindowsCount > 1)
            {
                uiManager.Back(WindowTransition.AnimateOpening | WindowTransition.AnimateClosing);
            }
        }
    }
}