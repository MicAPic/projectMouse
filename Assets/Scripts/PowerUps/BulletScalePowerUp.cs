using UnityEngine;

namespace PowerUps
{
    public class BulletScalePowerUp : PowerUpBase
    {
        [SerializeField]
        private float modifier = 1.10f;
        [SerializeField]
        private float maxScale = 2.0f;
        
        protected override void Activate()
        {
            PlayerController.Instance.bulletScaleModifier *= modifier;

            if (PlayerController.Instance.bulletScaleModifier > maxScale)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
            }
        }
    }
}
