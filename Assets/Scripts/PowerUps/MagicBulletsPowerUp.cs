using UnityEngine;

namespace PowerUps
{
    public class MagicBulletsPowerUp : UpgradablePowerUp<MagicBulletsPowerUp>
    {
        [SerializeField]
        private string newDescription = "Add bullet for next magic circle";
        [SerializeField]
        private int maxBulletsInShield = 5;

        private bool _wasActivated;
        protected override void Awake()
        {
            base.Awake();
            if (PlayerController.Instance.MagicBulletsEnabled)
            {
                _wasActivated = true;
                descriptionText.text = newDescription;
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
            
            if (PlayerController.Instance.numOfMagicBullets > maxBulletsInShield)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
                return;
            }
            
            base.Activate();
        }
    }
}
