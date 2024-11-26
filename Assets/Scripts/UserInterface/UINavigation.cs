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
        public bool RecepieOpened;

        private bool _lastWin;
        [SerializeField] private TextMeshProUGUI _gameOverText;
        
        [SerializeField] private List<Button> _navigationMainButtons;
        [SerializeField] private List<Image> _navigationHelpImages;

        [SerializeField] private Image _bgOverImage;
        [SerializeField] private List<Sprite> _gameOverSprites;
        [SerializeField] private Button _recepieButton;
        
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
        }
        
        private void SetNavigationImages(int index)
        {
            for (int i = 0; i < _navigationMainButtons.Count; i++)
            {
                _navigationMainButtons[i].image.color = i == index ? new Color(_navigationMainButtons[i].image.color.r,
                                                                        _navigationMainButtons[i].image.color.g,
                                                                        _navigationMainButtons[i].image.color.b, 0f)
                                                                        : new Color(_navigationMainButtons[i].image.color.r,
                                                                        _navigationMainButtons[i].image.color.g,
                                                                        _navigationMainButtons[i].image.color.b, 1f);
            }

            for (int i = 0; i < _navigationHelpImages.Count; i++)
            {
                _navigationHelpImages[i].color = i == index ? new Color(_navigationHelpImages[i].color.r,
                                                                        _navigationHelpImages[i].color.g, _navigationHelpImages[i].color.b, 1f)
                                                                        : new Color(_navigationHelpImages[i].color.r, _navigationHelpImages[i].color.g,
                                                                        _navigationHelpImages[i].color.b, 0f);
            }

            foreach (var button in _navigationMainButtons)
            {
                button.interactable = false;
            }
        }


        private void ResetMenuPopups()
        {
            foreach (var popup in MainMenuPopups.Where(popup => popup.transform.localRotation != Quaternion.Euler(new Vector3(0f,90f,0f))))
                StartCoroutine(AnimateRotation(popup, false));
        }

        private void ResetOnStart()
        {
            foreach (var popup in MainMenuPopups) popup.transform.localRotation = Quaternion.Euler(new Vector3(0f,90f,0f));
        }

        private void ResetPopups()
        {
            OpenGroup(LoadingMenu);

            CloseGroup(GameMenu);
            CloseGroup(MainMenu);

            ResetGamePopups();
            ResetOnStart();
        }

        public void OpenMainMenu()
        {
            _isInLoad = false;
            StartCoroutine(OpenMenuPopup(0, true, false, true));
            SetNavigationImages(0);
        }

        public void CloseGameUI()
        {
            gameClose = true;
            GameInstance.NovelController.CurrentSceneIndex = 0;
            StartCoroutine(OpenMenuPopup(0, true, _lastWin, true));
            SetNavigationImages(0);
        }

        public void BackToMainMenu()
        {
            GameInstance.Audio.Play(GameInstance.Audio.BookSound);
            StartCoroutine(OpenMenuPopup(0));
            SetNavigationImages(0);
        }

        public void OpenAlchemistUI()
        {
            GameInstance.Audio.Play(GameInstance.Audio.BookSound);
            StartCoroutine(OpenMenuPopup(1, false));
            SetNavigationImages(1);
        }

        public void OpenSettingsUI()
        {
            GameInstance.Audio.Play(GameInstance.Audio.BookSound);
            StartCoroutine(OpenMenuPopup(2, false));
            SetNavigationImages(2);
        }
        
        public void OpenGameMenu()
        {
            StartCoroutine(OpenGamePopup());
        }
        
        public void OpenGameOverPopup(bool isLast = false)
        {
            _bgOverImage.sprite = _gameOverSprites[GameInstance.NovelController.CurrentChapterIndex];
            ResetGamePopups();
            IsInGame = false;
            StartCoroutine(FadeCanvasGroup(GamePopups[0], true));
            _lastWin = true;
            GameInstance.Audio.Play(GameInstance.Audio.GameOverSounds);
            GameInstance.FXController.PlayFireworksParticle();

            _gameOverText.text = isLast ? "Congratulations! You've passed the Volcano of Destiny!" : "Keep it up ! This award will help you in your adventures!";
        }

        public void OpenScrollPopup()
        {
            StartCoroutine(FadeCanvasGroup(GamePopups[1], true));
        }

        public void CloseScrollPopup()
        {
            StartCoroutine(FadeCanvasGroup(GamePopups[1], false));
        }
        
        public void OpenRecepiePopup()
        {
            if(RecepieOpened) return;
            GameInstance.Audio.Play(GameInstance.Audio.BookSound);
            RecepieOpened = true;
            StartCoroutine(FadeCanvasGroup(GamePopups[2], true));
        }

        public void CloseRecepiePopup()
        {
            if(!RecepieOpened) return;
            GameInstance.Audio.Play(GameInstance.Audio.BookSound);
            RecepieOpened = false;
            StartCoroutine(FadeCanvasGroup(GamePopups[2], false));
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

            if (needAward)
            {
                GameInstance.MoneyManager.AddCoinsCurrency(500);
                _lastWin = false;
            }

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
            ResetMenuPopups();

            StartCoroutine(AnimateRotation(MainMenuPopups[index], true));
        }

        private IEnumerator OpenGamePopup(bool isRestart = false)
        {
            yield return new WaitForSeconds(0.3f);
            if (MainMenuPopups[0].transform.localScale == Vector3.one)
            {
                StartCoroutine(AnimateRotation(MainMenuPopups[0], false));
            }
            
            yield return new WaitForSeconds(0.7f);
            TransitionAnimation();
            yield return new WaitForSeconds(0.5f);
            GameInstance.MusicSystem.ChangeMusicClip(false);
            CheckRecepieGot();
            IsInGame = true;
            GameInstance.FXController.StopFireworksParticle();
            GameInstance.FXController.PlayGameBackgroundParticle();
            GameInstance.FXController.StopMenuBackgroundParticle();
            ResetGamePopups();
            OpenGroup(GameMenu);
            CloseGroup(LoadingMenu);
            CloseGroup(MainMenu);
            if (isRestart)
                OnGameRestarted.Invoke();
            else
                OnGameStarted?.Invoke();
        }

        public void CheckRecepieGot()
        {
            _recepieButton.interactable = PlayerPrefs.GetInt("RecepieGot", 0) == 1;
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

        public IEnumerator AnimateRotation(CanvasGroup canvasGroup, bool show, float duration = 0.7f, bool needWait = true)
        {
            foreach (var button in _navigationMainButtons)
            {
                button.interactable = false;
            } 
            
            if (show && needWait)
            {
                yield return new WaitForSeconds(0.9f);
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;

            var startRotation = show ? Quaternion.Euler(0f, 90f, 0f) : Quaternion.Euler(0f, 0f, 0f);
            var endRotation = show ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 90f, 0f);

            var elapsedTime = 0f;

            canvasGroup.transform.localRotation = startRotation;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                canvasGroup.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);

                yield return null;
            }

            canvasGroup.transform.localRotation = endRotation;

            if (show)
            {
                foreach (var button in _navigationMainButtons)
                {
                    button.interactable = true;
                }
            }
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

        private void SelectMenuPopup(int selectedIndex)
        {
            OpenGroup(MainMenu);
            CloseGroup(GameMenu);

            for (var i = 0; i < MainMenuPopups.Count; i++)
                if (i == selectedIndex)
                {
                    MainMenuPopups[i].alpha = 1f;
                    MainMenuPopups[i].blocksRaycasts = true;
                    MainMenuPopups[i].interactable = true;
                }
                else
                {
                    MainMenuPopups[i].alpha = 0f;
                    MainMenuPopups[i].blocksRaycasts = false;
                    MainMenuPopups[i].interactable = false;
                }

            foreach (var popup in GamePopups)
            {
                popup.alpha = 0f;
                popup.blocksRaycasts = false;
                popup.interactable = false;
            }

            OnActivePopupChanged?.Invoke();
        }

        private void SelectGamePopup(int selectedIndex)
        {
            OpenGroup(GameMenu);
            CloseGroup(MainMenu);

            for (var i = 0; i < GamePopups.Count; i++)
                if (i == selectedIndex)
                {
                    GamePopups[i].alpha = 1f;
                    GamePopups[i].blocksRaycasts = true;
                    GamePopups[i].interactable = true;
                }
                else
                {
                    GamePopups[i].alpha = 0f;
                    GamePopups[i].blocksRaycasts = false;
                    GamePopups[i].interactable = false;
                }

            foreach (var popup in MainMenuPopups)
            {
                popup.alpha = 0f;
                popup.blocksRaycasts = false;
                popup.interactable = false;
            }

            OnActivePopupChanged?.Invoke();
        }
    }
}