using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace TicTacToe.UI.Windows.OptionsWindow.SubWindows.Tutorial
{
    [CreateAssetMenu(menuName = "TicTacToe/Tutorial Content Storage", fileName = "TutorialContentStorage")]
    public class TutorialContentStorage : ScriptableObject
    {
        [SerializeField] private ModeTutorialsDict _modeTutorials;

        private IDictionary<TutorialType, RectTransform[]> _modeTutorialsProcessed;

        public IDictionary<TutorialType, RectTransform[]> ModeTutorials
        {
            get
            {
                if (_modeTutorialsProcessed == null)
                {
                    _modeTutorialsProcessed = new Dictionary<TutorialType, RectTransform[]>();
                    foreach (var type in _modeTutorials.Keys)
                    {
                        _modeTutorialsProcessed[type] = _modeTutorials[type].Slides;
                    }
                }

                return _modeTutorialsProcessed;
            }
        }

        [Serializable]
        private class ModeTutorialsDict : SerializableDictionaryBase<TutorialType, TutorialSlides>
        {
        }

        [Serializable]
        private class TutorialSlides
        {
            public RectTransform[] Slides;
        }
    }
}