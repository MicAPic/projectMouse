using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ShakeSetting : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text label;

        private bool _currentSetting;

        void Awake()
        {
            _currentSetting = Convert.ToBoolean(PlayerPrefs.GetInt("ShakeCamera", 1));
            SetLabel();
        }

        public void SetShake()
        {
            _currentSetting = !_currentSetting;
            PlayerPrefs.SetInt("ShakeCamera", Convert.ToInt32(_currentSetting));
            SetLabel();
        }

        private void SetLabel()
        {
            label.text = _currentSetting ? "On" : "Off";
        }
    }
}
