using UnityEngine;

namespace PowerUps
{
    public class DamagePowerUp : UpgradablePowerUp<DamagePowerUp>
    {
        [SerializeField]
        private float modifier = 50f;
        [SerializeField]
        private float maxDamageToDeal = float.MaxValue;

        protected override void Awake()
        {
            base.Awake();
            descriptionText.text = descriptionText.text.Replace("?", 
                                                                (modifier * 100 / PlayerController.Instance.damageToDeal)
                                                                .ToString("N0"));
        }

        protected override void Activate()
        {
            PlayerController.Instance.damageToDeal += modifier;

            if (PlayerController.Instance.damageToDeal > maxDamageToDeal)
            {
                ExperienceManager.Instance.RemoveFromPowerUps(this);
                return;
            }
            
            base.Activate();
        }
    }
}
