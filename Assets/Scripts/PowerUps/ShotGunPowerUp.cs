using UnityEngine;
using TMPro;

namespace PowerUps
{
    public class ShotGunPowerUp : PowerUpBase
    {
        [SerializeField]
        private TMP_Text descriptionTMP;
        [SerializeField]
        private string newDescription = "Add bullet for shoot";

        private bool _wasActivated = false;

        protected override void Awake()
        {
            base.Awake();
            if (PlayerController.Instance.ShotgunPowerUpEnabled)
            {
                _wasActivated = true;
                descriptionTMP.text = newDescription;
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
        }
    }
}
