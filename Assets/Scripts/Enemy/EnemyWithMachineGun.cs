using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy
{
    public class EnemyWithMachineGun : EnemyController
    {
        [Header("BulletsSetting")]
        [SerializeField] private int _numberOfBulletsInBurst = 6;
        [SerializeField] private float _timeBetweenShoots = 0.2f;
        protected override void Shoot()
        {
            if (Time.time - _lastFireTime <= _fireTemp ||
               _currentState is State.RETREAT or State.PATROL or State.BEHINDCAMERA)
            {
                return;
            }

            for(int i = 0; i < _numberOfBulletsInBurst; ++i)
            {
                StartCoroutine(BulletShot(_timeBetweenShoots * i));
            }
            _lastFireTime = Time.time + _numberOfBulletsInBurst * _timeBetweenShoots;

        }

        private IEnumerator BulletShot(float offset)
        {
            yield return new WaitForSeconds(offset);

            var bullet = BulletPool.Instance.GetBulletFromPool(1);
            var sPosition = _shootingPoint.transform.position;

            bullet.transform.position = sPosition;

            Vector3 direction = (PlayerController.Instance.transform.position - sPosition).normalized;

            bullet.Enable(direction, _firePower, damageToDeal);
        }


    }
}
