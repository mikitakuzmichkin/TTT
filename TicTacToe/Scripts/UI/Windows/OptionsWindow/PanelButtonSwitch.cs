using System;
using UnityEngine;

namespace TicTacToe.UI.Windows.OptionsWindow
{
    public class PanelButtonSwitch : PanelButton
    {
        [SerializeField] private SwitchUI _switch;

        public bool IsOn
        {
            get => _switch.IsOn;
            set => _switch.IsOn = value;
        }

        public event Action<bool> Changed;

        protected override void Awake()
        {
            base.Awake();
            Button.onClick.AddListener(() =>
            {
                var newValue = !_switch.IsOn;
                _switch.IsOn = newValue;
                Changed?.Invoke(newValue);
            });
        }

        public override void RemoveAllListeners()
        {
            base.RemoveAllListeners();
            Changed = null;
        }
    }
}