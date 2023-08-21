using CameraShake;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace HealthControllers
{
    public class PlayerHealth : Health
    {
        [Header("HP")]
        public float defenceModifier = 1.0f;
        public float MaxHealth { get; private set; }
        
        [SerializeField] 
        [Tooltip("Everything below this percentage is considered low HP")]
        [Range(0, 1)]
        private float criticalThreshold = 0.1f;
        private bool _inCriticalCondition;

        [Header("Visuals")]
        [SerializeField] 
        private Image healthBar;
        [SerializeField] 
        private float healthBarChangeDuration;

        // Start is called before the first frame update
        void Start()
        {
            MaxHealth = healthPoints;
        }

        public void FullHeal()
        {
            healthPoints = MaxHealth;
            UpdateHealthBar();

            if (_inCriticalCondition)
            {
                ChatManager.Instance.ToggleCriticalHealthChatInfo();
            }
        }

        public void IncreaseMaxHealth(float increment)
        {
            MaxHealth += increment;
            UpdateHealthBar();
        }
        
        public override void TakeDamage(float damagePoints)
        {
            base.TakeDamage(damagePoints * defenceModifier);
            CameraShaker.Presets.Explosion2D(rotationStrength:0.1f);
            UpdateHealthBar();

            if (healthPoints <= criticalThreshold * MaxHealth && !_inCriticalCondition)
            {
                ChatManager.Instance.ToggleCriticalHealthChatInfo();
                _inCriticalCondition = true;
            }
        }

        protected override void Die() 
        {
            GameManager.Instance.GameOver();
        }

        private void UpdateHealthBar()
        {
            healthBar.DOFillAmount(healthPoints / MaxHealth, healthBarChangeDuration);
        }
    }
}