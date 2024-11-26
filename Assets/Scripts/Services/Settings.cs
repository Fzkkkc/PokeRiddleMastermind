using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Services
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private string _policyString;
        [SerializeField] private string _appURLString;

        [SerializeField] private Button _policyButton;
        [SerializeField] private Button _shareButton;
        [SerializeField] private Button _rateButton;
        
        private void Start()
        {
            _policyButton.onClick.AddListener(PolicyView);
            _shareButton.onClick.AddListener(ShareApp);
            _rateButton.onClick.AddListener(RateApp);
        }
        
        private void ShareApp()
        {
            if(_appURLString != "")
                Application.OpenURL(_appURLString);
        }

        private void PolicyView()
        {
            if(_policyString != "")
                Application.OpenURL(_policyString);
        }

        private void RateApp()
        {
#if UNITY_IOS
            Device.RequestStoreReview();
#endif
        }
    }
}
