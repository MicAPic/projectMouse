using UnityEngine;

namespace HealthControllers
{
    public abstract class Health : MonoBehaviour
    {
        [SerializeField]
        private float healthPoints;

        public float GetCurrentHealth()
        {
            return healthPoints;
        }

        public virtual void TakeDamage(float damagePoints)
        {
            healthPoints -= damagePoints;
            if (healthPoints <= 0)
            {
                Die();
            }
        
            //TODO: animate taking damage here or in the children 
        }

        protected virtual void Die()
        {
            Destroy(gameObject);
        }
    }
}
