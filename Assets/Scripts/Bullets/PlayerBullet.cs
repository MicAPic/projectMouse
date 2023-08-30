namespace Bullets
{
    public class PlayerBullet : Bullet
    {
        private void OnBecameInvisible()
        {
            Disable();
        }
    }
}
