using UnityEngine;

namespace PowerUps
{
    public class FireRatePowerUp : PowerUpBase
    {
        [SerializeField]
        private float modifier = 0.9f;
        [SerializeField]
        private float maxFireRate = 0.1f;
        
        protected override void Activate()
        {
            var playerControllers = FindObjectsOfType<PlayerController>();
            foreach (var playerController in playerControllers)
            {
                playerController.fireRate -= modifier;
            }
            
            if (playerControllers[0].fireRate <= maxFireRate)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
            }
        }
    }
}
