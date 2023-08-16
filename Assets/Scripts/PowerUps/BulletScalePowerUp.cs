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
            var playerControllers = FindObjectsOfType<PlayerController>();
            foreach (var playerController in playerControllers)
            {
                playerController.bulletScaleModifier *= modifier;
            }

            if (playerControllers[0].bulletScaleModifier > maxScale)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
            }
        }
    }
}
