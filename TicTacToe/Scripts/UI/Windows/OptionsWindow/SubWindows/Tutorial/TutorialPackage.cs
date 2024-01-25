using UnityEngine;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.Tutorial
{
    public class TutorialPackage
    {
        public readonly TutorialType Type;
        public readonly RectTransform[] Steps;

        public TutorialPackage(TutorialType type, RectTransform[] steps)
        {
            Type = type;
            Steps = steps;
        }
    }
}