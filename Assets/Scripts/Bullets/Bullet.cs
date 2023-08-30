using HealthControllers;
using UnityEngine;

namespace Bullets
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] 
        private float damage;
        private Rigidbody2D _rb;

        void Awake()
        {
            _rb = GetComponentInChildren<Rigidbody2D>();
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.TryGetComponent(out Health healthController) && healthController.enabled)
            {
                healthController.TakeDamage(damage);
            }
            Disable();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Health healthController) && healthController.enabled)
            {
                healthController.TakeDamage(damage);
            }
            Disable();
        }

        public void Enable(Vector3 direction, float firePower, float damageToDeal, float scaleModifer=1.0f)
        {
            var normalizedDirection = (Vector2) direction;
            normalizedDirection.Normalize();

            transform.localScale = Vector3.one * scaleModifer; 
        
            damage = damageToDeal;
            gameObject.SetActive(true);
            _rb.AddForce(normalizedDirection * firePower, ForceMode2D.Impulse);
        }

        public void EnableWithoutForce(float damageToDeal)
        {
            damage = damageToDeal;
            gameObject.SetActive(true);
        }

        public void EnableBulletForce(Vector3 direction, float firePower)
        {
            var normalizedDirection = (Vector2)direction;
            normalizedDirection.Normalize();

            _rb.AddForce(normalizedDirection * firePower, ForceMode2D.Impulse);
        }

        protected void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
