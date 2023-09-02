using Bullets;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyShotGunController : EnemyController
    {
        [Header("Shotgun Properties")]
        [FormerlySerializedAs("_shootAngel")] 
        [SerializeField] 
        private float shootAngle = 20;
        
        protected override void Shoot()
        {
            var sPosition = shootingPoint.transform.position;

            var directions = new Vector3[3];
            // forward:
            directions[0] = PlayerController.Instance.transform.position - sPosition;
            if (_firstShoot)
            {
                directions[0] = Quaternion.AngleAxis(_blunderAngel, Vector3.forward) * directions[0];
                _firstShoot = false;
            }
            // left:
            directions[1] = Quaternion.AngleAxis(Random.Range(5f, shootAngle), Vector3.forward) * directions[0];
            // right:
            directions[2] = Quaternion.AngleAxis(-Random.Range(5f, shootAngle), Vector3.forward) * directions[0];

            Bullet bullet;
            foreach (var direction in directions)
            {
                bullet = BulletPool.Instance.GetBulletFromPool(1);
                bullet.transform.position = sPosition;
                bullet.Enable(direction, _firePower, damageToDeal);
            }
            base.Shoot(); // play sfx

            _lastFireTime = Time.time;
        }
    }
}
