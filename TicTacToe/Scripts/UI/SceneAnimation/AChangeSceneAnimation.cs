using System;
using UnityEngine;

namespace TicTacToe.UI.SceneAnimation
{
    public abstract class AChangeSceneAnimation : MonoBehaviour
    {
        public abstract void PlayAfterSceneChanged();
        public abstract void PlayBeforeSceneChanged(Action callback);
    }
}