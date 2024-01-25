using System;
using Newtonsoft.Json;
using UnityEngine;

namespace TicTacToe.Services.RateUs
{
    public class RateUsResolver : IRateUsResolver
    {
        [Serializable]
        private class Data
        {
            [JsonProperty("times_skipped")] public int TimesSkipped;
            [JsonProperty("show_after")] public DateTime ShowAfter;
        }

        public bool CanBeShown
        {
            get
            {
                if (TryGetFirstOpenDatetime(out var dateTime))
                {
                    var utcNow = DateTime.UtcNow;
                    if (dateTime.AddDays(2) <= utcNow && GetData().ShowAfter <= utcNow)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void RegisterResult(RateUsResult result)
        {
            Data newData;
            var utcNow = DateTime.UtcNow;

            switch (result)
            {
                case RateUsResult.Rated:
                {
                    newData = new Data {ShowAfter = utcNow.AddMonths(1)};
                    break;
                }
                case RateUsResult.Skipped:
                {
                    newData = GetData();
                    switch (newData.TimesSkipped)
                    {
                        case 0:
                            newData.ShowAfter = utcNow.AddDays(4);
                            break;
                        case 1:
                            newData.ShowAfter = utcNow.AddDays(7);
                            break;
                        default:
                            newData.ShowAfter = utcNow.AddMonths(1);
                            break;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(result));
            }

            var json = JsonConvert.SerializeObject(newData);
            PlayerPrefs.SetString(PlayerPrefsConsts.RATE_US_RESOLVER_DATA, json);
        }

        private static bool TryGetFirstOpenDatetime(out DateTime dateTime)
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsConsts.FIRST_TIME_OPEN_DATETIME))
            {
                dateTime = default;
                return false;
            }

            var json = PlayerPrefs.GetString(PlayerPrefsConsts.FIRST_TIME_OPEN_DATETIME, null);
            try
            {
                dateTime = JsonConvert.DeserializeObject<DateTime>(json);
                return true;
            }
            catch (Exception e)
            {
#if DEV_LOG
                Debug.LogWarningFormat("Exception occurred while getting \"FIRST_TIME_OPEN_DATETIME\".\n{0}", e);
#endif
                dateTime = default;
                return false;
            }
        }

        private static Data GetData()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsConsts.RATE_US_RESOLVER_DATA))
            {
                return new Data {ShowAfter = DateTime.UtcNow.AddSeconds(-1)};
            }

            var json = PlayerPrefs.GetString(PlayerPrefsConsts.RATE_US_RESOLVER_DATA, null);
            try
            {
                return JsonConvert.DeserializeObject<Data>(json);
            }
            catch (Exception e)
            {
#if DEV_LOG
                Debug.LogWarningFormat("Exception occurred while getting \"RATE_US_RESOLVER_DATA\".\n{0}", e);
#endif
                return new Data {ShowAfter = DateTime.UtcNow.AddSeconds(-1)};
            }
        }
    }
}