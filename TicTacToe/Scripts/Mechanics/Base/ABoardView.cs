using TicTacToe.InteractiveTutorial;
using UnityEngine;

namespace TicTacToe.Mechanics.Base
{
    public abstract class ABoardView : MonoBehaviour
    {
        public abstract bool Interactable { get; set; }

        public abstract void Initialize(ABoardModel model);
        public abstract void Uninitialize();
        public abstract void Show();
        public abstract void Close();
        public virtual void SetInteractiveTutorial(AInteractiveTutorial interactiveTutorial){}
    }
}