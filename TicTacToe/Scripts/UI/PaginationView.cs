using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI
{
    public class PaginationView : MonoBehaviour
    {
        [SerializeField] private Color _defaultBulletColor = Color.white;
        [SerializeField] private Color _activeBulletColor = Color.white;

        [Space] [SerializeField] private Image _bulletPrefab;
        [SerializeField] private Transform _container;

        private readonly List<(Image image, Tween tween)> _bullets = new List<(Image, Tween)>();
        private int _activeIndex = -1;

        private void Awake()
        {
            var bulletsCount = _container.childCount;
            for (var i = 0; i < bulletsCount; i++)
            {
                _bullets.Add((_container.GetChild(i).GetComponent<Image>(), null));
            }
        }

        public void Initialize(int count, int activeIndex = 0)
        {
            //spawn new bullets if need
            if (count > _bullets.Count)
            {
                var bulletsCount = _bullets.Count;
                for (var i = 0; i < count - bulletsCount; i++)
                {
                    var bullet = Instantiate(_bulletPrefab, _container);
                    _bullets.Add((bullet, null));
                }
            }

            //reset bullets color
            for (var i = 0; i < _bullets.Count; i++)
            {
                _bullets[i].image.color = _defaultBulletColor;
            }

            //enable bullets
            for (var i = 0; i < count; i++)
            {
                var bulletObj = _bullets[i].image.gameObject;
                if (!bulletObj.activeSelf)
                {
                    bulletObj.SetActive(true);
                }
            }

            //disable extra bullets
            for (var i = count; i < _bullets.Count; i++)
            {
                var bulletObj = _bullets[i].image.gameObject;
                if (bulletObj.activeSelf)
                {
                    bulletObj.SetActive(false);
                }
            }

            SetActiveBullet(activeIndex);
        }

        public void SetActiveBullet(int index)
        {
            if (_activeIndex != -1)
            {
                var prevBullet = _bullets[_activeIndex];
                prevBullet.tween?.Kill();
                prevBullet.tween = prevBullet.image.DOColor(_defaultBulletColor, 0.3f);
            }

            var bullet = _bullets[index];
            bullet.tween?.Kill();
            bullet.tween = bullet.image.DOColor(_activeBulletColor, 0.3f);

            _activeIndex = index;
        }
    }
}