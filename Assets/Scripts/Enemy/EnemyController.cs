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

        protected float _playerDistance;

        protected enum State
        {
            PATROL,
            CHASE,
            ATTACK,
            RETREAT,
        }

        protected State _currentState;

        [Header("Chasing")]
        [SerializeField] private float _chasingDistance = 15f;
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
            _playerController = FindAnyObjectByType<PlayerController>();
        }

        protected virtual void Start()
        {
            _currentState = State.PATROL;
            damageToDeal = SpawnManager.Instance != null ? SpawnManager.Instance.GetEnemyDamage() : 1.0f;
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
            switch (_currentState)
            {
                case State.PATROL:
                    _rigidbody.velocity = (Vector2)(_playerController.transform.position - transform.position).normalized * _chasingMovementSpeed;
                    if (_playerDistance < _chasingDistance)
                    {
                        _currentState = State.CHASE;
                    }
                    break;
                case State.CHASE:
                    _rigidbody.velocity = (Vector2)(_playerController.transform.position - transform.position).normalized * _chasingMovementSpeed;
                    if (_playerDistance < _attackDistance)
                    {
                        _rigidbody.velocity = Vector2.zero;
                        _currentState = State.ATTACK;
                    }
                    if (_playerDistance > _chasingDistance)
                        _currentState = State.PATROL;
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
            if (Time.time - _lastFireTime <= _fireTemp || _currentState is State.RETREAT or State.PATROL)
                return;

            var bullet = BulletPool.Instance.GetBulletFromPool(1);
            var sPosition = _shootingPoint.transform.position;

            Vector3 direction = (_playerController.transform.position - sPosition).normalized;

            bullet.transform.position = sPosition;
            bullet.Enable(direction, _firePower, damageToDeal);
            _lastFireTime = Time.time;
        }
    }
}
