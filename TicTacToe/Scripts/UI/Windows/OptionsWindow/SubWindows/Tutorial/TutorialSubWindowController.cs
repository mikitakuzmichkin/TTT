using System.Collections.Generic;
using System.Linq;
using Dainty.UI.WindowBase;
using TicTacToe.DI;
using TicTacToe.Services.Analytics;
using UnityEngine;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.Tutorial
{
    public class TutorialSubWindowController : ASubWindowController<TutorialSubWindowView>,
        IConfigurableWindow<TutorialSubWindowSettings>
    {
        private readonly TutorialContentStorage _tutorialContentStorage;
        private readonly ITicTacToeAnalytics _analytics;

        private TutorialSubWindowSettings _settings;
        private List<TutorialPackage> _content;

        private IEnumerable<TutorialType> _tutorialTypes;
        private float _openTime;
        private int _currentIndex;

        public override string WindowId => SubWindowsId.TUTORIAL;
        public override string Title => "How to play";

        public TutorialSubWindowController()
        {
            _tutorialContentStorage = ProjectContext.GetInstance<TutorialContentStorage>();
            _analytics = ProjectContext.GetInstance<ITicTacToeAnalytics>();
        }

        public void Initialize(TutorialSubWindowSettings data)
        {
            _settings = data;

            var type = data.Type;
            _content = new List<TutorialPackage>
            {
                new TutorialPackage(type, _tutorialContentStorage.ModeTutorials[type])
            };

            if (PlayerPrefs.GetInt(PlayerPrefsConsts.TUTORIAL_OPENS_NOT_FIRST_TIME, 0) == 0)
            {
                PlayerPrefs.SetInt(PlayerPrefsConsts.TUTORIAL_OPENS_NOT_FIRST_TIME, 1);

                if (type != TutorialType.CommonRules)
                {
                    var package = new TutorialPackage(TutorialType.CommonRules,
                                                      _tutorialContentStorage.ModeTutorials[TutorialType.CommonRules]);
                    _content.Insert(0, package);
                }
            }

            view.SetContent(_content);
            _openTime = Time.time;
            _currentIndex = 0;

            _tutorialTypes = _content.Select(package => package.Type);
            _analytics.TutorialOpen(_tutorialTypes, data.IsManualOpen);
        }

        protected override void OnSubscribe()
        {
            view.PageChanged += ViewOnPageChanged;
            uiManager.WindowClosing += UiManagerOnWindowClosing;
        }

        protected override void OnUnSubscribe()
        {
            view.PageChanged -= ViewOnPageChanged;
            uiManager.WindowClosing -= UiManagerOnWindowClosing;
        }

        private void UiManagerOnWindowClosing(IWindowController obj)
        {
            if (obj == this)
            {
                var totalTime = Time.time - _openTime;
                if (_currentIndex < _content.Select(package => package.Steps.Length).Sum() - 1)
                {
                    _analytics.TutorialSkip(_tutorialTypes, _currentIndex, totalTime);
                }
                else
                {
                    _analytics.TutorialComplete(_tutorialTypes, totalTime);
                }

                uiManager.WindowClosing -= UiManagerOnWindowClosing;
            }
        }

        private void ViewOnPageChanged(int index)
        {
            var currentType = _settings.Type;
            var i = 0;
            for (var j = 0; j < _content.Count; j++)
            {
                i += _content[j].Steps.Length;
                if (index < i)
                {
                    currentType = _content[j].Type;
                    break;
                }
            }

            _currentIndex = index;
            _analytics.TutorialStep(currentType, index, Time.time - _openTime);
        }
    }
}