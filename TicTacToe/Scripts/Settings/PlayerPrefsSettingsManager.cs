using System;
using Newtonsoft.Json;
using UnityEngine;

namespace TicTacToe.Settings
{
    public class PlayerPrefsSettingsManager : ISettingsManager
    {
        private readonly JsonData _jsonData;

        public RxProperty<bool> MusicEnabled { get; set; }
        public RxProperty<bool> SoundEnabled { get; set; }
#if DEV
        public RxProperty<bool> DebugAdsEnabled { get; set; }
        public RxProperty<bool> DebugMonitorEnabled { get; set; }
#endif

        private PlayerPrefsSettingsManager(JsonData data)
        {
            _jsonData = data;

            MusicEnabled = new RxProperty<bool>(_jsonData.MusicEnabled);
            SoundEnabled = new RxProperty<bool>(_jsonData.SoundEnabled);

            MusicEnabled.Subscribe(value =>
            {
                _jsonData.MusicEnabled = value;
                Save();
            });

            SoundEnabled.Subscribe(value =>
            {
                _jsonData.SoundEnabled = value;
                Save();
            });

#if DEV
            DebugAdsEnabled = new RxProperty<bool>(_jsonData.DebugAdsEnabled);
            DebugMonitorEnabled = new RxProperty<bool>(_jsonData.DebugMonitorEnabled);
            
            DebugAdsEnabled.Subscribe(value =>
            {
                _jsonData.DebugAdsEnabled = value;
                Save();
            });
            
            DebugMonitorEnabled.Subscribe(value =>
            {
                _jsonData.DebugMonitorEnabled = value;
                Save();
            });
#endif
        }

        private void Save()
        {
            var json = JsonConvert.SerializeObject(_jsonData, Formatting.None);
            PlayerPrefs.SetString(PlayerPrefsConsts.SETTINGS, json);
        }

        public static PlayerPrefsSettingsManager Load()
        {
            var json = PlayerPrefs.GetString(PlayerPrefsConsts.SETTINGS);
            var data = JsonConvert.DeserializeObject<JsonData>(json);
            if (data == null)
            {
                data = GetDefaultSettingsData();

                var result = new PlayerPrefsSettingsManager(data);
                result.Save();
                return result;
            }

            return new PlayerPrefsSettingsManager(data);
        }

        private static JsonData GetDefaultSettingsData()
        {
            return new JsonData
            {
                MusicEnabled = true,
                SoundEnabled = true,
#if DEV
                DebugAdsEnabled = true,
                DebugMonitorEnabled = false
#endif
            };
        }

        [Serializable]
        private class JsonData
        {
            [JsonProperty("music_enabled")] public bool MusicEnabled;
            [JsonProperty("sound_enabled")] public bool SoundEnabled;
#if DEV
            [JsonProperty("debug_ads_enabled")] public bool DebugAdsEnabled;
            [JsonProperty("debug_monitor_enabled")] public bool DebugMonitorEnabled;
#endif
        }
    }
}