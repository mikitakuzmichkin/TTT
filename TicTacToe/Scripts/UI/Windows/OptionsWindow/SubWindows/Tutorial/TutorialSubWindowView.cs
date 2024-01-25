using System;
using System.Collections.Generic;
using Dainty.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.Tutorial
{
    public class TutorialSubWindowView : ASubWindowView
    {
        [SerializeField] private ScrollRect _scroll;
        [SerializeField] private HorizontalScrollSnap _scrollSnap;
        [SerializeField] private PaginationView _paginationView;

        [Header("SafeArea")] [SerializeField] private SafeArea _safeArea;
        [SerializeField] private RectTransform _bottomPanelBackground;

        private int _pagesCount;
        private int _lastPage;

        public event Action<int> PageChanged;

        private void Start()
        {
            _scrollSnap.OnSelectionPageChangedEvent.AddListener(page => _paginationView.SetActiveBullet(page));

            _safeArea.Changed += OnSafeAreaApplied;
            OnSafeAreaApplied();
        }

        protected override void OnSubscribe()
        {
            _scroll.enabled = true;
            _scroll.onValueChanged.AddListener(OnScrollValueChanged);
        }

        private void OnScrollValueChanged(Vector2 arg0)
        {
            var currentPage = Mathf.FloorToInt(Mathf.Clamp(arg0.x * _pagesCount, 0, _pagesCount - 1));
            if (currentPage != _lastPage)
            {
                _lastPage = currentPage;
                PageChanged?.Invoke(currentPage);
            }
        }

        protected override void OnUnSubscribe()
        {
            _scroll.enabled = false;
            _scroll.onValueChanged.RemoveListener(OnScrollValueChanged);
        }

        public void SetContent(IReadOnlyList<TutorialPackage> packages)
        {
            var container = _scrollSnap._screensContainer;

            //clear container
            for (var i = container.childCount - 1; i >= 0; i--)
            {
                var child = container.GetChild(i).gameObject;
                child.SetActive(false);
                Destroy(child);
            }

            container.DetachChildren();
            _pagesCount = 0;

            //fill container
            var totalSlidesCount = 0;
            for (var i = 0; i < packages.Count; i++)
            {
                var slides = packages[i].Steps;
                var slidesCount = slides.Length;
                totalSlidesCount += slidesCount;

                for (var j = 0; j < slidesCount; j++)
                {
                    Instantiate(slides[j], container);
                }

                _pagesCount += slidesCount;
            }

            _paginationView.Initialize(totalSlidesCount);
            _scrollSnap.InitialiseChildObjectsFromScene();
            _scrollSnap.CurrentPage = 0;
            _scrollSnap.UpdateLayout();
        }

        private void OnSafeAreaApplied()
        {
            var offsetMin = _bottomPanelBackground.offsetMin;
            var safeArea = UiRoot.GetSafeArea();
            offsetMin.y = -safeArea.y;
            _bottomPanelBackground.offsetMin = offsetMin;
        }
    }
}