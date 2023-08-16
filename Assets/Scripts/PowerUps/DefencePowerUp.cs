using HealthControllers;
using UnityEngine;

namespace PowerUps
{
    public class DefencePowerUp : PowerUpBase
    {
        [SerializeField]
        private float modifier = 0.9f;
        [SerializeField]
        private float maxDefenceModifier = 0.5f;
        
        protected override void Activate()
        {
            var playerHealth = FindObjectOfType<PlayerHealth>();
            playerHealth.defenceModifier *= modifier;

            if (playerHealth.defenceModifier < maxDefenceModifier)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
            }
        }
    }
}
