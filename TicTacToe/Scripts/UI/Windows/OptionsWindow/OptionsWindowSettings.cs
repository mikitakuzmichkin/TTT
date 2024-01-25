using TicTacToe.UI.Windows.OptionsWindow.SubWindows.Tutorial;

namespace TicTacToe.UI.Windows.OptionsWindow
{
    public class OptionsWindowSettings
    {
        public enum StartupParams
        {
            Default,
            Tutorial
        }

        public readonly StartupParams Params;
        public readonly TutorialType TutorialType;
        public readonly bool IsManualTutorialOpen;

        private OptionsWindowSettings(StartupParams @params, TutorialType tutorialType,
            bool isManualTutorialOpen)
        {
            TutorialType = tutorialType;
            Params = @params;
            IsManualTutorialOpen = isManualTutorialOpen;
        }

        public static OptionsWindowSettings OpenTutorial(TutorialType type, bool isManualOpen)
        {
            return new OptionsWindowSettings(StartupParams.Tutorial, type, isManualOpen);
        }
    }
}