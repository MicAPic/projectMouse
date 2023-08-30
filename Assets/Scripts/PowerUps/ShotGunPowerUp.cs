using UnityEngine;
namespace PowerUps
{
    public class ShotGunPowerUp : PowerUpBase
    {
        [SerializeField] private MagicBulletsPowerUp _magicBulletsPowerUp;
        protected override void Activate()
        {
            PlayerController.Instance.EnableShotgun();
            var powerUpName = _magicBulletsPowerUp.GetType().Name;
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
