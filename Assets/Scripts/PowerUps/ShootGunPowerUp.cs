using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PowerUps
{
    public class ShootGunPowerUp : PowerUpBase
{
        protected override void Activate()
        {
            PlayerController.Instance.EnableShootGun();
            ExperienceManager.Instance.RemoveFromPowerUps(this);
        }
    }
}
