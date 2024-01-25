using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe
{
    public class FrameAnimationComponent : MonoBehaviour
    {
        [SerializeField] private Sprite[] _sprites;

        [SerializeField] private Image _image;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _delayAfterAnimation = 0f;
        
        public void StartAnimation()
        {
            var seq = CreateAnim();
            seq.Play();
        }
        
        public Sequence ReturnAnimation()
        {
            var seq = CreateAnim();
            seq.Pause();
            return seq;
        }

        public void ForceAnimation()
        {
            _image.sprite = _sprites[_sprites.Length - 1];
        }

        private Sequence CreateAnim()
        {
            var seq = DOTween.Sequence();
            seq.AppendCallback(() =>_image.gameObject.SetActive(true));
            seq.Append(_image.DoFrameAnimation(_sprites, _duration));
            if (_delayAfterAnimation > 0)
            {
                seq.AppendInterval(_delayAfterAnimation);
            }
            return seq;
        }
    }
}