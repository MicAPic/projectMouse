using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {

        public float experiencePointWorth = 10f;
        public float damageToDeal = 34f;

        protected PlayerController _playerController;
        [SerializeField] protected Transform _shootingPoint;
        [SerializeField] protected float _firePower = 20;
        protected Rigidbody2D _rigidbody;

        private PointEffector2D _pointEffector;

        protected float _playerDistance;
        protected enum State
        {
            BEHINDCAMERA,
            PATROL,
            CHASE,
            ATTACK,
            RETREAT,
        }

        protected State _currentState;

        [Header("Chasing")]
        [SerializeField] private float _behindCameraSpeed = 30;
        [SerializeField] private float _behindCameraDistance = 35;

        [Header("Chasing")]
        [SerializeField] private float _chasingDistance = 20f;
        [SerializeField] private float _chasingMovementSpeed;

        [Header("Attacking")]
        [SerializeField] private float _attackDistance = 15f;
        [SerializeField] protected float _fireTemp = 3f;

        [Header("Retreating")]
        [FormerlySerializedAs("_retrateDistance")]
        [SerializeField] 
        private float _retreatDistance = 5f;
        [FormerlySerializedAs("_retrateSpeed")] 
        [SerializeField]
        private float _retreatSpeed = 20f;



        private Vector3 _startTargetDirection;
    
        protected virtual void Awake()
        {
            _pointEffector = GetComponent<PointEffector2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerController = FindAnyObjectByType<PlayerController>();
        }

        private int _randRotationDir;
        protected virtual void Start()
        {
            if (Random.value < 0.5)
                _randRotationDir = -1;
            else
                _randRotationDir = 1;
            _currentState = State.BEHINDCAMERA;

            damageToDeal = SpawnManager.Instance != null ? SpawnManager.Instance.GetEnemyDamage() : 30.0f;
            _startTargetDirection = (transform.position - _playerController.transform.position).normalized * _attackDistance;

        }
    
        // Update is called once per frame
        protected virtual void Update()
        {
            _playerDistance = Vector2.Distance(_playerController.transform.position, transform.position);
            Shoot();
        }

        private void FixedUpdate()
        {
            Move();
        }


        protected float _lastFireTime = 0f;



        private void Move()
        {
            RotateTargetPositionVector();
            Vector3 _currentTargetPosition = FindTargetPosition();
            switch (_currentState)
            {
                case State.BEHINDCAMERA:
                    _rigidbody.velocity = (_currentTargetPosition - transform.position).normalized * _behindCameraSpeed;
                    if(_playerDistance < _behindCameraDistance)
                    {
                        _currentState = State.PATROL;
                    }
                    break;
                case State.PATROL:
                    _rigidbody.velocity = (_currentTargetPosition - transform.position).normalized * _chasingMovementSpeed;
                    if (_playerDistance < _chasingDistance)
                    {
                        _currentState = State.CHASE;
                    }
                    if(_playerDistance > _behindCameraDistance)
                    {
                        _currentState = State.BEHINDCAMERA;
                    }
                    break;
                case State.CHASE:
                    _rigidbody.velocity = (_currentTargetPosition - transform.position).normalized * _chasingMovementSpeed;
                    if (_playerDistance < _attackDistance)
                    {
                        _rigidbody.velocity = Vector2.zero;
                        _currentState = State.ATTACK;
                        _pointEffector.enabled = false;
                    }
                    if (_playerDistance > _chasingDistance)
                        _currentState = State.PATROL;
                    break;
                case State.ATTACK:
                    //_rigidbody.velocity = (_currentTargetPosition - transform.position).normalized * _chasingMovementSpeed;
                    if (_playerDistance > _attackDistance)
                    {
                        _currentState = State.CHASE;
                        _pointEffector.enabled = true;
                    }
                    if (_playerDistance < _retreatDistance)
                        _currentState = State.RETREAT;
                    break;
                case State.RETREAT:
                    if (_playerDistance > _retreatDistance)
                    {
                        _currentState = State.ATTACK;
                        _rigidbody.velocity = Vector2.zero;
                        break;
                    }
                    _rigidbody.velocity = (_playerController.transform.position - transform.position).normalized * -_retreatSpeed;
                    break;
            }
        }

        private float _targetPositionRotationSpeed = 100f;

        private void RotateTargetPositionVector()
        {
            _startTargetDirection = Quaternion.AngleAxis(_randRotationDir * _targetPositionRotationSpeed * Time.deltaTime, _playerController.transform.forward) * _startTargetDirection;
        }
        private Vector3 FindTargetPosition()
        {
            return _playerController.transform.position + _startTargetDirection;
        }
    
        protected virtual void Shoot()
        {
            if (Time.time - _lastFireTime <= _fireTemp || 
                _currentState is State.RETREAT or State.PATROL or State.BEHINDCAMERA)
            {
                return;
            }

            var bullet = BulletPool.Instance.GetBulletFromPool(1);
            var sPosition = _shootingPoint.transform.position;

            Vector3 direction = (_playerController.transform.position - sPosition).normalized;

            bullet.transform.position = sPosition;
            bullet.Enable(direction, _firePower, damageToDeal);
            _lastFireTime = Time.time;
        }


       

    }
}
