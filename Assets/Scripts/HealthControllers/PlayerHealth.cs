using CameraShake;

namespace HealthControllers
{
    public class PlayerHealth : Health
    {
        public float defenceModifier = 1.0f;
        public float MaxHealth { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            MaxHealth = healthPoints;
        }

        public void FullHeal()
        {
            healthPoints = MaxHealth;
        }

        public void IncreaseMaxHealth(float increment)
        {
            MaxHealth += increment;
        }
        
        public override void TakeDamage(float damagePoints)
        {
            base.TakeDamage(damagePoints * defenceModifier);
            CameraShaker.Presets.Explosion2D(rotationStrength:0.1f);
        }

        protected override void Die() 
        {
            GameManager.Instance.GameOver();
        }
    }
}