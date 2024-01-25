using System;
using Newtonsoft.Json;
using UnityEngine;

namespace TicTacToe.Services.Ads
{
    public static class AdDisplayLimiter
    {
        private class Data
        {
            public DateTime LastInterstitialShown;
            public int AdFreeSessions;
        }

        private static Data _data;

        public static DateTime LastInterstitialShown => _data.LastInterstitialShown;
        public static int AdFreeSessions => _data.AdFreeSessions;

        static AdDisplayLimiter()
        {
            var json = PlayerPrefs.GetString(PlayerPrefsConsts.AD_DISPLAY_LIMITER_DATA);
            try
            {
                _data = JsonConvert.DeserializeObject<Data>(json);
            }
            catch
            {
            }

            if (_data == null)
            {
                _data = new Data
                {
                    LastInterstitialShown = DateTime.MinValue,
                    AdFreeSessions = 0
                };
            }
        }

        public static bool CanShow(TimeSpan minInterval)
        {
            return _data.LastInterstitialShown + minInterval <= DateTime.UtcNow;
        }

        public static void TrackInterstitial()
        {
            _data.LastInterstitialShown = DateTime.UtcNow;
            Save();
        }

        public static void TrackNewSession()
        {
            if (_data.AdFreeSessions > 0)
            {
                _data.AdFreeSessions--;
                Save();
            }
        }

        private static void Save()
        {
            PlayerPrefs.SetString(PlayerPrefsConsts.AD_DISPLAY_LIMITER_DATA, JsonConvert.SerializeObject(_data));
        }
    }
}