namespace PowerUps
{
    public class ShotGunPowerUp : PowerUpBase
    {
        protected override void Activate()
        {
            PlayerController.Instance.EnableShotgun();
            ExperienceManager.Instance.RemoveFromPowerUps(this);
        }
    }
}
