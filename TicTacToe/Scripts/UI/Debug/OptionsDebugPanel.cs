#if UNITY_EDITOR || DEV
using System;
using TicTacToe.UI.Windows.OptionsWindow;
using UnityEngine;

public class OptionsDebugPanel : MonoBehaviour
{
    [SerializeField] private PanelButtonSwitch _adsEnabledSwitch;
    [SerializeField] private PanelButtonSwitch _monitorEnabledSwitch;

    public bool AdsEnabledSwitchState
    {
        get => _adsEnabledSwitch.IsOn;
        set => _adsEnabledSwitch.IsOn = value;
    }

    public bool MonitorEnabledSwitchState
    {
        get => _monitorEnabledSwitch.IsOn;
        set => _monitorEnabledSwitch.IsOn = value;
    }

    public event Action<bool> AdsEnabledSwitchChanged
    {
        add => _adsEnabledSwitch.Changed += value;
        remove => _adsEnabledSwitch.Changed -= value;
    }

    public event Action<bool> MonitorEnabledSwitchChanged
    {
        add => _monitorEnabledSwitch.Changed += value;
        remove => _monitorEnabledSwitch.Changed -= value;
    }

    public void Unsubscribe()
    {
        _adsEnabledSwitch.RemoveAllListeners();
        _monitorEnabledSwitch.RemoveAllListeners();
    }
}
#endif