using GameCore;
using UnityEngine;
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
        
        public static UINavigation UINavigation => Default._uiNavigation;
        public static FXController FXController => Default._fxController;
        public static AudioSystem Audio => Default._audio;
        public static MusicSystem MusicSystem => Default._music;
        public static MoneyManager MoneyManager => Default._moneyManager;
        public static NovelController NovelController => Default._novelController;

        protected override void Awake()
        { 
            base.Awake();
            _uiNavigation.Init();
            _fxController.Init();
            _audio.Init();
            _music.Init();
            _moneyManager.Init(0);
            _novelController.Init();
            Application.targetFrameRate = 90;
        }
    }
}