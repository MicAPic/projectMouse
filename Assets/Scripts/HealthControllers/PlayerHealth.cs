using CameraShake;

namespace HealthControllers
{
    public class PlayerHealth : Health
    {
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