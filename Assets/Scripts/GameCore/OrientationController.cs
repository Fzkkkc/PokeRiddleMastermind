using UnityEngine;

namespace GameCore
{
    public class OrientationController : MonoBehaviour
    {
        [SerializeField] private bool _isMain;

        private void Start()
        {
            if (_isMain)
            {
                ChangeToPortrait();
            }
            else
            {
                ChangeToLandscape();
            }
        }

        private void ChangeToPortrait()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        
        private void ChangeToLandscape()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }
}