using UnityEngine;

namespace PowerUps
{
    public class FirePowerPowerUp : UpgradablePowerUp<FirePowerPowerUp>
    {
        [SerializeField]
        private float modifier = 1.12f;
        [SerializeField]
        private float maxFirePower = 40.0f;
        
        protected override void Activate()
        {
            
            PlayerController.Instance.firePower *= modifier;

            if (PlayerController.Instance.fireRate >= maxFirePower)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
                return;
            }
            
            base.Activate();
        }
    }
}
