using UnityEngine;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.About
{
    public class AboutSubWindowController : ASubWindowController<AboutSubWindowView>
    {
        public override string WindowId => SubWindowsId.ABOUT;
        public override string Title => "About";

        public override void BeforeShow()
        {
            base.BeforeShow();
            view.SetName(Application.productName);
            view.SetVersion(Application.version);
        }

        protected override void OnSubscribe()
        {
            view.ToMoreGames += ToMoreGames;
            view.ToTerms += ToTerms;
            view.ToPrivacyPolicy += ToPrivacyPolicy;
        }

        protected override void OnUnSubscribe()
        {
            view.ToMoreGames -= ToMoreGames;
            view.ToTerms -= ToTerms;
            view.ToPrivacyPolicy -= ToPrivacyPolicy;
        }

        public void ToMoreGames()
        {
            Application.OpenURL(Constants.URL_MORE_GAMES);
        }

        public void ToTerms()
        {
            Application.OpenURL(Constants.URL_EULA);
        }

        public void ToPrivacyPolicy()
        {
            Application.OpenURL(Constants.URL_PRIVACY_POLICY);
        }
    }
}