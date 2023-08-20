using UnityEngine;

namespace PowerUps
{
    public class FirePowerPowerUp : PowerUpBase
    {
        [SerializeField]
        private float modifier = 1.12f;
        [SerializeField]
        private float maxFirePower = 40.0f;
        
        protected override void Activate()
        {
            var playerControllers = FindObjectsOfType<PlayerController>();
            foreach (var playerController in playerControllers)
            {
                playerController.firePower *= modifier;
            }
            
            if (playerControllers[0].fireRate >= maxFirePower)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
            }
        }
    }
}
