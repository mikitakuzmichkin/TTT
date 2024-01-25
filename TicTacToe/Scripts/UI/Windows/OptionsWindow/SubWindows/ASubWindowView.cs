using Dainty.UI.WindowBase;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows
{
    public abstract class ASubWindowView : AWindowView
    {
        public AWindowAnimation WindowAnimation => Animation;
    }
}