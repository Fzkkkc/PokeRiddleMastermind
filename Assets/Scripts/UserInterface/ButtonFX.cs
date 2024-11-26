using System.Collections;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class ButtonFX : MonoBehaviour
    {
        [SerializeField] private Button _fxButton;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private bool _noSound;

        private bool _isAnimating = false;
        
        private void OnValidate()
        {
            _fxButton ??= GetComponent<Button>();
            _rectTransform ??= GetComponent<RectTransform>();
        }

        private void Start()
        {
            _fxButton.onClick.AddListener(PlayButtonFX);
        }

        private void OnDestroy()
        {
            _fxButton.onClick.RemoveListener(PlayButtonFX);
        }

        public void PlayButtonFX()
        {
            if(_isAnimating) return;
            _isAnimating = true;
            if (!_noSound)
            {
                GameInstance.Audio.Play(GameInstance.Audio.TapSound);
            }
            
            GameInstance.FXController.PlayTapParticle(_fxButton.transform.position);

            StartCoroutine(AnimateButtonScale());
        }

        private IEnumerator AnimateButtonScale()
        {
            Vector3 originalScale = _rectTransform.localScale;
            Vector3 scaleUp = new Vector3(1.2f,1.2f,1.2f);
            Vector3 scaleDown = new Vector3(0.7f,0.7f,0.7f);

            yield return ScaleOverTime(originalScale, scaleUp, animationDuration * 0.25f);

            yield return ScaleOverTime(scaleUp, scaleDown, animationDuration * 0.25f);

            yield return ScaleOverTime(scaleDown, originalScale, animationDuration * 0.5f);
            _isAnimating = false;
        }

        private IEnumerator ScaleOverTime(Vector3 from, Vector3 to, float duration)
        {
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                _rectTransform.localScale = Vector3.Lerp(from, to, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _rectTransform.localScale = to;
        }
    }
}
