using UnityEngine;

namespace PowerUps
{
    public class DamagePowerUp : PowerUpBase
    {
        [SerializeField]
        private float modifier = 1.15f;
        
        protected override void Activate()
        {
            var playerControllers = FindObjectsOfType<PlayerController>();
            foreach (var playerController in playerControllers)
            {
                playerController.damageToDeal *= modifier;
            }
        }
    }
}
