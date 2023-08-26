using UnityEngine;

namespace Enemy
{
    public class EnemySimpleController : EnemyController
    { 
        protected override void Shoot()
        {
            var bullet = BulletPool.Instance.GetBulletFromPool(1);
            var sPosition = _shootingPoint.transform.position;

            Vector3 direction = (PlayerController.Instance.transform.position - sPosition).normalized;

            bullet.transform.position = sPosition;
            bullet.Enable(direction, _firePower, damageToDeal);
        }
    }
}
