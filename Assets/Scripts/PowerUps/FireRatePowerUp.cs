using UnityEngine;

namespace PowerUps
{
    public class FireRatePowerUp : PowerUpBase
    {
        [SerializeField]
        private float modifier = 0.9f;
        
        protected override void Activate()
        {
            var playerControllers = FindObjectsOfType<PlayerController>();
            foreach (var playerController in playerControllers)
            {
                playerController.fireRate *= modifier;
            }
        }
    }
}
