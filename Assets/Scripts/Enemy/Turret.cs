using UnityEngine;

namespace Enemy
{
    public class Turret : MonoBehaviour
    {
        [SerializeField] 
        private float damageToDeal = 0.3f;
        [SerializeField] 
        private Transform shootingPoint;
        [SerializeField] 
        private float fireRate = 0.5f;
        [SerializeField] 
        private float firePower = 20;

        private float _lastFireTime;

        // Update is called once per frame
        void Update()
        {
            if (Time.time - _lastFireTime >= fireRate)
            {
                var bullet = BulletPool.Instance.GetBulletFromPool(1);
                var sPosition = shootingPoint.transform.position;
                var direction = shootingPoint.right;
        
                bullet.transform.position = sPosition;
                bullet.Enable(direction, firePower, damageToDeal);
        
                _lastFireTime = Time.time;
            }
        }
    }
}
