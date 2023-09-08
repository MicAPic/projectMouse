using UnityEngine;

namespace PowerUps
{
    public class ShotGunPowerUp : UpgradablePowerUp<ShotGunPowerUp>
    {
        [SerializeField]
        private string newDescription = "Fire in one more direction\nThe more the merrier!";
        [SerializeField]
        private int maxBulletsToShoot = 5;

        private bool _wasActivated;

        protected override void Awake()
        {
            base.Awake();
            if (PlayerController.Instance.ShotgunPowerUpEnabled)
            {
                _wasActivated = true;
                descriptionText.text = newDescription;
            }
        }

        protected override void Activate()
        {
            if (!_wasActivated)
            {
                PlayerController.Instance.EnableShotgun();
            }
            else
            {
                PlayerController.Instance.AddShotgunBullet();
            }
            
            if (PlayerController.Instance.bulletsInShotgun > maxBulletsToShoot)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
                return;
            }
            
            base.Activate();
        }
    }
}
