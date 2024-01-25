using System;
using TicTacToe.DI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TicTacToe.Sound
{
    public class SoundButton : Button
    {
        [Serializable]
        public class ButtonPressedEvent : UnityEvent
        {
        }

        [SerializeField] private ButtonPressedEvent m_OnPress = new ButtonPressedEvent();
        [SerializeField] private GameSound m_Sound;

        public ButtonPressedEvent OnPress
        {
            get => m_OnPress;
            set => m_OnPress = value;
        }

        public GameSound Sound
        {
            get => m_Sound;
            set => m_Sound = value;
        }

        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            var soundManager = ProjectContext.GetInstance<ISoundManager>();
            m_OnPress.AddListener(() => soundManager.Sound(Sound));
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (eventData.button == PointerEventData.InputButton.Left && IsActive() && IsInteractable())
            {
                m_OnPress.Invoke();
            }
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SoundButton), true)]
    [UnityEditor.CanEditMultipleObjects]
    public class SoundButtonEditor : UnityEditor.UI.ButtonEditor
    {
        private UnityEditor.SerializedProperty _onPressProperty, _soundProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            _onPressProperty = serializedObject.FindProperty("m_OnPress");
            _soundProperty = serializedObject.FindProperty("m_Sound");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UnityEditor.EditorGUILayout.Space();

            serializedObject.Update();
            UnityEditor.EditorGUILayout.PropertyField(_onPressProperty);
            UnityEditor.EditorGUILayout.PropertyField(_soundProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}