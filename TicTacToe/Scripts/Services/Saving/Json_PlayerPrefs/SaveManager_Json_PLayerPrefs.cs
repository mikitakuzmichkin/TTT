using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Base;
using TicTacToe.Players;
using UnityEngine;

namespace TicTacToe.Services.Saving
{
    public class SaveManager_Json_PlayerPrefs : ISaveManager
    {
        public const string PREFS_KEY = "SaveManager_Json_PlayerPrefs";

        private readonly Dictionary<EGameMode, string> _saves;

        public SaveManager_Json_PlayerPrefs()
        {
            if (PlayerPrefs.HasKey(PREFS_KEY))
            {
                try
                {
                    _saves = JsonConvert.DeserializeObject<Dictionary<EGameMode, string>>(
                        PlayerPrefs.GetString(PREFS_KEY));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            if (_saves == null)
            {
                _saves = new Dictionary<EGameMode, string>();
            }
        }

        public IGameSaveProvider CurrentSave { get; private set; }

        public bool SaveExists(EGameMode gameMode)
        {
            return _saves.ContainsKey(gameMode);
        }

        public IGameSaveProvider CreateSave(EGameMode gameMode, EPersonType personType, ABoardModel boardModel,
            ETttType yourSign)
        {
            var result = new GameSaveProvider_Json_Full(gameMode, personType, boardModel, yourSign, SaveAction);
            CurrentSave = result;
            return result;
        }

        public IGameSaveProvider Load(EGameMode gameMode)
        {
            if (_saves.ContainsKey(gameMode))
            {
                var result = new GameSaveProvider_Json_Full(_saves[gameMode], SaveAction);
                CurrentSave = result;
                return result;
            }

            throw new InvalidOperationException("No save found for selected game mode");
        }

        public void RemoveSave(EGameMode mode)
        {
            if (_saves.Remove(mode))
            {
                if (CurrentSave.GameMode == mode)
                {
                    CurrentSave = null;
                }

                SerializeAndSave();
            }
        }

        private void SaveAction(IGameSaveProvider provider, string json)
        {
            _saves[provider.GameMode] = json;
            SerializeAndSave();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SerializeAndSave()
        {
            PlayerPrefs.SetString(PREFS_KEY, JsonConvert.SerializeObject(_saves));
        }
    }
}