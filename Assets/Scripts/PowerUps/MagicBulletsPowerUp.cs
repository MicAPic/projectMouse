using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PowerUps
{
    public class MagicBulletsPowerUp : PowerUpBase
    {
        protected override void Activate()
        {
            PlayerController.Instance.EnableMagicBullets();
            ExperienceManager.Instance.RemoveFromPowerUps(this);
        }
    }
}
