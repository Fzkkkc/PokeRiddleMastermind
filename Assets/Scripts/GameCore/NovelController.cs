using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
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
        
        public int CurrentSceneIndex;
        public int CurrentChapterIndex = 0;
        private bool _isTextAnimating;
        
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
                    ChangeBackground(_1ChapterSprites[1]);
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
                    ChangeBackground(_1ChapterSprites[3]);
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
                    ChangeBackground(_1ChapterSprites[3]);
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
                    GameInstance.UINavigation.OpenGameOverPopup();
                    break;
                default:
                    StartCoroutine(ShowPanelWithText(0, "Конец первой главы!"));
                    break;
            }
        }

        private void ShowChapter2Scenes()
        {
            switch (CurrentSceneIndex)
            {
                case 0:
                    StartCoroutine(ShowPanelWithText(0, "Aliya and Kael approach the altar and see unfamiliar symbols in another language"));
                    ChangeBackground(_1ChapterSprites[7]);
                    break;
                case 1:
                    StartCoroutine(ShowPanelWithText(0, "Aliyah begins to read the symbols on the altar. At first they seem meaningless, but then she begins to understand their meanings."));
                    ChangeBackground(_1ChapterSprites[7]);
                    break;
                case 2:
                    StartCoroutine(ShowPanelWithText(0, "Suddenly, a girl comes out of the darkness. She doesn't look like all the people around, she seems to glow from the inside."));
                    ChangeBackground(_1ChapterSprites[7]);
                    StartCoroutine(AnimateCharacter(1));
                    break;
                case 3:
                    StartCoroutine(ShowPanelWithText(1, "“You have come to the altar of the Volcano. Are you looking for answers?”", "Girl"));
                    StartCoroutine(AnimateCharacter(1));
                    ChangeBackground(_1ChapterSprites[7]);
                    break;
                case 4:
                    StartCoroutine(ShowPanelWithText(1, "“Yeah. We want to learn more about the volcano. He is very strong and threatens to destroy everything around him.”", "Aliya"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[7]);
                    break;
                case 5:
                    StartCoroutine(ShowPanelWithText(1, "“The volcano is the heart of the city. He keeps secrets that not everyone can know.“I can give you a clue. But you have to be ready for the challenge.", "Girl"));
                    StartCoroutine(AnimateCharacter(1));
                    ChangeBackground(_1ChapterSprites[7]);
                    break;
                case 6:
                    StartCoroutine(ShowPanelWithText(0, "She hands you a scroll."));
                    ChangeBackground(_1ChapterSprites[7]);
                    StartCoroutine(AnimateCharacter(1));
                    break;
                case 7:
                    ChangeBackground(_1ChapterSprites[7]);
                    StartCoroutine(AnimateCharacter(1));
                    break;
                case 8:
                    StartCoroutine(ShowPanelWithText(1, "The girl smiles a mysterious smile.”", "Girl"));
                    StartCoroutine(AnimateCharacter(1));
                    ChangeBackground(_1ChapterSprites[7]);
                    break;
                case 9:
                    StartCoroutine(ShowPanelWithText(1, "“I can't tell you everything. But I can tell you that you need to find three ingredients to open the way to the heart of the volcano. They are hiding in this city. Find them and calm the volcano.”", "Girl"));
                    StartCoroutine(AnimateCharacter(1));
                    ChangeBackground(_1ChapterSprites[7]);
                    CurrentSceneIndex = 10;
                    break;
                case 10:
                    StartCoroutine(ShowPanelWithText(1, "“This scroll says what you need to do to calm the volcano. But be careful. The path to the secrets of the volcano is dangerous.”", "Girl"));
                    StartCoroutine(AnimateCharacter(1));
                    ChangeBackground(_1ChapterSprites[7]);
                    break;
                case 11:
                    StartCoroutine(ShowPanelWithText(0, "She points to the symbols written on the scroll. You see three signs"));
                    ChangeBackground(_1ChapterSprites[7]);
                    StartCoroutine(AnimateCharacter(1));
                    break;
                case 12:
                    StartCoroutine(ShowPanelWithText(0, "The symbol of a bright red blooming orchid, which is located in a cave in the southern part of the city."));
                    ChangeBackground(_1ChapterSprites[7]);
                    StartCoroutine(AnimateCharacter(1));
                    break;
                case 13:
                    StartCoroutine(ShowPanelWithText(0, "A piece of frozen lava that makes a strange sound when pressed. This piece is located at the bottom of the volcano."));
                    ChangeBackground(_1ChapterSprites[7]);
                    StartCoroutine(AnimateCharacter(1));
                    break;
                case 14:
                    StartCoroutine(ShowPanelWithText(0, "Tear Stone: it lies at the bottom of a deep lake, which is located in the crater of a volcano."));
                    ChangeBackground(_1ChapterSprites[7]);
                    StartCoroutine(AnimateCharacter(1));
                    break;
                case 15:
                    StartCoroutine(ShowPanelWithText(0, "The girl suddenly disappeared"));
                    ChangeBackground(_1ChapterSprites[7]);
                    break;
                case 16:
                    GameInstance.UINavigation.OpenGameOverPopup();
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
    }
}