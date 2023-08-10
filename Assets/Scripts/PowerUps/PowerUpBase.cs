using System;
using UnityEngine;
using UnityEngine.UI;

namespace PowerUps
{
    public abstract class PowerUpBase : MonoBehaviour
    {
        private Button _button;

        void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Activate);
            _button.onClick.AddListener(ExperienceManager.Instance.SelectPowerUp);
        }

        protected abstract void Activate();
    }
}
