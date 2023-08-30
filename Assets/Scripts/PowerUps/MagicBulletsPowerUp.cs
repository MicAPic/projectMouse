using UnityEngine;
namespace PowerUps
{
    public class MagicBulletsPowerUp : PowerUpBase
    {
        [SerializeField] private ShotGunPowerUp _shotGunPowerUp;
        protected override void Activate()
        {
            PlayerController.Instance.EnableMagicBullets();
            var powerUpName = _shotGunPowerUp.GetType().Name;
            foreach (var pair in ExperienceManager.Instance.PowerUpsWithCounters)
            {
                if (!pair.Key.name.Contains(powerUpName)) continue;
                ExperienceManager.Instance.PowerUpsWithCounters[pair.Key] += 1;
                break;
            }
            ExperienceManager.Instance.RemoveFromPowerUps(this);
        }
    }
}
