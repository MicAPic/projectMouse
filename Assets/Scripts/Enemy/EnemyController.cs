using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {

        public float experiencePointWorth = 10f;
        public float damageToDeal = 34f;

        //TODO: make PlayerController singleton OR assign this in Awake()
        [SerializeField] protected PlayerController _playerController;
        [SerializeField] protected Transform _shootingPoint;
        [SerializeField] protected float _firePower = 20;
        protected Rigidbody2D _rigidbody;

        protected float _playerDistance;

        protected enum State
        {
            ROAMING,
            CHASE,
            ATTACK,
            RETREAT,
        }

        protected State _currentState;

        [Header("Roaming")]

        [SerializeField] private float _roamingMovementSpeed;
        [SerializeField] private float _detectionDistance = 20f;
        [SerializeField] private float _minRoamingDistance = 5f;
        [SerializeField] private float _maxRoamingDistance = 15f;
        private Vector3 _currentRoamPosition;

        [Header("Chasing")]
        [SerializeField] private float _chasingMovementSpeed;

        [Header("Attacking")]
        [SerializeField] private float _attackDistance = 5f;
        [SerializeField] protected float _fireTemp = 3f;

        [Header("Retreating")]
        [FormerlySerializedAs("_retrateDistance")]
        [SerializeField] 
        private float _retreatDistance = 5f;
        [FormerlySerializedAs("_retrateSpeed")] 
        [SerializeField]
        private float _retreatSpeed = 20f;
    
        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        protected virtual void Start()
        {
            _currentState = State.ROAMING;
            _currentRoamPosition = GetNextRandomPosition();
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

        private Vector3 GetNextRandomPosition()
        {
            Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * Random.Range(_minRoamingDistance, _maxRoamingDistance);
            return transform.position + direction;
        }

        protected float _lastFireTime = 0f;

        private void Move()
        {
            switch (_currentState)
            {
                case State.ROAMING:
                    if (_playerDistance < _detectionDistance)
                    {
                        _currentState = State.CHASE;
                        break;
                    }        
                    _rigidbody.velocity = (Vector2)(_currentRoamPosition - transform.position).normalized * _roamingMovementSpeed; 
                    if (Vector3.Distance(transform.position, _currentRoamPosition) < 0.1f)
                        _currentRoamPosition = GetNextRandomPosition();
                    break;
                case State.CHASE:
                    _rigidbody.velocity = (Vector2)(_playerController.transform.position - transform.position).normalized * _chasingMovementSpeed;
                    if (_playerDistance > _detectionDistance)
                    {
                        _currentRoamPosition = GetNextRandomPosition();
                        _currentState = State.ROAMING;
                    }
                    if (_playerDistance < _attackDistance)
                    {
                        _rigidbody.velocity = Vector2.zero;
                        _currentState = State.ATTACK;
                    }
                    break;
                case State.ATTACK:
                    if (_playerDistance > _attackDistance)
                        _currentState = State.CHASE;
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
                    _rigidbody.velocity = (Vector2)(_playerController.transform.position - transform.position).normalized * -_retreatSpeed;
                    break;
            }
        }
    
        protected virtual void Shoot()
        {
            if (Time.time - _lastFireTime <= _fireTemp || _currentState is State.ROAMING or State.RETREAT)
                return;

            var bullet = BulletPool.Instance.GetBulletFromPool(1);
            var sPosition = _shootingPoint.transform.position;

            Vector3 direction = (_playerController.transform.position - sPosition).normalized;

            bullet.transform.position = sPosition;
            bullet.Enable(direction.normalized, _firePower, damageToDeal);
            _lastFireTime = Time.time;
        }
    }
}
