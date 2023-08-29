using UnityEngine;

namespace Enemy
{
    public class EnemyShotGunController : EnemyController
    {
    
        [SerializeField] private float _shootAngel = 20;
        protected override void Shoot()
        {
            var sPosition = shootingPoint.transform.position;

            Vector3 direction = PlayerController.Instance.transform.position - sPosition;
            direction.Normalize();
            if (_firstShoot)
            {
                direction = Quaternion.AngleAxis(_blunderAngel, Vector3.forward) * direction;
                _firstShoot = false;
            }
            Vector3 leftDirection = Quaternion.AngleAxis(Random.Range(5f, _shootAngel), Vector3.forward) * direction;
            Vector3 rightDirection = Quaternion.AngleAxis(-Random.Range(5f, _shootAngel), Vector3.forward) * direction;

            var bullet = BulletPool.Instance.GetBulletFromPool(1);
            bullet.transform.position = sPosition;
            bullet.Enable(direction, _firePower, damageToDeal);

            bullet = BulletPool.Instance.GetBulletFromPool(1);
            bullet.transform.position = sPosition;
            bullet.Enable(leftDirection, _firePower, damageToDeal);

            bullet = BulletPool.Instance.GetBulletFromPool(1);
            bullet.transform.position = sPosition;
            bullet.Enable(rightDirection, _firePower, damageToDeal);
            
            _lastFireTime = Time.time;
        }
    }
}
