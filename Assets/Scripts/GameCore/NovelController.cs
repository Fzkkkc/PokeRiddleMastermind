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
                    StartCoroutine(ShowPanelWithText(0, "The Wonders of the World Circus was moving towards the next city, leaving behind a cloud of dust and the noise of cheerful melodies."));
                    ChangeBackground(_1ChapterSprites[0]);
                    break;
                case 1:
                    StartCoroutine(ShowPanelWithText(0, "There was a girl named Aliya sitting in one of the carriages. Her bright carnival costume with sequins and feathers looked ridiculous against the background of the dusty road, but she was confident in her beauty."));
                    ChangeBackground(_1ChapterSprites[1]);
                    break;
                case 2:
                    StartCoroutine(ShowPanelWithText(1, "“I'm so tired of the road… Are we coming to town soon? I want to perform already!”", "Aliya"));
                    ChangeBackground(_1ChapterSprites[1]);
                    break;
                case 3:
                    StartCoroutine(ShowPanelWithText(1, "“Maybe I'll meet my prince in this city? Or at least an interesting clown...”", "Aliya"));
                    ChangeBackground(_1ChapterSprites[1]);
                    break;
                case 4:
                    StartCoroutine(ShowPanelWithText(2, " “Aliya, are you already dreaming of a prince? What about our circus? We need to prepare for the performance and impress the audience with new tricks!”", "Kael"));
                    ChangeBackground(_1ChapterSprites[1]);
                    break;
                case 5:
                    StartCoroutine(ShowPanelWithText(1, "“Of course, Kael, you know how much I love the circus. But it doesn't hurt to dream a little too.””", "Aliya"));
                    ChangeBackground(_1ChapterSprites[1]);
                    break;
                case 6:
                    StartCoroutine(ShowPanelWithText(0, "Suddenly, the car braked sharply. Kael and Alia lost their balance and fell."));
                    ChangeBackground(_1ChapterSprites[0]);
                    break;
                case 7:
                    StartCoroutine(ShowPanelWithText(1, "Wow! What an interesting town… And what a volcano! Maybe we can put on a show here too?", "Aliya"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[2]);
                    break;
                case 8:
                    StartCoroutine(ShowPanelWithText(2, "I don't know, Aliya… I feel something is wrong. The volcano does not seem to be simple…", "Kael"));
                    StartCoroutine(AnimateCharacter(2));
                    ChangeBackground(_1ChapterSprites[2]);
                    break;
                case 9:
                    ChangeBackground(_1ChapterSprites[2]);
                    break;
                case 10:
                    StartCoroutine(ShowPanelWithText(0, "You left the circus cars at the edge of the city and moved to the center. The houses were built of dark volcanic stone, and the streets were narrow and winding."));
                    ChangeBackground(_1ChapterSprites[2]);
                    break;
                case 11:
                    StartCoroutine(ShowPanelWithText(2, "“This place looks like a set for a horror movie!”", "Kael"));
                    StartCoroutine(AnimateCharacter(2));
                    ChangeBackground(_1ChapterSprites[2]);
                    break;
                case 12:
                    StartCoroutine(ShowPanelWithText(1, "“Just look at how beautiful it is! Imagine what bright and magical effects we can do during a performance with such an atmosphere!”", "Aliya"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[2]);
                    break;
                case 13:
                    StartCoroutine(ShowPanelWithText(2, "“Yes, but I still feel a little awkward. This place seems very strange.”", "Kael"));
                    StartCoroutine(AnimateCharacter(2));
                    ChangeBackground(_1ChapterSprites[2]);
                    break;
                case 14:
                    StartCoroutine(ShowPanelWithText(0, "You see a group of people standing at the entrance to some room. They are dressed in dark clothes and look at you with disbelief. One of them comes forward and addresses you in a harsh voice."));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 15:
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 16:
                    StartCoroutine(ShowPanelWithText(2, "“Don't worry, Aliya! We'll be leaving here soon! In the next city we will put on the most magnificent show in the world!”", "Kael"));
                    ChangeBackground(_1ChapterSprites[1]);
                    break;
                case 17:
                    StartCoroutine(ShowPanelWithText(1, "“I can't get this volcano out of my head… It doesn't seem simple. There's something so... mystical about him...”", "Aliya"));
                    ChangeBackground(_1ChapterSprites[1]);
                    break;
                case 18:
                    StartCoroutine(ShowPanelWithText(0, "Suddenly, the train starts shaking. At first it is easy, but gradually the trembling increases, and the car sways from side to side.. You run out of the carriage"));
                    ChangeBackground(_1ChapterSprites[6]);
                    break;
                case 19:
                    StartCoroutine(ShowPanelWithText(1, "It seems to be a volcano! It's like he's shaking the earth!”", "Aliya"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[6]);
                    break;
                case 20:
                    StartCoroutine(ShowPanelWithText(0, "Kael pulls out a small doll from his pocket, which he always carries with him. Kael casts a spell, and the doll turns into a bright bird"));
                    ChangeBackground(_1ChapterSprites[6]);
                    StartCoroutine(AnimateCharacter(2));
                    break;
                case 21:
                    StartCoroutine(ShowPanelWithText(0, "The bird flies into the air, carrying them away from the trembling train."));
                    ChangeBackground(_1ChapterSprites[2]);
                    CurrentSceneIndex = 41;
                    break;
                case 22:
                    StartCoroutine(ShowPanelWithText(1, "“Hello! We are the Wonders of the World Circus! We came to your city to please you with our show!”", "Aliya"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 23:
                    StartCoroutine(ShowPanelWithText(1, "“The circus? Why would you come here? This is not a place for fun. Only the volcano reigns here!”", "Man"));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 24:
                    StartCoroutine(ShowPanelWithText(1, "“But we came for a reason. We want to help this city! We can put on a show and cheer you up!”", "Aliya"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 25:
                    StartCoroutine(ShowPanelWithText(2, "“Yes, and we want to learn more about the volcano. Maybe we can help you with it?”", "Kael"));
                    StartCoroutine(AnimateCharacter(2));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 26:
                    StartCoroutine(ShowPanelWithText(1, "“Help me? With a volcano? No one can help with the volcano. He's a force of nature, and he doesn't need help.”", "Man"));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 27:
                    StartCoroutine(ShowPanelWithText(1, "“But he's so restless! He threatens to destroy your city!”", "Aliya"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 28:
                    StartCoroutine(ShowPanelWithText(1, "“This is a natural cycle. The volcano is breathing, and we must live in harmony with it.”", "Man"));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 29:
                    StartCoroutine(ShowPanelWithText(1, "“Well, if you're so brave, then come in. But don't say we didn't warn you.”", "Man"));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 30:
                    StartCoroutine(ShowPanelWithText(0, "He steps aside and you enter the temple. It is dark and damp inside, and in the center of the temple there is a huge altar on which a fiery torch is burning."));
                    ChangeBackground(_1ChapterSprites[5]);
                    break;
                case 31:
                    GameInstance.UINavigation.OpenGameOverPopup();
                    break;
                case 32:
                    StartCoroutine(ShowPanelWithText(1, "“Sorry, we just wanted to know more about this volcano. We have heard many stories about him, but we want to find out the truth from the locals.”", "Aliya"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 33:
                    StartCoroutine(ShowPanelWithText(1, "“You shouldn't have come here. This place is not for outsiders.”", "Man"));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 34:
                    StartCoroutine(ShowPanelWithText(1, "“But we want to help! The volcano is very strong, and we want to find out what can be done to prevent it from destroying your city.”", "Aliya"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 35:
                    StartCoroutine(ShowPanelWithText(1, "“Help me? With a volcano? No one can help with the volcano. He's a force of nature, and he doesn't need help.”", "Man"));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 36:
                    StartCoroutine(ShowPanelWithText(1, "“You shouldn't have come here. This place is not for you. But since you are already here, you will see that you are not destined to know.”", "Man"));
                    StartCoroutine(AnimateCharacter(3));
                    ChangeBackground(_1ChapterSprites[3]);
                    CurrentSceneIndex = 29;
                    break;
                case 37:
                    StartCoroutine(ShowPanelWithText(0, "You turn around and leave the temple."));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 38:
                    StartCoroutine(ShowPanelWithText(2, "“Aliya, wait! Maybe we could learn something else from them? Maybe they know how to calm the volcano?”", "Kael"));
                    StartCoroutine(AnimateCharacter(2));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 39:
                    StartCoroutine(ShowPanelWithText(1, "“I do not know, Kael. But I feel like we shouldn't stop. We have to go to the circus and get ready for the performance. Maybe we'll find the answers elsewhere.”", "Aliya"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 40:
                    StartCoroutine(ShowPanelWithText(0, "You leave the temple and turn towards the city. In the distance you can see the circus wagons that stood on the edge of the city."));
                    ChangeBackground(_1ChapterSprites[0]);
                    break;
                case 41:
                    ChangeBackground(_1ChapterSprites[0]);
                    break;
                case 42:
                    StartCoroutine(ShowPanelWithText(0, "The bird landed on the edge of the city, next to the very temple where you met people in dark clothes. Aliya walked towards the temple."));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 43:
                    StartCoroutine(ShowPanelWithText(2, "“What are you doing? You can't go there! It's dangerous out there!”", "Kael"));
                    StartCoroutine(AnimateCharacter(2));
                    ChangeBackground(_1ChapterSprites[3]);
                    break;
                case 44:
                    StartCoroutine(ShowPanelWithText(1, "“I have to try. I feel that there is something important in this temple, something that can help us save the city.”", "Aliya"));
                    StartCoroutine(AnimateCharacter(0));
                    ChangeBackground(_1ChapterSprites[3]);
                    CurrentSceneIndex = 29;
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
                    GameInstance.UINavigation.CloseRecepiePopup();
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