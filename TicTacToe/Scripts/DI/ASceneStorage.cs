using UnityEngine;

namespace TicTacToe.DI
{
    public abstract class ASceneStorage : MonoBehaviour
    {
        private void Awake()
        {
            Initialize();
        }

        protected abstract void Initialize();
    }
}