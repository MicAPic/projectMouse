using UnityEngine;

namespace PowerUps
{
    public class DodgePowerUp : PowerUpBase
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
                playerController.dodgeSpeedModifier *= modifier;
            }

            if (playerControllers[0].dodgeSpeedModifier > maxSpeed)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
            }
        }
    }
}
