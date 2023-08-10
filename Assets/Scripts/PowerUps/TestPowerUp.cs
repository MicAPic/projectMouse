namespace PowerUps
{
    public class TestPowerUp : PowerUpBase
    {
        protected override void Activate()
        {
            var playerControllers = FindObjectsOfType<PlayerController>();
            foreach (var playerController in playerControllers)
            {
                playerController.fireRate /= 2.0f;
            }
        }
    }
}
