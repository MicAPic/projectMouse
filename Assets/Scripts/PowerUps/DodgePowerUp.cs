using UnityEngine;

namespace PowerUps
{
    public class DodgePowerUp : UpgradablePowerUp<DodgePowerUp>
    {
        [SerializeField]
        private float modifier = 1.10f;
        [SerializeField]
        private float maxSpeed = float.MaxValue;
        
        protected override void Activate()
        {
            PlayerController.Instance.dodgeSpeedModifier *= modifier;

            if (PlayerController.Instance.dodgeSpeedModifier > maxSpeed)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
                return;
            }
            
            base.Activate();
        }
    }
}
