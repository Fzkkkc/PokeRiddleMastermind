using System.Collections;
using Services;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class LoadingAnimation : MonoBehaviour
    {
        [SerializeField] private TMP_Text _loadingText;
        [SerializeField] private bool _isTest = false;

        private void Start()
        {
            StartCoroutine(AnimatePercentage());
        }

        private IEnumerator AnimatePercentage()
        {
            GameInstance.UINavigation._isInLoad = true;
            var duration = 4.0f;
            StartCoroutine(AnimateText(duration));

            yield return new WaitForSeconds(duration); 

            if (_isTest)
            {
                GameInstance.UINavigation.OpenGameMenu();
            }
            else
            {
                GameInstance.UINavigation.OpenMainMenu();
            }
        }

        private IEnumerator AnimateText(float totalDuration)
        {
            string[] loadingStates = { "", ".", "..", "..." };
            float stateDuration = totalDuration / (loadingStates.Length * 4);
            int totalCycles = 4 * loadingStates.Length;

            for (int i = 0; i < totalCycles; i++)
            {
                _loadingText.text = loadingStates[i % loadingStates.Length]; 
                yield return new WaitForSeconds(stateDuration);
            }
        }
    }
}
