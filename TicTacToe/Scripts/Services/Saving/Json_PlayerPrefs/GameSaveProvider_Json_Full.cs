using System;
using Newtonsoft.Json;
using TicTacToe.Mechanics;
using TicTacToe.Mechanics.Base;
using TicTacToe.Players;
using UnityEngine;

namespace TicTacToe.Services.Saving
{
    public class GameSaveProvider_Json_Full : IGameSaveProvider
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        private readonly Action<IGameSaveProvider, string> _save;

        private readonly Data _data;

        public EGameMode GameMode => _data.GameMode;
        public EPersonType PersonType => _data.PersonType;
        public ABoardModel BoardModel => _data.BoardModel;
        public ETttType YourSign => _data.YourSign;

        public GameSaveProvider_Json_Full(EGameMode gameMode, EPersonType personType, ABoardModel boardModel,
            ETttType yourSign, Action<IGameSaveProvider, string> save)
        {
            if (save == null)
            {
                throw new ArgumentNullException(nameof(save));
            }

            if (boardModel == null)
            {
                throw new ArgumentNullException(nameof(boardModel));
            }

            _save = save;
            _data = new Data(gameMode, personType, boardModel, yourSign);
            _data.BoardModel.TurnChanged += Save;
            _data.BoardModel.GameOver += BoardModelOnGameOver;
        }

        public GameSaveProvider_Json_Full(string json, Action<IGameSaveProvider, string> save)
        {
            if (save == null)
            {
                throw new ArgumentNullException(nameof(save));
            }

            _save = save;
            _data = JsonConvert.DeserializeObject<Data>(json, JsonSerializerSettings);
            _data.BoardModel.TurnChanged += Save;
            _data.BoardModel.GameOver += BoardModelOnGameOver;
        }

        private void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.None, JsonSerializerSettings);
            _save(this, json);
        }

#if UNITY_EDITOR
        public void ForceSave()
        {
            Save();
        }
#endif

        private void BoardModelOnGameOver(ETttType winner)
        {
            _data.BoardModel.TurnChanged -= Save;
            _data.BoardModel.GameOver -= BoardModelOnGameOver;
        }

        private readonly struct Data
        {
            public readonly EGameMode GameMode;
            public readonly EPersonType PersonType;
            public readonly ABoardModel BoardModel;
            public readonly ETttType YourSign;

            public Data(EGameMode gameMode, EPersonType personType, ABoardModel boardModel, ETttType yourSign)
            {
                GameMode = gameMode;
                PersonType = personType;
                BoardModel = boardModel;
                YourSign = yourSign;
            }
        }
    }
}