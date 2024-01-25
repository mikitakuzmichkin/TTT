using Dainty.UI.WindowBase;
using TicTacToe.DI;
using TicTacToe.Services.Analytics;
using TicTacToe.Services.RateUs;
using UnityEngine;

namespace TicTacToe.UI.Windows.RateUs
{
    public class RateUsWindowController : AWindowController<RateUsWindowView>, IConfigurableWindow<RateUsWindowSettings>
    {
        private readonly ITicTacToeAnalytics _analytics;

        private RateUsWindowSettings _settings;
        private RateUsResult _result = RateUsResult.Skipped;

        public override string WindowId => WindowsId.RATE_US;

        public RateUsWindowController()
        {
            _analytics = ProjectContext.GetInstance<ITicTacToeAnalytics>();
        }

        public void Initialize(RateUsWindowSettings data)
        {
            _settings = data;
            _analytics.RateUsShown();
        }

        protected override void OnSubscribe()
        {
            view.RateNowButtonClick += OnRateNowButtonClick;
            view.RemindLaterButtonClick += OnRemindLaterButtonClick;
        }

        protected override void OnUnSubscribe()
        {
            view.RateNowButtonClick -= OnRateNowButtonClick;
            view.RemindLaterButtonClick -= OnRemindLaterButtonClick;
        }

        protected override void OnEscape()
        {
            OnRemindLaterButtonClick();
        }

        private void OnRateNowButtonClick()
        {
            _result = RateUsResult.Rated;
            _analytics.RateUsClosed(_result);
            Application.OpenURL(Constants.RATE_US_URL);
            uiManager.Back();
        }

        private void OnRemindLaterButtonClick()
        {
            _result = RateUsResult.Skipped;
            _analytics.RateUsClosed(_result);
            uiManager.Back();
        }

        public override void Dispose()
        {
            base.Dispose();

            ProjectContext.GetInstance<IRateUsResolver>().RegisterResult(_result);
            _settings.OnClosed?.Invoke();
        }
    }
}