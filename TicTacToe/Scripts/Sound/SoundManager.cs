using Dainty.Sound;
using DG.Tweening;
using UnityEngine;

namespace TicTacToe.Sound
{
    public class SoundManager : DaintySoundManager, ISoundManager
    {
        [SerializeField] private GameSounds _sounds;

        private Tween _tween;

        public override bool MusicEnabled
        {
            get => base.MusicEnabled;
            set
            {
                base.MusicEnabled = value;
                if (!value)
                {
                    _musicSource.volume = 0;
                }
                else if (_musicSource.clip != null)
                {
                    _tween?.Kill();
                    _tween = _musicSource.DOFade(1, 0.5f)
                                         .OnComplete(() => _tween = null);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _musicSource.volume = 0;
        }

        public void MusicStart(GameSound sound)
        {
            if (_sounds.Sounds.TryGetValue(sound, out var clip))
            {
                MusicStart(clip);
            }
            else
            {
                Debug.LogWarningFormat("Cannot play \"{0}\"! Sound not found.", sound);
            }
        }

        public override void MusicStart(AudioClip clip)
        {
            if (_musicEnabled)
            {
                _tween?.Kill();
                _tween = _musicSource.DOFade(0, 0.5f * _musicSource.volume)
                                     .OnComplete(() =>
                                     {
                                         _musicSource.clip = clip;
                                         _musicSource.Play();
                                         _tween = _musicSource.DOFade(1, 0.5f)
                                                              .OnComplete(() => _tween = null);
                                     });
            }
            else
            {
                _musicSource.clip = clip;
            }
        }

        public void Sound(GameSound sound, bool playCompletely = false)
        {
            if (_sounds.Sounds.TryGetValue(sound, out var clip))
            {
                Sound(clip, playCompletely);
            }
            else
            {
                Debug.LogWarningFormat("Cannot play \"{0}\"! Sound not found.", sound);
            }
        }
    }
}