using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class MapRoadNavigation : MonoBehaviour
    {
        [SerializeField] private Button _1gameButtons;
        [SerializeField] private Button _2gameButtons;
        [SerializeField] private Button _3gameButtons;
        
        [SerializeField] private Image _coin2ButtonImage;
        [SerializeField] private Image _coin3ButtonImage;
        [SerializeField] private TextMeshProUGUI _text2Button;
        [SerializeField] private TextMeshProUGUI _text3Button;
        
        [SerializeField] private Sprite _2notPurchasedSprite;
        [SerializeField] private Sprite _3notPurchasedSprite;
        
        private Sprite _2PurchasedSprite;
        private Sprite _3PurchasedSprite;
        
        private int _level;
        private int _currentLevel;
        
        private int PrefsLevel
        {
            get => int.Parse(PlayerPrefs.GetString("PREFS_Level", "0"));
            set => PlayerPrefs.SetString("PREFS_Level", value.ToString());
        }

        public void Start()
        {
            _2PurchasedSprite = _2gameButtons.image.sprite;
            _3PurchasedSprite = _3gameButtons.image.sprite;
            _level = PrefsLevel;
            _1gameButtons.onClick.AddListener(OpenGameUI1Chapter);
            _2gameButtons.onClick.AddListener(Game2Chapter);
            _3gameButtons.onClick.AddListener(Game3Chapter);
            LoadSavesData();
        }

        public void IncreaseLevel()
        {
            PrefsLevel = _level = (_level + 1);
        }
        
        private void Game2Chapter()
        {
            if (PrefsLevel >= 1)
            {
                GameInstance.NovelController.CurrentChapterIndex = 1;
                GameInstance.UINavigation.OpenGameMenu();
            }
            else
            {
                if (!GameInstance.MoneyManager.HasEnoughCoinsCurrency(500)) return;
                GameInstance.MoneyManager.SpendCoinsCurrency(500);
                
                IncreaseLevel();
            }
            
            LoadSavesData();
        }
        
        private void Game3Chapter()
        {
            if (PrefsLevel >= 2)
            {
                GameInstance.NovelController.CurrentChapterIndex = 2;
                GameInstance.UINavigation.OpenGameMenu();
            }
            else if (PrefsLevel == 1)
            {
                if (!GameInstance.MoneyManager.HasEnoughCoinsCurrency(500)) return;
                GameInstance.MoneyManager.SpendCoinsCurrency(500);
                
                IncreaseLevel();
            }
            
            LoadSavesData();
        }

        private void OpenGameUI1Chapter()
        {
            GameInstance.NovelController.CurrentChapterIndex = 0;
            GameInstance.UINavigation.OpenGameMenu();
        }

        private void LoadSavesData()
        {
            switch (PrefsLevel)
            {
                case 1:
                    _coin2ButtonImage.gameObject.SetActive(false);
                    _text2Button.gameObject.SetActive(false);
                    _2gameButtons.image.sprite = _2PurchasedSprite;
                    _3gameButtons.image.sprite = _3notPurchasedSprite;
                    break;
                case 2:
                    _coin2ButtonImage.gameObject.SetActive(false);
                    _text2Button.gameObject.SetActive(false);
                    _coin3ButtonImage.gameObject.SetActive(false);
                    _text3Button.gameObject.SetActive(false);
                    _2gameButtons.image.sprite = _2PurchasedSprite;
                    _3gameButtons.image.sprite = _3PurchasedSprite;
                    break;
                default:
                    _coin2ButtonImage.gameObject.SetActive(true);
                    _text2Button.gameObject.SetActive(true);
                    _coin3ButtonImage.gameObject.SetActive(true);
                    _text3Button.gameObject.SetActive(true);
                    _3gameButtons.image.sprite = _3notPurchasedSprite;
                    _2gameButtons.image.sprite = _2notPurchasedSprite;
                    break;
            }
        }
    }
}
