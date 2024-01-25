using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public static class TweenExtensions
{
    private class FrameAnimation
    {
        private Image _image;
        private Sprite[] _sprites;

        public int CurrentImage;

        public FrameAnimation(Image image, Sprite[] sprites)
        {
            _image = image;
            _sprites = sprites;
            CurrentImage = 0;
        }

        public void SetImage(int indImage)
        {
            _image.sprite = _sprites[indImage];
            CurrentImage = indImage;
        }
    }

    public static TweenerCore<int, int, NoOptions> DoFrameAnimation(this Image image, Sprite[] sprites, int fps)
    {
        var frameAnim = new FrameAnimation(image, sprites);
        TweenerCore<int, int, NoOptions> t = DOTween.To(() => frameAnim.CurrentImage, (x => frameAnim.SetImage(x)),
            sprites.Length - 1, sprites.Length / (float) fps);
        t.SetEase(Ease.Linear);
        return t;
    }
    
    public static TweenerCore<int, int, NoOptions> DoFrameAnimation(this Image image, Sprite[] sprites, float duration)
    {
        var frameAnim = new FrameAnimation(image, sprites);
        TweenerCore<int, int, NoOptions> t = DOTween.To(() => frameAnim.CurrentImage, (x => frameAnim.SetImage(x)),
            sprites.Length - 1, duration);
        t.SetEase(Ease.Linear);
        return t;
    }
}
