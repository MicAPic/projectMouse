using HealthControllers;

namespace PowerUps
{
    public class FullHealPowerUp : PowerUpBase
    {
        protected override void Activate()
        {
            var playerHealth = FindObjectOfType<PlayerHealth>();
            playerHealth.FullHeal();
        }
    }
}
