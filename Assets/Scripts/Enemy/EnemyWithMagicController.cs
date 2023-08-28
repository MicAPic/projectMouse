using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Bullets;

namespace Enemy
{
    public class EnemyWithMagicController : EnemyController
    {
        [Header("BulletsSetting")]
        [SerializeField] private int _numberOfBullets = 3;
        [SerializeField] private int _radius = 2;
        [SerializeField] private float _bulletsRotationSpeed = 2f;
        private List<Bullet> _bullets;

        private Vector3 _mainRotationVector;
        protected override void Start()
        {
            base.Start();
            _bullets = new List<Bullet>(_numberOfBullets);
            for (int i = 0; i < _numberOfBullets; ++i)
                _bullets.Add(null);
            SetUpBullets();
        }

        private void SetUpBullets()
        {
            //_mainRotationVector = Vector3.up * _radius;
            for (int i = 0; i < _bullets.Capacity; ++i)
            {
                //Vector3 offset = Quaternion.Euler(0,0, (360 / _numberOfBullets) * i) * _mainRotationVector;
                _bullets[i] = BulletPool.Instance.GetBulletFromPool(1);
                _bullets[i].transform.position = shootingPoint.position/* + offset*/;
                _bullets[i].EnableWithoutForce(damageToDeal);
            }
            _bulletsAnimFinished = false;
            StartCoroutine(AnimateBulletsInst());
        }


        private float _timeFromStartBulletAnim = 0;
        private bool _bulletsAnimFinished;
        private float _animTimeScaler = 3;
        private IEnumerator AnimateBulletsInst()
        {
            _mainRotationVector = Vector3.up * _radius;
            while(_timeFromStartBulletAnim / _animTimeScaler < 1)
            {
                for (int i = 0; i < _bullets.Capacity; ++i)
                {
                    Vector3 offset = Quaternion.Euler(0, 0, (360 / _numberOfBullets) * i) * _mainRotationVector;
                    if (_bullets[i] != null) 
                        _bullets[i].transform.position = Vector3.Lerp(shootingPoint.position, shootingPoint.position + offset, _timeFromStartBulletAnim / _animTimeScaler);
                    _timeFromStartBulletAnim += Time.deltaTime;
                }
                yield return null;
            }


            for (int i = 0; i < _bullets.Capacity; ++i)
            {
                Vector3 offset = Quaternion.Euler(0, 0, (360 / _numberOfBullets) * i) * _mainRotationVector;
                if(_bullets[i] != null)
                    _bullets[i].transform.position = shootingPoint.position + offset;
            }

            _bulletsAnimFinished = true;
            _timeFromStartBulletAnim = 0;

        }

        private void RotateBullets()
        {
            if (_bulletsAnimFinished)
            {
                _mainRotationVector = Quaternion.AngleAxis(_bulletsRotationSpeed * Time.deltaTime, Vector3.forward) * _mainRotationVector;
                for (int i = 0; i < _bullets.Capacity; ++i)
                {
                    Vector3 offset = Quaternion.Euler(0, 0, (360 / _numberOfBullets) * i) * _mainRotationVector;
                    if (_bullets[i] != null)
                        _bullets[i].transform.position = transform.position + offset;
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            RotateBullets();

        }

        private int _currentBulletIndex = 0;
        protected override void Shoot()
        {
            if (_currentBulletIndex >= _bullets.Capacity)
            {
                SetUpBullets();
                _currentBulletIndex = 0;
            }
            else
            {
                if(_bullets[_currentBulletIndex] == null)
                {
                    ++_currentBulletIndex;
                    return;
                }
                if(!_bullets[_currentBulletIndex].gameObject.activeSelf)
                {
                    // Debug.Log("make bullet null");
                    _bullets[_currentBulletIndex] = null;
                    ++_currentBulletIndex;
                    return;
                }
                Vector3 direction = PlayerController.Instance.transform.position - _bullets[_currentBulletIndex].transform.position;
                if (_firstShoot)
                {
                    direction = Quaternion.AngleAxis(_blunderAngel, Vector3.forward) * direction;
                    _firstShoot = false;
                }
                _bullets[_currentBulletIndex].EnableBulletForce(direction, _firePower);
                _bullets[_currentBulletIndex] = null;
                ++_currentBulletIndex;
            }
            
            _lastFireTime = Time.time;
        }

        public void EnableAllBullets()
        {
            for (int i = 0; i < _bullets.Count; ++i)
            {
                if (_bullets[i] != null)
                {
                    Vector3 direction = PlayerController.Instance.transform.position - _bullets[i].transform.position;
                    _bullets[i].EnableBulletForce(direction, _firePower);
                }
            }
        }
    }
}
