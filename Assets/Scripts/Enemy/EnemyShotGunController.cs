using UnityEngine;

namespace Enemy
{
    public class EnemyShotGunController : EnemyController
    {
    
        [SerializeField] private float _shootAngel = 20;
        protected override void Shoot()
        {
            var bullet = BulletPool.Instance.GetBulletFromPool(1);
            var leftBullet = BulletPool.Instance.GetBulletFromPool(1);
            var rightBullet = BulletPool.Instance.GetBulletFromPool(1);

            var sPosition = _shootingPoint.transform.position;

            Vector3 direction = PlayerController.Instance.transform.position - sPosition;
            direction.Normalize();
            Vector3 leftDirection = Quaternion.AngleAxis(Random.Range(5f, _shootAngel), Vector3.forward) * direction;
            Vector3 rightDirection = Quaternion.AngleAxis(-Random.Range(5f, _shootAngel), Vector3.forward) * direction;

            bullet.transform.position = sPosition;
            bullet.Enable(direction, _firePower, damageToDeal);

            leftBullet.transform.position = sPosition;
            leftBullet.Enable(leftDirection, _firePower, damageToDeal);

            rightBullet.transform.position = sPosition;
            rightBullet.Enable(rightDirection, _firePower, damageToDeal);
            
            _lastFireTime = Time.time;
        }
    }
}
