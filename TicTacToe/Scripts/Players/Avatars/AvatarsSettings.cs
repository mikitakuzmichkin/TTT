using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace TicTacToe.Players.Avatars
{
    [CreateAssetMenu(fileName = "AvatarsSettings", menuName = "TicTacToe/Avatars Settings", order = 0)]
    public class AvatarsSettings : ScriptableObject
    {
#pragma warning disable 649
        [SerializeField] private AvatarsDictionary _avatarsDictionary;
#pragma warning restore 649

        public IDictionary<AvatarType, Sprite> Avatars => _avatarsDictionary;

        [Serializable]
        private class AvatarsDictionary : SerializableDictionaryBase<AvatarType, Sprite>
        {
        }
    }
}