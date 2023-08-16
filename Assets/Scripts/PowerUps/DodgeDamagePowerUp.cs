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
        private PlayerController[] playerControllers;

        protected override void Awake()
        {
            base.Awake();
            playerControllers = FindObjectsOfType<PlayerController>();
            if (playerControllers[0].dodgeDamage > 0)
            {
                _wasActivated = true;
                descriptionTMP.text = newDescription;
            }
        }

        protected override void Activate()
        {
            if (_wasActivated)
            {
                playerControllers[0].dodgeDamage *= modifier;
            }
            else
            {
                playerControllers[0].dodgeDamage = initialValue;
            }

            if (playerControllers[0].dodgeDamage > maxDamageToDeal)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
            }
        }
    }
}
