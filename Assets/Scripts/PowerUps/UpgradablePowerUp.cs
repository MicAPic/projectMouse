using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PowerUps
{
    public abstract class UpgradablePowerUp<T>: PowerUpBase where T : class
    {
        [SerializeField]
        private TMP_Text titleText;
        [SerializeField]
        protected TMP_Text descriptionText;

        private static int _currentLevel = 1;
        private string _defaultTitle;
        private IDisposable _resetEvent;
        
        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += ResetCounter;
            //TODO: set colour
            titleText.text += $" <color=#102447>Lv.{_currentLevel}";
        }

        protected override void Activate()
        {
            _currentLevel++;
        }

        private void ResetCounter(Scene current, LoadSceneMode mode)
        {
            _currentLevel = 1;
            SceneManager.sceneLoaded -= ResetCounter;
        }
    }
}
