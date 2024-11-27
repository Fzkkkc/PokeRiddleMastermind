using UnityEngine;

namespace Services
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;
        public static T Default => _instance;
        public static bool HasDefault => _instance != null;

        protected bool isOrigin;

        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                DontDestroyOnLoad(gameObject);
                isOrigin = true;
            }
            else if (_instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (isOrigin)
            {
                _instance = null;
            }
        }
    }
}