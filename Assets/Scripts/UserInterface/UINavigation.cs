using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class UINavigation : MonoBehaviour
    {
        public List<CanvasGroup> GamePopups;
        public List<CanvasGroup> MainMenuPopups;
        [SerializeField] private Animator _transitionAnimator;

        public CanvasGroup LoadingMenu;
        public CanvasGroup MainMenu;
        public CanvasGroup GameMenu;

        public Action OnActivePopupChanged;
        public Action OnGameStarted;
        public Action OnGameRestarted;
        public Action OnGameWindowClosed;

        public bool _isInLoad;
        public bool _toMain;
        public bool IsInGame = false;
        private bool gameClose;

        private bool _lastWin;
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private TextMeshProUGUI _rewardText;
        
        [SerializeField] private Image _bgOverImage;
        [SerializeField] private List<Sprite> _gameOverSprites;

        public void Init()
        {
            ResetPopups();
        }

        private void ResetGamePopups()
        {
            foreach (var popup in GamePopups)
            {
                popup.alpha = 0f;
                popup.blocksRaycasts = false;
                popup.interactable = false;
            }
            
            foreach (var popup in MainMenuPopups)
            {
                popup.alpha = 0f;
                popup.blocksRaycasts = false;
                popup.interactable = false;
            }
        }
        
        private void ResetPopups()
        {
            OpenGroup(LoadingMenu);

            CloseGroup(GameMenu);
            CloseGroup(MainMenu);

            ResetGamePopups();
        }

        public void OpenMainMenu()
        {
            _isInLoad = false;
            StartCoroutine(OpenMenuPopup(0, true, false, true));
        }

        public void CloseGameUI()
        {
            gameClose = true;
            GameInstance.NovelController.CurrentSceneIndex = 0;
            StartCoroutine(OpenMenuPopup(0, true, _lastWin, true));
        }
        
        public void OpenGameMenu()
        {
            StartCoroutine(OpenGamePopup());
        }

        public void OpenGameOverPopup(string gameOverText, string rewardText)
        {
            ResetGamePopups();
            IsInGame = false;
            StartCoroutine(FadeCanvasGroup(GamePopups[0], true));
            _lastWin = true;
            GameInstance.Audio.Play(GameInstance.Audio.GameOverSounds);
            GameInstance.FXController.PlayFireworksParticle();

            _gameOverText.text = gameOverText;
            _rewardText.text = rewardText;
        }
        
        public IEnumerator OpenMenuPopup(int index, bool toMenu = true, bool needAward = false, bool afterLoad = false)
        {
            if (afterLoad)
            {
                TransitionAnimation();
                yield return new WaitForSeconds(0.5f);
            }

            GameInstance.FXController.PlayMenuBackgroundParticle();
            if (toMenu)
            {
                GameInstance.MusicSystem.ChangeMusicClip();
            }

            GameInstance.FXController.StopFireworksParticle();

            if (gameClose)
            {
                gameClose = false;
                ResetGamePopups();
                OnGameWindowClosed?.Invoke();
                GameInstance.FXController.StopGameBackgroundParticle();
            }

            OpenGroup(MainMenu);
            CloseGroup(GameMenu);
            CloseGroup(LoadingMenu);
            
            yield return new WaitForSeconds(0.5f);
            
            if (needAward)
            {
                switch (GameInstance.NovelController.CurrentChapterIndex)
                {
                    case 0:
                        GameInstance.MoneyManager.AddCoinsCurrency(500);
                        break;
                    case 1:
                        GameInstance.MoneyManager.AddCoinsCurrency(1500);
                        break;
                }

                _lastWin = false;
            }
        }

        private IEnumerator OpenGamePopup()
        {
            TransitionAnimation();
            yield return new WaitForSeconds(0.5f);
            GameInstance.MusicSystem.ChangeMusicClip(false);
            IsInGame = true;
            GameInstance.FXController.StopFireworksParticle();
            GameInstance.FXController.PlayGameBackgroundParticle();
            GameInstance.FXController.StopMenuBackgroundParticle();
            ResetGamePopups();
            OpenGroup(GameMenu);
            CloseGroup(LoadingMenu);
            CloseGroup(MainMenu);
            OnGameStarted?.Invoke();
        }
        
        public IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, bool show, float duration = 1f)
        {
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;

            var startAlpha = canvasGroup.alpha;
            var elapsedTime = 0f;

            var finishValue = show ? 1f : 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, finishValue, elapsedTime / duration);
                yield return null;
            }

            canvasGroup.alpha = finishValue;
        }
        
        public void TransitionAnimation()
        {
            _transitionAnimator.SetTrigger("Transition");
        }

        public void OpenGroup(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        public void CloseGroup(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}