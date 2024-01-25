using System;
using Dainty;
using Dainty.Notifications;
using DG.Tweening;
using Newtonsoft.Json;
using TicTacToe.DI;
using TicTacToe.Players.Avatars;
using TicTacToe.SceneControllers.Params;
using TicTacToe.Services.Ads;
using TicTacToe.Services.Analytics;
using TicTacToe.Services.RateUs;
using TicTacToe.Services.Saving;
using TicTacToe.Settings;
using TicTacToe.Sound;
using TicTacToe.UI.Windows.OptionsWindow.SubWindows.Tutorial;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TicTacToe.SceneControllers
{
    public class BootstrapSceneController : ASceneController
    {
#pragma warning disable 649
        [SerializeField] private BoardViewsSettings _boardViewsSettings;
        [SerializeField] private AvatarsSettings _avatarsSettings;
        [SerializeField] private TutorialContentStorage _tutorialContentStorage;
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private SceneManagerWrapper _sceneManagerWrapper;
        [SerializeField] private BoardAnimatorManager boardAnimatorManager;

#if UNITY_EDITOR || DEV
        [Header("DEBUG")] [SerializeField] private Tayx.Graphy.GraphyManager _graphyManagerPrefab;
#endif
#pragma warning restore 649

        private DateTime _firstTimeOpenDatetime;

        protected override void Start()
        {
            DOTween.Init().SetCapacity(20, 5);

            if (!PlayerPrefs.HasKey(PlayerPrefsConsts.FIRST_TIME_OPEN_DATETIME))
            {
                var dateTime = DateTime.UtcNow;
                _firstTimeOpenDatetime = dateTime;
                PlayerPrefs.SetString(PlayerPrefsConsts.FIRST_TIME_OPEN_DATETIME,
                                      JsonConvert.SerializeObject(dateTime, Formatting.None));
            }
            else
            {
                var json = PlayerPrefs.GetString(PlayerPrefsConsts.FIRST_TIME_OPEN_DATETIME);
                _firstTimeOpenDatetime = JsonConvert.DeserializeObject<DateTime>(json);
            }

            ProjectContext.Bind(new SceneStartupParams());
            base.Start();
        }

        protected override void Initialize(SceneStartupParams @params)
        {
            var adsController = TicTacToeAdsControllerFactory.Create();
#if !MOTION_DESIGN
            if (AdDisplayLimiter.AdFreeSessions == 0)
            {
                var gdprAccepted = PlayerPrefs.GetInt(PlayerPrefsConsts.GDPR_ACCEPTED, 0) != 0;
                adsController.Initialize(gdprAccepted);
            }
            else
            {
                AdDisplayLimiter.TrackNewSession();
            }
#endif
            ProjectContext.Bind(adsController);
            ProjectContext.Bind<IBoardUtil>(new BoardUtil(_boardViewsSettings));
            ProjectContext.Bind<ISaveManager>(new SaveManager_Json_PlayerPrefs());
            ProjectContext.Bind(boardAnimatorManager);

            DontDestroyOnLoad(_sceneManagerWrapper);
            ProjectContext.Bind(_sceneManagerWrapper);

            var settingsManager = BindSettingsManager(_soundManager);
            _soundManager.MusicEnabled = settingsManager.MusicEnabled;
            _soundManager.SoundEnabled = settingsManager.SoundEnabled;

            DontDestroyOnLoad(_soundManager.gameObject);
            ProjectContext.Bind<ISoundManager>(_soundManager);

            ProjectContext.Bind(_avatarsSettings);
            ProjectContext.Bind(_tutorialContentStorage);

            var analytics = new TicTacToeAnalytics();
            analytics.Initialize();
            analytics.SetPropertiesOnce(settingsManager.MusicEnabled, settingsManager.SoundEnabled);
            analytics.NotificationsEnabled(NativeUtils.ApplicationUtils.GetNotificationsEnabled());
            ProjectContext.Bind<ITicTacToeAnalytics>(analytics);

            ProjectContext.Bind<IRateUsResolver>(new RateUsResolver());

            var asyncOperation = SceneManager.LoadSceneAsync("Main");
            asyncOperation.allowSceneActivation = false;
            DOVirtual.DelayedCall(1.5f, () => asyncOperation.allowSceneActivation = true);

            CreateNotifications();
#if DEV
            CreateGraphyMonitor();
#endif
        }

        private static ISettingsManager BindSettingsManager(ISoundManager soundManager)
        {
            var settingsManager = PlayerPrefsSettingsManager.Load();
            ProjectContext.Bind<ISettingsManager>(settingsManager);

            settingsManager.MusicEnabled.Subscribe(value => soundManager.MusicEnabled = value);
            settingsManager.SoundEnabled.Subscribe(value => soundManager.SoundEnabled = value);

#if DEV
            var adsController = ProjectContext.GetInstance<Dainty.Ads.IAdsController>();
            adsController.Enabled = settingsManager.DebugAdsEnabled;
            settingsManager.DebugAdsEnabled.Subscribe(value => adsController.Enabled = value);
#endif

            return settingsManager;
        }

        private void CreateNotifications()
        {
            var notificationsScheduler = new GameObject("NotificationsScheduler")
                .AddComponent<NotificationsScheduler>();
            DontDestroyOnLoad(notificationsScheduler.gameObject);

            const string LARGE_ICON = "round";
            const string SMALL_ICON = "small";

            notificationsScheduler.Initialize(NotificationsProviderFactory.Create(LARGE_ICON, SMALL_ICON));
            notificationsScheduler.Schedule += OnScheduleNotifications;
        }

        private void OnScheduleNotifications(INotificationsProvider notificationsProvider)
        {
            const string TITLE = "Hey, Samurai!";
            const string TEXT = "Do you wanna stretch your brains?";
            var fireTime = new TimeSpan(18, 58, 0);

            var dateTimeNow = DateTime.Now;
            var firstOpenDateBase = _firstTimeOpenDatetime.ToLocalTime().Date + fireTime;

            //Reminders
            var fireDateTime = dateTimeNow.Date.AddDays(7).Add(fireTime);
            notificationsProvider.ScheduleNotification(TITLE, TEXT, fireDateTime, TimeSpan.FromDays(7));

            //D1
            var dayOnePushDateTime = firstOpenDateBase.AddDays(1);
            if (dayOnePushDateTime > dateTimeNow)
            {
                notificationsProvider.ScheduleNotification(TITLE, TEXT, dayOnePushDateTime);
            }

            //D7
            var daySevenPushDateTime = firstOpenDateBase.AddDays(7);
            if (daySevenPushDateTime > dateTimeNow)
            {
                notificationsProvider.ScheduleNotification(TITLE, TEXT, daySevenPushDateTime);
            }
        }

#if DEV
        private void CreateGraphyMonitor()
        {
            var graphyManager = Instantiate(_graphyManagerPrefab);
            DontDestroyOnLoad(graphyManager.gameObject);
            StartCoroutine(GraphyCoroutine(graphyManager));
        }

        private System.Collections.IEnumerator GraphyCoroutine(Tayx.Graphy.GraphyManager graphyManager)
        {
            yield return null;

            var settingsManager = ProjectContext.GetInstance<ISettingsManager>();
            if (!settingsManager.DebugMonitorEnabled)
            {
                graphyManager.Disable();
            }

            settingsManager.DebugMonitorEnabled.Subscribe(isOn =>
            {
                if (isOn)
                {
                    graphyManager.Enable();
                }
                else
                {
                    graphyManager.Disable();
                }
            });
        }
#endif
    }
}