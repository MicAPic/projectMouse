using UnityEngine;

namespace Bullets
{
    public class PlayerBullet : Bullet
    {
        private void OnBecameInvisible()
        {
            // Debug.Log("invisible");
            Disable();
        }
    }
}
