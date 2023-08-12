using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        _mainRotationVector = Vector3.up * _radius;
        for (int i = 0; i < _bullets.Count; ++i)
        {
            Vector3 offset = Quaternion.Euler(0,0, (360 / _numberOfBullets) * i) * _mainRotationVector;
            _bullets[i] = BulletPool.Instance.GetBulletFromPool(1);
            _bullets[i].transform.position = transform.position + offset;
            _bullets[i].gameObject.SetActive(true);
        }
    }
    private void RotateBullets()
    {
        _mainRotationVector = Quaternion.AngleAxis(_bulletsRotationSpeed*Time.deltaTime, Vector3.forward) * _mainRotationVector;
        for (int i = 0; i < _bullets.Count; ++i)
        {
            Vector3 offset = Quaternion.Euler(0, 0, (360 / _numberOfBullets) * i) * _mainRotationVector;
            if(_bullets[i] != null)
                _bullets[i].transform.position = transform.position + offset;
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
        if (Time.time - _lastFireTime <= _fireTemp || _currentState == State.ROAMING || _currentState == State.RETREAT)
            return;
        if (_currentBulletIndex >= _bullets.Count)
        {
            SetUpBullets();
            _currentBulletIndex = 0;
        }
        else
        {
            Vector3 direction = (_playerController.transform.position - _bullets[_currentBulletIndex].transform.position).normalized;
            _bullets[_currentBulletIndex].Enable(direction.normalized, _firePower, damageToDeal);
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
                Vector3 direction = (_playerController.transform.position - _bullets[i].transform.position).normalized;
                _bullets[i].Enable(direction.normalized, _firePower, damageToDeal);
            }
        }
    }
}
