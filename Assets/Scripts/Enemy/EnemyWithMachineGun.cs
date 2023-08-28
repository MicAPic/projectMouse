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
            for (int i = 0; i < _numberOfBulletsInBurst; ++i)
            {
                StartCoroutine(BulletShot(_timeBetweenShoots * i, _firstShoot));
            }
            _lastFireTime = Time.time + _numberOfBulletsInBurst * _timeBetweenShoots;
            _firstShoot = false;
        }

        private IEnumerator BulletShot(float offset, bool firstShoot)
        {
            yield return new WaitForSeconds(offset);

            var bullet = BulletPool.Instance.GetBulletFromPool(1);
            var sPosition = shootingPoint.transform.position;

            bullet.transform.position = sPosition;

            Vector3 direction = (PlayerController.Instance.transform.position - sPosition).normalized;
            if (firstShoot)
                direction = Quaternion.AngleAxis(_blunderAngel, Vector3.forward) * direction;

            bullet.Enable(direction, _firePower, damageToDeal);
        }


    }
}
