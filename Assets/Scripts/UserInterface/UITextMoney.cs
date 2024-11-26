using Services;
using TMPro;
using UnityEngine;
using System.Collections;

namespace UserInterface
{
    public class UITextMoney : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currencyText;

        private void OnValidate()
        {
            _currencyText ??= GetComponentInChildren<TextMeshProUGUI>();
        }

        protected void Start()
        {
            GameInstance.MoneyManager.OnCoinsCurrencyChange += OnMoneyChanged;
            OnMoneyChanged(GameInstance.MoneyManager.GetCoinsCurrency());
        }
        
        private void OnDestroy()
        {
            GameInstance.MoneyManager.OnCoinsCurrencyChange -= OnMoneyChanged;
        }
        
        private void OnMoneyChanged(ulong money) 
        {
            StartCoroutine(AnimateTextChange(money));
            if (money != 0)
            {
                StartCoroutine(AnimateScale());
            }
           
        }

        private IEnumerator AnimateTextChange(ulong newAmount)
        {
            ulong currentAmount;
            if (ulong.TryParse(_currencyText.text, out currentAmount))
            {
                float elapsedTime = 0f;
                while (elapsedTime < 0.7f)
                {
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedTime / 0.7f);
                    ulong intermediateAmount = (ulong)Mathf.Lerp(currentAmount, (float)newAmount, t);
                    _currencyText.SetText(intermediateAmount.ToString());
                    yield return null;
                }
                _currencyText.SetText(newAmount.ToString());
            }
            else
            {
                _currencyText.SetText(newAmount.ToString());
            }
        }

        private IEnumerator AnimateScale()
        {
            Vector3 scaleUp = new Vector3(1.2f, 1.2f, 1.2f);
            Vector3 scaleDown = Vector3.one;
            bool scalingUp = true;

            float animationDuration = 0.7f;
            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                transform.localScale = scalingUp ? scaleUp : scaleDown;
                scalingUp = !scalingUp;
                yield return new WaitForSeconds(0.1f);
                elapsedTime += 0.1f;
            }

            transform.localScale = Vector3.one;
        }
    }
}
