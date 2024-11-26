using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    public class IngredientsCounter : MonoBehaviour
    {
        [SerializeField] private List<Image> _imagesPopup;
        [SerializeField] private List<Image> _imagesMenu;

        private void Start()
        {
            UpdateUI();
        }

        public void IncreaseIngredientsCount()
        {
            if(PrefsIngredientsCount == 2) return;
            PrefsIngredientsCount += 1;
            UpdateUI();
        }
        
        private int PrefsIngredientsCount
        {
            get => int.Parse(PlayerPrefs.GetString("PREFS_Ingredients", "-1"));
            set => PlayerPrefs.SetString("PREFS_Ingredients", value.ToString());
        }

        private void UpdateUI()
        {
            for (var i = 0; i <= PrefsIngredientsCount; i++)
            {
                _imagesPopup[i].enabled = true;
                _imagesMenu[i].enabled = true;
            }
        }
    }
}