using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.InteractiveTutorial
{
    public abstract class AInteractiveTutorial : MonoBehaviour
    {
        public event Action StartTutorial;
        protected bool _IsInitialized;
        public virtual void Initialize()
        {
            _IsInitialized = true;
        }

        public void ShowTutorial()
        {
            StartTutorial?.Invoke();
        }

        public abstract void TutorialHide();

    }
}