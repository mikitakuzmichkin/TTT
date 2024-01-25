using AppodealAds.Unity.Api;
using Dainty.Ads;

namespace TicTacToe.Services.Ads
{
    public static class TicTacToeAdsControllerFactory
    {
        public static IAdsController Create()
        {
#if UNITY_EDITOR
            return new EditorAdsController();
#else
            return new AppodealAdsControllerImpl();
#endif
        }

        private class AppodealAdsControllerImpl : AppodealAdsController
        {
            protected override string Key =>
#if UNITY_ANDROID
                "f2a44c44deb1598a512ac59250c6bae75405a94032ca9ec9";
#elif UNITY_IOS
                ""; //todo iOS key;
#endif

            public AppodealAdsControllerImpl(bool enabled = true)
                : base(Appodeal.INTERSTITIAL | Appodeal.BANNER_BOTTOM, enabled)
            {
            }

            public override void Initialize(bool gdprAccepted)
            {
                Appodeal.setSmartBanners(false);
                base.Initialize(gdprAccepted);
            }
        }
    }
}