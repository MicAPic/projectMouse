using HealthControllers;
using UnityEngine;

namespace PowerUps
{
    public class MaxHealthPowerUp : PowerUpBase
    {
        [SerializeField]
        private float increment = 10f;
        [SerializeField]
        private float maxHealth = 200.0f;
        
        protected override void Activate()
        {
            var playerHealth = FindObjectOfType<PlayerHealth>();
            playerHealth.IncreaseMaxHealth(increment);
            
            if (playerHealth.MaxHealth >= maxHealth)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
            }
        }
    }
}
