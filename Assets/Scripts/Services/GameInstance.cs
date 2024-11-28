using GameCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using UserInterface;

namespace Services
{
    public class GameInstance : Singleton<GameInstance>
    {
        [SerializeField] private UINavigation _uiNavigation;
        [SerializeField] private FXController _fxController;
        [SerializeField] private AudioSystem _audio;
        [SerializeField] private MusicSystem _music;
        [SerializeField] private MoneyManager _moneyManager;
        [SerializeField] private NovelController _novelController;
        [SerializeField] private LoadingAnimation _loadingAnimation;

        public static UINavigation UINavigation => Default?._uiNavigation;
        public static FXController FXController => Default?._fxController;
        public static AudioSystem Audio => Default?._audio;
        public static MusicSystem MusicSystem => Default?._music;
        public static MoneyManager MoneyManager => Default?._moneyManager;
        public static NovelController NovelController => Default?._novelController;
        public static LoadingAnimation LoadingAnimation => Default?._loadingAnimation;

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 90;
            if (isOrigin)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                InitializeComponents();
            }
        }

        private void InitializeComponents()
        {
            _uiNavigation ??= FindObjectOfType<UINavigation>();
            _novelController ??= FindObjectOfType<NovelController>();
            _fxController ??= FindObjectOfType<FXController>();
            _audio ??= FindObjectOfType<AudioSystem>();
            _music ??= FindObjectOfType<MusicSystem>();
            _loadingAnimation ??= FindObjectOfType<LoadingAnimation>();
            _moneyManager ??= FindObjectOfType<MoneyManager>();

            _uiNavigation?.Init();
            _novelController?.Init();
            _fxController?.Init();
            _audio?.Init();
            _music?.Init();
            _loadingAnimation?.Init();
            _moneyManager?.Init(0);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_uiNavigation == null || _novelController == null || _fxController == null ||
                _audio == null || _music == null || _loadingAnimation == null || _moneyManager == null)
            {
                InitializeComponents();
            }
            
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                Screen.orientation = ScreenOrientation.Portrait;
                _uiNavigation.gameObject.SetActive(true);
                _loadingAnimation.Init();
            }
            else if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                _uiNavigation.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (isOrigin)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
    }
}
