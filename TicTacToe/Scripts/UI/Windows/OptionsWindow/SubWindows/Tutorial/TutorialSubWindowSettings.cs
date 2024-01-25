namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.Tutorial
{
    public class TutorialSubWindowSettings
    {
        public readonly TutorialType Type;
        public readonly bool IsManualOpen;

        public TutorialSubWindowSettings(TutorialType type, bool isManualOpen)
        {
            Type = type;
            IsManualOpen = isManualOpen;
        }
    }
}