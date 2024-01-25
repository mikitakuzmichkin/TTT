using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace TicTacToe.Sound
{
    [CreateAssetMenu(menuName = "TicTacToe/Sound/Sounds", fileName = "GameSounds", order = 0)]
    public class GameSounds : ScriptableObject
    {
        [SerializeField] private SoundsDict _sounds;

        public IDictionary<GameSound, AudioClip> Sounds => _sounds;

        [Serializable]
        private class SoundsDict : SerializableDictionaryBase<GameSound, AudioClip>
        {
        }
    }
}