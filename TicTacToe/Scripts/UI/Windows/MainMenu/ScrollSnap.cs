using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TicTacToe.UI.Windows.MainMenu
{
    public class ScrollSnap : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
#pragma warning disable 649
        [SerializeField] public float _percentThreshold = 0.2f;
        [SerializeField] public float _easing = 0.5f;
        [SerializeField] private RectTransform _contentRect;
#pragma warning restore 649

        private Coroutine _coroutine;
        private int _currentPage;
        private float _dragBeginTime;

        private bool _interactable = true;
        private Vector3 _panelLocation;

        public bool Interactable
        {
            get => _interactable;
            set => _interactable = value;
        }

        public int PagesCount => _contentRect.childCount;

        public int CurrentPage
        {
            get => _currentPage;
            set => SetCurrentPage(value);
        }

        public event Action<int> PageChanged;

        private void Start()
        {
            _panelLocation = _contentRect.localPosition;
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

            var difference = (ConvertScreenPos(data.position) - ConvertScreenPos(data.pressPosition)).x;
            _contentRect.localPosition = _panelLocation + new Vector3(difference, 0, 0);
        }

        public void OnEndDrag(PointerEventData data)
        {
            if (!_interactable)
            {
                return;
            }

            var width = GetComponent<RectTransform>().rect.width;

            var percentage = (ConvertScreenPos(data.position) - ConvertScreenPos(data.pressPosition)).x / width;
            var durationScale = 1f;
            if (Mathf.Abs(percentage) >= _percentThreshold)
            {
                if (percentage < 0 && _currentPage + 1 < PagesCount)
                {
                    _currentPage++;
                    PageChanged?.Invoke(_currentPage);
                    _panelLocation += new Vector3(-width, 0, 0);
                }
                else if (percentage > 0 && _currentPage > 0)
                {
                    _currentPage--;
                    PageChanged?.Invoke(_currentPage);
                    _panelLocation += new Vector3(width, 0, 0);
                }

                var dragTime = Time.time - _dragBeginTime;
                durationScale = dragTime >= _easing ? 1 : dragTime / _easing;
            }

            StartSmoothMove(_panelLocation, durationScale);
        }

        public void SetCurrentPage(int index, bool animate = true)
        {
            var endPos = _contentRect.localPosition + (index - _currentPage) * Screen.width * Vector3.left;

            if (animate)
            {
                StartSmoothMove(endPos, 1);
            }
            else
            {
                _contentRect.localPosition = endPos;
            }
        }

        private void StartSmoothMove(Vector3 endPos, float durationScale)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            var startPos = _contentRect.localPosition;

            var width = GetComponent<RectTransform>().rect.width;
            var time = Vector2.Distance(startPos, endPos) / width * _easing * durationScale;
            _coroutine = StartCoroutine(SmoothMove(startPos, endPos, time));
        }

        private IEnumerator SmoothMove(Vector3 startPos, Vector3 endPos, float time)
        {
            var t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / time;
                _contentRect.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }

        private Vector2 ConvertScreenPos(Vector2 data)
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(data);
            return _contentRect.parent != null ? _contentRect.parent.InverseTransformPoint(worldPoint) : worldPoint;
        }
    }
}