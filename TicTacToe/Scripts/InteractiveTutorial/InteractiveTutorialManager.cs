using System;
using DG.Tweening;
using TicTacToe.Mechanics.Base;
using TicTacToe.Players;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.InteractiveTutorial
{
    public class InteractiveTutorialManager : MonoBehaviour
    {
        private const float ANIM_DURATION = 0.5f;
        
        [SerializeField] private Button _closeTutor;
        [SerializeField] private Graphic _raycaster;
        [SerializeField] private RectTransform _textPanel;
        [SerializeField] private InteractiveTutorialForClassic _interactiveTutorialForClassic;
        private Action DestroyAction;
        private AInteractiveTutorial _activeTutor;
        private float _startTextPanelY;

        private void Awake()
        {
            _raycaster.enabled = false;
            _startTextPanelY = _textPanel.anchoredPosition.y;
            _textPanel.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            DestroyAction?.Invoke();
            _closeTutor.onClick.RemoveAllListeners();
        }

        public void Initialize(GameplayData gameplayData, ABoardView view, APlayerController player,
            APlayerController enemy)
        {
            _closeTutor.onClick.AddListener(TutorialHide);
            
            switch (gameplayData.Mode)
            {
                case EGameMode.Classic:
                    _activeTutor = _interactiveTutorialForClassic;
                    view.SetInteractiveTutorial(_interactiveTutorialForClassic);
                    player.StartTurn += _interactiveTutorialForClassic.MarkAllEmptyCells;
                    if (gameplayData.PersonType == EPersonType.Human)
                    {
                        enemy.StartTurn += _interactiveTutorialForClassic.MarkAllEmptyCells;
                        _interactiveTutorialForClassic.For2Players();
                        DestroyAction += () =>
                        {
                            enemy.StartTurn -= _interactiveTutorialForClassic.MarkAllEmptyCells;
                        };
                    }

                    _interactiveTutorialForClassic.StartTutorial += ShowTutorial;

                    DestroyAction += () =>
                    {
                        player.StartTurn -= _interactiveTutorialForClassic.MarkAllEmptyCells;
                        _interactiveTutorialForClassic.StartTutorial -= ShowTutorial;
                    };
                    break;
                case EGameMode.Alternative:
                    break;
                case EGameMode.Strategy:
                    break;
            }
        }
        
        public void ShowTutorial()
        {
            _raycaster.enabled = true;
            var pos = _textPanel.anchoredPosition;
            pos.y = 0;
            _textPanel.anchoredPosition = pos;
            _textPanel.gameObject.SetActive(true);
            DOTween.Sequence().Append(_textPanel.DOAnchorPosY(_startTextPanelY, ANIM_DURATION)
                .SetEase(Ease.OutQuart));
        }

        public virtual void TutorialHide()
        {
            _activeTutor.TutorialHide();
            _textPanel.gameObject.SetActive(false);
            _raycaster.enabled = false;
        }
        
       
    }
}