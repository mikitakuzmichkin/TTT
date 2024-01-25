using System;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI.Windows.OptionsWindow
{
    [RequireComponent(typeof(Button))]
    public class PanelButton : MonoBehaviour
    {
        protected Button Button;

        public event Action Click;

        protected virtual void Awake()
        {
            Button = transform.GetComponent<Button>();
            Button.onClick.AddListener(() => Click?.Invoke());
        }

        public virtual void RemoveAllListeners()
        {
            Click = null;
        }
    }
}