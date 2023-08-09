using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public int experiencePointWorth;

    //TODO: make PlayerController singleton
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private float _firePower = 20;

    private float _playerDistance;

    private enum State
    {
        ROAMING,
        CHASE,
        ATTACK,
        RETRATE,
    }

    private State _currentState;


    [Header("Roaming")]

    [SerializeField] private float _roamingMovementSpeed;
    [SerializeField] private float _detectionDistance = 20f;
    [SerializeField] private float _minRoamingDistance = 5f;
    [SerializeField] private float _maxRoamingDistance = 15f;
    private Vector3 _currentRoamPosition;

    [Header("Chasing")]
    [SerializeField] private float _chasingMovementSpeed;

    [Header("Attack")]
    [SerializeField] private float _attackDistance = 5f;
    [SerializeField] private float _fireTemp = 3f;

    [Header("Retarte")]
    [SerializeField] private float _retrateDistance = 5f;
    [SerializeField ]private float _retrateSpeed = 20f;






    void Start()
    {
        _currentState = State.ROAMING;
        _currentRoamPosition = GetNextRandomPosition();
    }
    // Update is called once per frame
    void Update()
    {
        _playerDistance = Vector2.Distance(_playerController.transform.position, transform.position);
        switch (_currentState)
        {
            case State.ROAMING:
                if (_playerDistance < _detectionDistance)
                {
                    _currentState = State.CHASE;
                    break;
                }
                transform.position = Vector3.MoveTowards(transform.position, _currentRoamPosition, _roamingMovementSpeed * Time.deltaTime);
                if(Vector3.Distance(transform.position, _currentRoamPosition) < 0.1f)
                    _currentRoamPosition = GetNextRandomPosition();
                break;
            case State.CHASE:
                transform.position = Vector3.MoveTowards(transform.position, _playerController.transform.position, _chasingMovementSpeed * Time.deltaTime);
                if(_playerDistance > _detectionDistance)
                {
                    _currentRoamPosition = GetNextRandomPosition();
                    _currentState = State.ROAMING;
                }
                if(_playerDistance < _attackDistance)
                    _currentState = State.ATTACK;
                Shoot();
                break;
            case State.ATTACK:
                if (_playerDistance > _attackDistance)
                    _currentState = State.CHASE;
                if (_playerDistance < _retrateDistance)
                    _currentState = State.RETRATE;
                Shoot();
                break;
            case State.RETRATE:
                if (_playerDistance > _retrateDistance)
                    _currentState = State.ATTACK;
                transform.position = Vector3.MoveTowards(transform.position, _playerController.transform.position, -_retrateSpeed * Time.deltaTime);
                //Shoot();
                break;
        }
    }

    private Vector3 GetNextRandomPosition()
    {
        Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * Random.Range(_minRoamingDistance, _maxRoamingDistance);
        return transform.position + direction;
    }

    private float _lastFireTime = 0f;
    private void Shoot()
    {
        if (Time.time - _lastFireTime <= _fireTemp)
            return;

        var bullet = BulletPool.Instance.GetBulletFromPool(1);
        var sPosition = _shootingPoint.transform.position;

        Vector3 direction = (_playerController.transform.position - sPosition).normalized;

        bullet.transform.position = sPosition;
        bullet.Enable(direction.normalized, _firePower);
        _lastFireTime = Time.time;

    }



}
