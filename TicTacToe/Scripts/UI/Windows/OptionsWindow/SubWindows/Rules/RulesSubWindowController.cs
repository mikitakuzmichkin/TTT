using Dainty.UI;
using TicTacToe.UI.Windows.OptionsWindow.SubWindows.Tutorial;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.Rules
{
    public class RulesSubWindowController : ASubWindowController<RulesSubWindowView>
    {
        public override string WindowId => SubWindowsId.RULES;
        public override string Title => "How to play";

        protected override void OnSubscribe()
        {
            view.ToCommonRules += ViewOnToCommonRules;
            view.ToClassicRules += ViewOnToClassicRules;
            view.ToAlternativeRules += ViewOnToAlternativeRules;
            view.ToStrategyRules += ViewOnToStrategyRules;
        }

        protected override void OnUnSubscribe()
        {
            view.ToCommonRules -= ViewOnToCommonRules;
            view.ToClassicRules -= ViewOnToClassicRules;
            view.ToAlternativeRules -= ViewOnToAlternativeRules;
            view.ToStrategyRules -= ViewOnToStrategyRules;
        }

        private void ViewOnToCommonRules()
        {
            OpenTutorial(TutorialType.CommonRules);
        }

        private void ViewOnToClassicRules()
        {
            OpenTutorial(TutorialType.Classic);
        }

        private void ViewOnToAlternativeRules()
        {
            OpenTutorial(TutorialType.Alternative);
        }

        private void ViewOnToStrategyRules()
        {
            OpenTutorial(TutorialType.Strategy);
        }

        private void OpenTutorial(TutorialType gameMode)
        {
            var settings = new TutorialSubWindowSettings(gameMode, true);
            uiManager.Open<TutorialSubWindowController, TutorialSubWindowSettings>(
                settings, WindowTransition.AnimateClosing | WindowTransition.AnimateOpening);
        }
    }
}