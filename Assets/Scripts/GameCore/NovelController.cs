using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameCore
{
    public class NovelController : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _manImage;
        [SerializeField] private GameObject[] _textPanels;
        [SerializeField] private Button _nextSlideButton;
        [SerializeField] private TextMeshProUGUI[] _panelTexts;
        [SerializeField] private float _typingSpeed = 0.05f;
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private List<Sprite> _1ChapterSprites;
        [SerializeField] private List<Sprite> _2ChapterSprites;
        [SerializeField] private List<Sprite> _charactersImages;
        public AudioCueScriptableObject _voice;
        [SerializeField] private List<AudioClip> _clips;
        [SerializeField] private List<AudioClip> _clips2Chapter;
        
        public int CurrentSceneIndex;
        public int CurrentChapterIndex = 0;
        private bool _isTextAnimating;

        private void SetClip()
        {
            if (CurrentChapterIndex == 0 && CurrentSceneIndex < _clips.Count)
            {
                _voice.Options.clip = _clips[CurrentSceneIndex];
            }
            else if (CurrentChapterIndex == 1 && CurrentSceneIndex < _clips2Chapter.Count)
            {
                _voice.Options.clip = _clips2Chapter[CurrentSceneIndex];
            }
        }

        private void SetClipPrevious()
        {
            if (CurrentChapterIndex == 1 && CurrentSceneIndex <= _clips2Chapter.Count)
            {
                _voice.Options.clip = _clips2Chapter[CurrentSceneIndex - 1];
            }
        }
        
        public void Init()
        {
            _nextSlideButton.onClick.AddListener(GoToNextScene);
            GameInstance.UINavigation.OnGameStarted += ShowCurrentScene;
        }

        private void OnDestroy()
        {
            GameInstance.UINavigation.OnGameStarted -= ShowCurrentScene;
        }

        public IEnumerator TypeText(string text, TextMeshProUGUI textComponent)
        {
            textComponent.text = ""; 
            _isTextAnimating = true;

            foreach (var letter in text)
            {
                textComponent.text += letter; 
                yield return new WaitForSeconds(_typingSpeed); 
            }

            _isTextAnimating = false; 
            _nextSlideButton.gameObject.SetActive(true); 
        }

        public void ChangeBackground(Sprite newBackground)
        {
            if (newBackground != _backgroundImage.sprite)
            {
                _backgroundImage.sprite = newBackground;
            }
        }

        public IEnumerator ShowPanelWithText(int panelIndex, string text, string authorName = null)
        {
            var panel = _textPanels[panelIndex];
            var panelText = _panelTexts[panelIndex];
            
            yield return StartCoroutine(FadeIn(panel));
            StartCoroutine(TypeText(text, panelText));
        }
        
        public IEnumerator FadeIn(GameObject uiElement)
        {
            var canvasGroup = uiElement.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = uiElement.AddComponent<CanvasGroup>();

            uiElement.SetActive(true);
            canvasGroup.alpha = 0f;
            uiElement.transform.localScale = Vector3.zero;  

            var timer = 0f;
            var maxScale = 1.2f;  

            while (timer < _fadeDuration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / _fadeDuration);
                uiElement.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * maxScale, timer / _fadeDuration);  
                yield return null;
            }

            timer = 0f;
            while (timer < 0.2f)  
            {
                timer += Time.deltaTime;
                uiElement.transform.localScale = Vector3.Lerp(Vector3.one * maxScale, Vector3.one, timer / 0.2f);  
                yield return null;
            }

            canvasGroup.alpha = 1f;
            uiElement.transform.localScale = Vector3.one;
        }


        public IEnumerator AnimateCharacter(int index)
        {
            _manImage.gameObject.SetActive(true);
            _manImage.transform.localScale = Vector3.zero;
            var spriteCharacter = _charactersImages[index];
            _manImage.sprite = spriteCharacter;

            var timer = 0f;
            while (timer < _fadeDuration)
            {
                timer += Time.deltaTime;
                _manImage.transform.localScale =
                    Vector3.Lerp(Vector3.zero, Vector3.one, timer / _fadeDuration); 
                yield return null;
            }

            _manImage.transform.localScale = Vector3.one; 
        }

        private void GoToNextScene()
        {
            if (_isTextAnimating) return;
            CurrentSceneIndex++;
            StartCoroutine(TransitionToNextScene());
        }

        private IEnumerator TransitionToNextScene()
        {
            GameInstance.UINavigation.TransitionAnimation();

            yield return new WaitForSeconds(0.5f);
            ClearText();
            ShowCurrentScene();

            yield return new WaitForSeconds(0.5f);
        }

        private void HideAllUIElements()
        {
            foreach (var panel in _textPanels) panel.SetActive(false);
            
            _nextSlideButton.gameObject.SetActive(false);
            _manImage.gameObject.SetActive(false);
        }

        private void ShowCurrentScene()
        {
            HideAllUIElements();
            SetClip();
            switch (CurrentChapterIndex)
            {
                case 0: 
                    ShowChapter1Scenes();
                    break;
                case 1: 
                    ShowChapter2Scenes();
                    break;
            }
        }
        
        private void ShowChapter1Scenes()
        {
            if (CurrentSceneIndex <= 21)
            {
                GameInstance.Audio.Play(_voice);
            }
            
            switch (CurrentSceneIndex)
            {
                case 0:
                    StartCoroutine(ShowPanelWithText(0, "This story is set in the dark city of “LasPegas”. This city is not famous for its honest inhabitants, for all have been swallowed up by one disease, playing cards"));
                    ChangeBackground(_1ChapterSprites[0]);
                    break;
                case 1:
                    StartCoroutine(ShowPanelWithText(0, "But our story is not about the city itself, which is rotten from the inside. It's about the great gambler Victor."));
                    ChangeBackground(_1ChapterSprites[0]);
                    break;
                case 2:
                    StartCoroutine(ShowPanelWithText(0, "He is a talented poker player with a difficult fate. After winning a regional tournament, his career is abruptly cut short by suspicions of fraud. Having lost his reputation, Victor is forced to go into hiding and play only in underground arenas. "));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[5]);
                    break;
                case 3:
                    StartCoroutine(ShowPanelWithText(0, "One gloomy day Victor received an anonymous letter saying that a closed tournament was being held, and of course it was clear what kind of tournament it was"));
                    ChangeBackground(_1ChapterSprites[1]);
                    break;
                case 4:
                    StartCoroutine(ShowPanelWithText(0, "Victor understood perfectly well that the participants were the elite of the criminal world and professionals ready to do anything to win. The prize is not only a huge amount of money, but also influence in the whole city."));
                    ChangeBackground(_1ChapterSprites[2]);
                    break;
                case 5:
                    StartCoroutine(ShowPanelWithText(0, "Victor decided that he needed to participate in this tournament. But he had doubts, so he called his brother Sam and asked him to come to discuss important issues"));
                    ChangeBackground(_1ChapterSprites[5]);
                    break;
                case 6:
                    StartCoroutine(ShowPanelWithText(1, "Thank you for coming so quickly Sam"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 7:
                    StartCoroutine(ShowPanelWithText(1, "Good to see you Victor, tell me what's so urgent that I had to postpone work."));
                    StartCoroutine(AnimateCharacter(2));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 8:
                    StartCoroutine(ShowPanelWithText(1, "I think you've heard there's a private tournament in town, so I thought I'd show up and do the honors."));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 9:
                    StartCoroutine(ShowPanelWithText(1, "I'm not sure that's a great idea, Victor. After all, it's a bunch of con professionals out there. You think you can win them over?"));
                    StartCoroutine(AnimateCharacter(2));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 10:
                    StartCoroutine(ShowPanelWithText(1, "I don't have a choice, brother. Look where I live, how I live. My honor has been sullied. I've always been an honorable player and I want to prove it to everyone and get my good name back."));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 11:
                    StartCoroutine(ShowPanelWithText(1, "Well, if that's what you decide. I have a very good friend named Sheryl. I think she can help you get in there."));
                    StartCoroutine(AnimateCharacter(2));
                    ChangeBackground(_1ChapterSprites[3]);    
                    break;
                case 12:
                    StartCoroutine(ShowPanelWithText(1, "Sheryl, is that the redheaded girl who works at the bar? I didn't know she had that kind of connection."));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 13:
                    StartCoroutine(ShowPanelWithText(1, "Trust me, there's a lot you don't know about her. You shouldn't judge her by her looks. She's not called the queen of hearts because she broke a lot of men's hearts. It's for other reasons."));
                    StartCoroutine(AnimateCharacter(2));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 14:
                    StartCoroutine(ShowPanelWithText(0, "Sam was able to arrange for Sheryl and Victor to meet at a bar that hardly anyone goes to."));
                    ChangeBackground(_1ChapterSprites[5]);
                    break;
                case 15:
                    StartCoroutine(ShowPanelWithText(1, "Sheryl, I'm so glad you could make it. Meet Victor, my brother."));
                    StartCoroutine(AnimateCharacter(2));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 16:
                    StartCoroutine(ShowPanelWithText(1, "Oh, Victor. I've heard a lot about you. What can I do for a man like you?"));
                    StartCoroutine(AnimateCharacter(1));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 17:
                    StartCoroutine(ShowPanelWithText(1, "Hello, I'd like to enter the private tournament."));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 18:
                    StartCoroutine(ShowPanelWithText(1, "Not so loud. Why are you going in there? Isn't life nice without adrenaline?"));
                    StartCoroutine(AnimateCharacter(1));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 19:
                    StartCoroutine(ShowPanelWithText(1, "I want to live up to my name, it's very important to me. I'm tired of living and hiding from everyone. I'm an honest man and even more so an honest player."));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 20:
                    StartCoroutine(ShowPanelWithText(1, "Sheryl, please help him get into this tournament. I'll do anything you want for the favor."));
                    StartCoroutine(AnimateCharacter(2));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 21:
                    StartCoroutine(ShowPanelWithText(1, "Well, I'll take you there. But mind you, I'm not doing this because I care about your name, only out of sympathy for your brother."));
                    StartCoroutine(AnimateCharacter(1));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 22:
                    GameInstance.UINavigation.OpenGameOverPopup("The End chapter 1", "+500");
                    break;
                default:
                    StartCoroutine(ShowPanelWithText(0, "kinec"));
                    break;
            }
        }

        private void ShowChapter2Scenes()
        {
            if (CurrentSceneIndex <= 15)
            {
                GameInstance.Audio.Play(_voice);
            }
            else if (CurrentSceneIndex >= 17 && CurrentSceneIndex <= 23)
            {
                SetClipPrevious();
                GameInstance.Audio.Play(_voice);
            }
            
            switch (CurrentSceneIndex)
            {
                case 0:
                    StartCoroutine(ShowPanelWithText(0, "Sheryl, kept her word and was able to get Victor into the private tournament."));
                    ChangeBackground(_1ChapterSprites[5]);
                    break;
                case 1:
                    StartCoroutine(ShowPanelWithText(0, "Going inside Victor felt old memories of this place, he remembers every table every machine, all this reminds him of the happy days when he was not considered a scammer"));
                    ChangeBackground(_2ChapterSprites[0]);
                    break;
                case 2:
                    StartCoroutine(ShowPanelWithText(1, "Oh so many fond and unpleasant memories of this place."));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_2ChapterSprites[0]);
                    break;
                case 3:
                    StartCoroutine(ShowPanelWithText(1, "Don't get all snotty, Victor! We didn't come here to reminisce."));
                    StartCoroutine(AnimateCharacter(1));
                    ChangeBackground(_2ChapterSprites[0]);
                    break;
                case 4:
                    StartCoroutine(ShowPanelWithText(1, "I'm sorry, Sheryl, I'm a little surprised it hasn't changed in so many years."));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_2ChapterSprites[0]);
                    break;
                case 5:
                    StartCoroutine(ShowPanelWithText(1, "Shit, here comes the Maestro. If we haven't met Victor."));
                    StartCoroutine(AnimateCharacter(1));
                    ChangeBackground(_2ChapterSprites[0]);
                    break;
                case 6:
                    StartCoroutine(ShowPanelWithText(1, "Well, Well, Well,  look who's here. It's Victor Rush himself. What are you doing here, my dear friend?"));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_2ChapterSprites[0]);
                    break;
                case 7:
                    StartCoroutine(ShowPanelWithText(1, "Maestro. I didn't realize you were still here"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_2ChapterSprites[0]);
                    break;
                case 8:
                    StartCoroutine(ShowPanelWithText(1, "I'm always here. I'm still wondering what you're doing here."));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_2ChapterSprites[0]);
                    break;
                case 9:
                    StartCoroutine(ShowPanelWithText(1, "I came to a private tournament to play my honorable name."));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_2ChapterSprites[0]);
                    break;
                case 10:
                    StartCoroutine(ShowPanelWithText(1, "Ha ha ha ha ha ha ha ha ha ha ha, honest to goodness ? Victor. What an honest name. You better get out of here."));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_2ChapterSprites[0]);
                    break;
                case 11:
                    StartCoroutine(ShowPanelWithText(1, "You're laughing. Well, I'm really here to do it. And you're not gonna stop me."));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_2ChapterSprites[0]);
                    break;
                case 12:
                    StartCoroutine(ShowPanelWithText(1, "Ha ha ha, Victor, I'd love to see that. Well, let's go. I'll take you "));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_2ChapterSprites[0]);
                    break;
                case 13:
                    StartCoroutine(ShowPanelWithText(0, "Maestro takes Victor to the hall where they hold a closed tournament, everyone looks around at Victor. This oppressive atmosphere is getting worse. But it's too late to retreat."));
                    ChangeBackground(_1ChapterSprites[5]);
                    break;
                case 14:
                    StartCoroutine(ShowPanelWithText(0, "You need to win a game\nThe main thing is to get win\nto pass. Good luck"));
                    ChangeBackground(_1ChapterSprites[5]);
                    break;
                case 15:
                    StartCoroutine(ShowPanelWithText(0, "Your task is to get rid of all the cards in your hand!"));
                    ChangeBackground(_1ChapterSprites[5]);
                    break;
                case 16:
                    PlayerPrefs.SetInt("IsFromNovell", 1);
                    PlayerPrefs.Save();
                    EnterUnoGame();
                    Debug.Log("Mini Game");
                    break;
                case 17:
                    StartCoroutine(ShowPanelWithText(1, "Well, well, well, Victor, you really are a professional player. Well, everything was fair and I really can't call you a scammer."));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_2ChapterSprites[1]);
                    break;
                case 18:
                    StartCoroutine(ShowPanelWithText(1, "I always play fair Maestro. And I'm glad I was able to clear my name. That's the best reward of all."));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_2ChapterSprites[1]);
                    break;
                case 19:
                    StartCoroutine(ShowPanelWithText(1, "You're also humble, which is nice, so take your winnings and....... your good name."));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_2ChapterSprites[1]);
                    break;
                case 20:
                    StartCoroutine(ShowPanelWithText(1, "Victor, you were absolutely marvelous! At one point I even thought you were cheating, but I followed your every move. And either you're a really good cheater or you're a really professional."));
                    StartCoroutine(AnimateCharacter(1));
                    ChangeBackground(_2ChapterSprites[1]);
                    break;
                case 21:
                    StartCoroutine(ShowPanelWithText(1, "Thanks Sheryl, but I played fair. And here's your share, so to speak, for helping me out! Take care of yourself and..... my brother."));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_2ChapterSprites[1]);
                    break;
                case 22:
                    StartCoroutine(ShowPanelWithText(1, "You're a pleasure to work with. We look forward to seeing you again at our place"));
                    StartCoroutine(AnimateCharacter(1));
                    ChangeBackground(_2ChapterSprites[1]);
                    break;
                case 23:
                    StartCoroutine(ShowPanelWithText(0, "Victor was able to prove to the whole town that he is an honest man and a great player. Now in every establishment he was held up as an example to everyone who entered. His life got better and he gave up these games, created his own family and completely forgot about his old life."));
                    ChangeBackground(_1ChapterSprites[5]);
                    break;
                case 24:
                    GameInstance.UINavigation.OpenGameOverPopup("The End chapter 2", "+1500");
                    break;
                default:
                    StartCoroutine(ShowPanelWithText(0, "Конец второй главы!"));
                    break;
            }
        }
        
        private void ClearText()
        {
            foreach (var text in _panelTexts)
            {
                text.text = "";
            }
        }

        public void EnterUnoGame()
        {
            StartCoroutine(ExitCor());
        }

        private IEnumerator ExitCor()
        {
            GameInstance.UINavigation.TransitionAnimationGame();
            yield return new WaitForSeconds(0.6f);
            SceneManager.LoadScene(1);
        }
    }
}