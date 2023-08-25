using UnityEngine;
using UnityEngine.UI;

namespace PowerUps
{
    public abstract class PowerUpBase : MonoBehaviour
    {
        private Button _button;

        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Activate);
            _button.onClick.AddListener(ExperienceManager.Instance.SelectPowerUp);
            _button.onClick.AddListener(() =>
            {
                var powerUpName = GetType().Name;
                foreach (var pair in ExperienceManager.Instance.PowerUpsWithCounters)
                {
                    if (!pair.Key.name.Contains(powerUpName)) continue;
                    ExperienceManager.Instance.PowerUpsWithCounters[pair.Key]++;
                    return;
                }
            });
        }

        protected abstract void Activate();
    }
}
