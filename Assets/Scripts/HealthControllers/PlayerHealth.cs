using System;
using CameraShake;

namespace HealthControllers
{
    public class PlayerHealth : Health
    {
        private float _maxHealth;

        // Start is called before the first frame update
        void Start()
        {
            _maxHealth = healthPoints;
        }

        public void FullHeal()
        {
            healthPoints = _maxHealth;
        }
        
        public override void TakeDamage(float damagePoints)
        {
            base.TakeDamage(damagePoints);
            CameraShaker.Presets.Explosion2D(rotationStrength:0.1f);
        }

        protected override void Die() 
        {
            GameManager.Instance.GameOver();
        }
    }
}