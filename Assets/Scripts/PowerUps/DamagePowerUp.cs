using UnityEngine;

namespace PowerUps
{
    public class DamagePowerUp : PowerUpBase
    {
        [SerializeField]
        private float modifier = 1.15f;
        [SerializeField]
        private float maxDamageToDeal = float.MaxValue;
        
        protected override void Activate()
        {
            var playerControllers = FindObjectsOfType<PlayerController>();
            foreach (var playerController in playerControllers)
            {
                playerController.damageToDeal *= modifier;
            }

            if (playerControllers[0].damageToDeal > maxDamageToDeal)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
            }
        }
    }
}
