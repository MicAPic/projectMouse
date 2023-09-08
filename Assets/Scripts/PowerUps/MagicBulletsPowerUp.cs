using UnityEngine;
using TMPro;
namespace PowerUps
{

    public class MagicBulletsPowerUp : PowerUpBase
    {

        [SerializeField]
        private TMP_Text descriptionTMP;
        [SerializeField]
        private string newDescription = "Add bullet for next magic circle";

        private bool _wasActivated = false;
        protected override void Awake()
        {
            base.Awake();
            if (PlayerController.Instance.MagicBulletsEnabled)
            {
                _wasActivated = true;
                descriptionTMP.text = newDescription;
            }
        }
        protected override void Activate()
        {
            if (!_wasActivated)
            {
                PlayerController.Instance.EnableMagicBullets();
            }
            else
            {
                PlayerController.Instance.AddMagicBullet();
            }
        }
    }
}
