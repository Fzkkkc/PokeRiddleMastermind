using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;
using UserInterface;

namespace GameCore
{
    public class SpriteScroller : MonoBehaviour
    {
        [SerializeField] private Image _frameImage;
        [SerializeField] private Button _leftArrowButton;
        [SerializeField] private Button _rightArrowButton;
        [SerializeField] private List<Sprite> _frameSprites;

        [SerializeField] private Button _activeButton;
        [SerializeField] private List<Sprite> _activeButtonSprites;
        [SerializeField] private ButtonFX _buttonFX;
        
        private int _currentIndex = 0;

        private void Start()
        {
            UpdateFrameImage();
            UpdateButtonStates();
            UpdateActiveButton();

            _leftArrowButton.onClick.AddListener(OnLeftArrowClicked);
            _rightArrowButton.onClick.AddListener(OnRightArrowClicked);
        }

        private void OnLeftArrowClicked()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                UpdateFrameImage();
                UpdateButtonStates();
                UpdateActiveButton();
            }
        }

        private void OnRightArrowClicked()
        {
            if (_currentIndex < _frameSprites.Count - 1)
            {
                _currentIndex++;
                UpdateFrameImage();
                UpdateButtonStates();
                UpdateActiveButton();
            }
        }

        private void UpdateFrameImage()
        {
            if (_frameSprites != null && _frameSprites.Count > 0)
            {
                _frameImage.sprite = _frameSprites[_currentIndex];
            }
        }

        private void UpdateButtonStates()
        {
            _leftArrowButton.interactable = _currentIndex > 0;
            _rightArrowButton.interactable = _currentIndex < _frameSprites.Count - 1;
        }

        private void UpdateActiveButton()
        {
            _activeButton.onClick.RemoveAllListeners();

            switch (_currentIndex)
            {
                case 0:
                    _activeButton.image.sprite = _activeButtonSprites[0];
                    _activeButton.onClick.AddListener(PlayFirstChapter);
                    break;

                case 1:
                    if (IsLevelPurchased(1))
                    {
                        _activeButton.image.sprite = _activeButtonSprites[0];
                        _activeButton.onClick.AddListener(PlaySecondChapter);
                    }
                    else
                    {
                        _activeButton.image.sprite = _activeButtonSprites[1];
                        _activeButton.onClick.AddListener(BuySecondLevel);
                    }
                    break;

                case 2:
                    if (IsLevelPurchased(2))
                    {
                        _activeButton.image.sprite = _activeButtonSprites[0];
                        _activeButton.onClick.AddListener(PlayThirdChapter);
                    }
                    else
                    {
                        _activeButton.image.sprite = _activeButtonSprites[2];
                        _activeButton.onClick.AddListener(BuyThirdLevel);
                    }
                    break;
            }
            
            _activeButton.onClick.AddListener(_buttonFX.PlayButtonFX);
        }

        private void PlayFirstChapter()
        {
            GameInstance.NovelController.CurrentChapterIndex = 0;
            GameInstance.UINavigation.OpenGameMenu();
        }

        private void PlaySecondChapter()
        {
            GameInstance.NovelController.CurrentChapterIndex = 1;
            GameInstance.UINavigation.OpenGameMenu();
        }

        private void PlayThirdChapter()
        {
            PlayerPrefs.SetInt("IsFromNovell", 0);
            PlayerPrefs.Save();
            GameInstance.NovelController.EnterUnoGame();
        }

        private void BuySecondLevel()
        {
            const int price = 500;

            if (!GameInstance.MoneyManager.HasEnoughCoinsCurrency(price)) return;

            GameInstance.MoneyManager.SpendCoinsCurrency(price);
            PlayerPrefs.SetInt("Level_1_Purchased", 1); 
            PlayerPrefs.Save();
            UpdateActiveButton();
        }

        private void BuyThirdLevel()
        {
            const int price = 1500;

            if (!GameInstance.MoneyManager.HasEnoughCoinsCurrency(price)) return;

            GameInstance.MoneyManager.SpendCoinsCurrency(price);
            PlayerPrefs.SetInt("Level_2_Purchased", 1); 
            PlayerPrefs.Save();
            UpdateActiveButton();
        }

        private bool IsLevelPurchased(int levelIndex)
        {
            return PlayerPrefs.GetInt($"Level_{levelIndex}_Purchased", 0) == 1;
        }

        private void OnDestroy()
        {
            _leftArrowButton.onClick.RemoveListener(OnLeftArrowClicked);
            _rightArrowButton.onClick.RemoveListener(OnRightArrowClicked);
            _activeButton.onClick.RemoveAllListeners();
        }
    }
}
