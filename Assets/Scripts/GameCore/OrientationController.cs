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
            Debug.Log("to portrait");
            Screen.orientation = ScreenOrientation.Portrait;
        }
        
        private void ChangeToLandscape()
        {
            Debug.Log("to Landscape");
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }
}