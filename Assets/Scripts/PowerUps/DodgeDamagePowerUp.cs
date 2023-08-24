using TMPro;
using UnityEngine;

namespace PowerUps
{
    public class DodgeDamagePowerUp : PowerUpBase
    {
        [SerializeField]
        private float initialValue = 10.0f;
        [SerializeField]
        private float modifier = 1.15f;
        [SerializeField]
        private float maxDamageToDeal = float.MaxValue;
        
        [SerializeField]
        private TMP_Text descriptionTMP;
        [SerializeField]
        private string newDescription = "Increase dash damage by 15%\nMake you look hornier";

        private bool _wasActivated;

        protected override void Awake()
        {
            base.Awake();
            if (PlayerController.Instance.dodgeDamage > 0)
            {
                _wasActivated = true;
                descriptionTMP.text = newDescription;
            }
        }

        protected override void Activate()
        {
            if (_wasActivated)
            {
                PlayerController.Instance.dodgeDamage *= modifier;
            }
            else
            {
                PlayerController.Instance.dodgeDamage = initialValue;
            }

            if (PlayerController.Instance.dodgeDamage > maxDamageToDeal)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
            }
        }
    }
}
