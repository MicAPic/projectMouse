using UnityEngine;

namespace PowerUps
{
    public class MovementPowerUp : PowerUpBase
    {
        [SerializeField]
        private float modifier = 1.10f;
        [SerializeField]
        private float maxSpeed = float.MaxValue;
        
        protected override void Activate()
        {
            var playerControllers = FindObjectsOfType<PlayerController>();
            foreach (var playerController in playerControllers)
            {
                playerController.movementSpeed *= modifier;
            }

            if (playerControllers[0].movementSpeed > maxSpeed)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
            }
        }
    }
}
