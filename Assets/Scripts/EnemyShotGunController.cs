using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShotGunController : EnemyController
{
    
    [SerializeField] private float _shootAngel = 20;
    protected override void Shoot()
    {
        if (Time.time - _lastFireTime <= _fireTemp || _currentState == State.ROAMING || _currentState == State.RETREAT)
            return;

        var bullet = BulletPool.Instance.GetBulletFromPool(1);
        var leftBullet = BulletPool.Instance.GetBulletFromPool(1);
        var rightBullet = BulletPool.Instance.GetBulletFromPool(1);

        var sPosition = _shootingPoint.transform.position;

        Vector3 direction = (_playerController.transform.position - sPosition).normalized;
        Vector3 leftDirection = Quaternion.AngleAxis(_shootAngel, Vector3.forward) * direction;
        Vector3 rightDirection = Quaternion.AngleAxis(-_shootAngel, Vector3.forward) * direction;

        bullet.transform.position = sPosition;
        bullet.Enable(direction.normalized, _firePower, damageToDeal);

        leftBullet.transform.position = sPosition;
        leftBullet.Enable(leftDirection.normalized, _firePower, damageToDeal);

        rightBullet.transform.position = sPosition;
        rightBullet.Enable(rightDirection.normalized, _firePower, damageToDeal);

        _lastFireTime = Time.time;

    }
}
