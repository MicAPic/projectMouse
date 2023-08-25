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
            
            PlayerController.Instance.fireRate -= modifier;

            if (PlayerController.Instance.fireRate <= maxFireRate)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
            }
        }
    }
}
