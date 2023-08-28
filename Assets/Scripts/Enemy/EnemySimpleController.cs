using UnityEngine;

namespace Enemy
{
    public class EnemySimpleController : EnemyController
    { 
        
        protected override void Shoot()
        {
            var bullet = BulletPool.Instance.GetBulletFromPool(1);
            var sPosition = shootingPoint.transform.position;
            Vector3 direction = (PlayerController.Instance.transform.position - sPosition).normalized;
            if (_firstShoot)
            {
                direction = Quaternion.AngleAxis(_blunderAngel, Vector3.forward) * direction;
                _firstShoot = false;
            }

            bullet.transform.position = sPosition;
            bullet.Enable(direction, _firePower, damageToDeal);
            
            _lastFireTime = Time.time;
        }
    }
}
