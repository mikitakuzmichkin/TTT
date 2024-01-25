using Dainty.UI.WindowBase;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows
{
    public interface ISubWindowController : IWindowController
    {
        string Title { get; }
    }
}