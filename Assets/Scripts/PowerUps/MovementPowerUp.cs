using UnityEngine;

namespace PowerUps
{
    public class MovementPowerUp : UpgradablePowerUp<MovementPowerUp>
    {
        [SerializeField]
        private float modifier = 1.10f;
        [SerializeField]
        private float maxSpeed = float.MaxValue;
        
        protected override void Activate()
        {
            PlayerController.Instance.movementSpeed *= modifier;

            if (PlayerController.Instance.movementSpeed > maxSpeed)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
                return;
            }
            
            base.Activate();
        }
    }
}
