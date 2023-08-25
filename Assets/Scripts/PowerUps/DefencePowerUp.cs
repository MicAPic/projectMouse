using HealthControllers;
using UnityEngine;

namespace PowerUps
{
    public class DefencePowerUp : PowerUpBase
    {
        [Tooltip("How long you're going to be invincible (in seconds)")]
        [SerializeField]
        private float powerUpDuration = 10.0f;
        [Tooltip("How long you're going to flash before loosing the power up (in seconds)")]
        [SerializeField]
        private float flashingDuration = 3.0f;

        protected override void Activate()
        {
            var playerHealth = FindObjectOfType<PlayerHealth>();
            playerHealth.GrantInvincibility(powerUpDuration, flashingDuration);
        }
    }
}
