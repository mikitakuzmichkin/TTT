#if UNITY_EDITOR || DEV
using System;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
public class WinLoseDrawButtonsPanel : MonoBehaviour
{
    [SerializeField] private Button _winButton;
    [SerializeField] private Button _loseButton;
    [SerializeField] private Button _drawButton;

    public void Subscribe(Action<CheatAction> action)
    {
        _winButton.onClick.AddListener(() => action?.Invoke(CheatAction.Win));
        _loseButton.onClick.AddListener(() => action?.Invoke(CheatAction.Lose));
        _drawButton.onClick.AddListener(() => action?.Invoke(CheatAction.Draw));
    }

    public void UnsubscribeAll()
    {
        _winButton.onClick.RemoveAllListeners();
        _loseButton.onClick.RemoveAllListeners();
        _drawButton.onClick.RemoveAllListeners();
    }
}
#endif