using System;
using UnityEngine;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.Rules
{
    public class RulesSubWindowView : ASubWindowView
    {
        [SerializeField] private PanelButton _commonRules;
        [SerializeField] private PanelButton _classicRules;
        [SerializeField] private PanelButton _alternativeRules;
        [SerializeField] private PanelButton _strategyRules;

        public event Action ToAlternativeRules;
        public event Action ToClassicRules;
        public event Action ToStrategyRules;
        public event Action ToCommonRules;

        private void Start()
        {
            _commonRules.Click += () => ToCommonRules?.Invoke();
            _classicRules.Click += () => ToClassicRules?.Invoke();
            _alternativeRules.Click += () => ToAlternativeRules?.Invoke();
            _strategyRules.Click += () => ToStrategyRules?.Invoke();
        }
    }
}