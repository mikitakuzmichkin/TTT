using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TicTacToe.UI.Windows.MainMenu
{
    [RequireComponent(typeof(RectTransform))]
    public class GameModeScrollSnap : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
#pragma warning disable 649
        [SerializeField] private RectTransform _container;
        [SerializeField] public float _percentThreshold = 0.2f;
        [SerializeField] public float _easing = 0.5f;
#pragma warning restore 649

        private Coroutine _coroutine;
        private int _currentPage;
        private float _dragBeginTime;

        private bool _interactable = true;
        private readonly List<Transform> _children = new List<Transform>();

        public bool Interactable
        {
            get => _interactable;
            set => _interactable = value;
        }

        public int PagesCount => _children.Count;

        public int CurrentPage
        {
            get => _currentPage;
            set => SetCurrentPage(value);
        }

        public event Action<int> PageChanged;

        private void Awake()
        {
            Refresh();
        }

        public void Refresh()
        {
            _children.Clear();
            var childCount = _container.childCount;
            var width = _container.rect.width;

            var child = _container.GetChild(0);
            _children.Add(child);
            child.localPosition = Vector3.zero;

            var rightPos = Vector3.right * width;
            for (var i = 1; i < childCount; i++)
            {
                child = _container.GetChild(i);
                _children.Add(child);

                child.localPosition = rightPos;
            }

            _currentPage = 0;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_interactable && _coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
                _dragBeginTime = Time.time;
            }
        }

        public void OnDrag(PointerEventData data)
        {
            if (!_interactable)
            {
                return;
            }

            var delta = (ConvertScreenPos(data.pressPosition) - ConvertScreenPos(data.position)).x;
            var width = _container.rect.width;
            OnStep(_currentPage + Mathf.Clamp(delta / width, -1,1));
        }

        public void OnEndDrag(PointerEventData data)
        {
            if (!_interactable)
            {
                return;
            }

            var width = _container.rect.width;

            var percentage = (ConvertScreenPos(data.position) - ConvertScreenPos(data.pressPosition)).x / width;
            var durationScale = 1f;
            var toPage = _currentPage;
            if (Mathf.Abs(percentage) >= _percentThreshold)
            {
                if (percentage < 0 && _currentPage + 1 < PagesCount)
                {
                    toPage++;
                    PageChanged?.Invoke(_currentPage);
                }
                else if (percentage > 0 && _currentPage > 0)
                {
                    toPage--;
                    PageChanged?.Invoke(_currentPage);
                }

                var dragTime = Time.time - _dragBeginTime;
                durationScale = dragTime >= _easing ? 1 : dragTime / _easing;
            }

            StartSmoothMove(toPage, durationScale);
        }

        public void SetCurrentPage(int index, bool animate = true)
        {
            if (index < 0 || index >= PagesCount)
            {
                throw new IndexOutOfRangeException();
            }

            if (animate)
            {
                StartSmoothMove(index, 1);
            }
            else
            {
                OnStep(index);
                _currentPage = index;
                PageChanged?.Invoke(_currentPage);
            }
        }

        private void StartSmoothMove(int toPage, float durationScale)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            var width = _container.rect.width;
            var startPos = _children[toPage > _currentPage ? toPage : _currentPage].localPosition;
            var percent = toPage > _currentPage
                ? startPos.magnitude / width
                : 1 - startPos.magnitude / width;

            var time = percent * _easing * durationScale;
            _coroutine = StartCoroutine(SmoothMove(toPage, time));
        }

        private IEnumerator SmoothMove(int toPage, float time)
        {
            float from;
            if (toPage == _currentPage)
            {
                if (_children[toPage].localPosition.x == 0)
                {
                    from = 1 - _children[toPage + 1].localPosition.x / _container.rect.width;
                }
                else
                {
                    from = -_children[toPage].localPosition.x / _container.rect.width;
                }
            }
            else if (toPage < _currentPage)
            {
                from = -_children[_currentPage].localPosition.x / _container.rect.width;
            }
            else
            {
                from = 1 - _children[toPage].localPosition.x / _container.rect.width;
            }

            from += _currentPage;

            var t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / time;
                var percent = Mathf.SmoothStep(from, toPage, t);
                OnStep(percent);

                var newCurrentPage = Mathf.RoundToInt(percent);
                if (_currentPage != newCurrentPage)
                {
                    _currentPage = newCurrentPage;
                    PageChanged?.Invoke(newCurrentPage);
                }

                yield return null;
            }
        }

        private Vector2 ConvertScreenPos(Vector2 data)
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(data);
            return _container.InverseTransformPoint(worldPoint);
        }

        private void OnStep(float percent)
        {
            var width = _container.rect.width;
            var to = percent > _currentPage ? Mathf.CeilToInt(percent) : Mathf.FloorToInt(percent);
            var percentNormalized = percent - (float) Math.Truncate(percent);
            if (percent > _currentPage && percentNormalized == 0)
            {
                percentNormalized = 1;
            }

            if (to >= PagesCount)
            {
                //_children[from].localPosition = Vector3.left * (width * percent);
            }
            else if (to < 0)
            {
                //_children[from].localPosition = Vector3.right * (width * percent);
            }
            else if (to > _currentPage)
            {
                for (var i = 0; i < to - 1; i++)
                {
                    _children[i].GetComponent<CanvasGroup>().alpha = 0;
                    _children[i].localPosition = Vector3.zero;
                }

                if (to > 0)
                {
                    _children[to - 1].GetComponent<CanvasGroup>().alpha = 1 - percentNormalized;
                    _children[to - 1].localPosition = Vector3.zero;
                }

                _children[to].localPosition = Vector3.right * (width * (1 - percentNormalized));
            }
            else if (_currentPage > to)
            {
                var right = Vector3.right * width;
                for (var i = to + 1; i < PagesCount; i++)
                {
                    _children[i].GetComponent<CanvasGroup>().alpha = 1;
                    _children[i].localPosition = right;
                }

                _children[to].GetComponent<CanvasGroup>().alpha = 1 - percentNormalized;
                _children[to].localPosition = Vector3.zero;

                if (to + 1 < PagesCount)
                {
                    _children[to + 1].localPosition = Vector3.right * (width * (1 - percentNormalized));
                }
            }
        }
    }
}