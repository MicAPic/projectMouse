using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public abstract class EnemyController : MonoBehaviour
    {
        public float experiencePointWorth = 10f;
        public float damageToDeal = 34f;
        
        [SerializeField] protected Transform _shootingPoint;
        [SerializeField] protected float _firePower = 20;
        protected Rigidbody2D _rigidbody;

        private PointEffector2D _pointEffector;

        [Header("Animation & Visuals")]
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private static readonly int Attack = Animator.StringToHash("Attack");

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
            _pointEffector = GetComponentInChildren<PointEffector2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();
            
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
            _startTargetDirection = (transform.position - PlayerController.Instance.transform.position).normalized * _attackDistance;

        }
    
        // Update is called once per frame
        protected virtual void Update()
        {
            _playerDistance = Vector2.Distance(PlayerController.Instance.transform.position, transform.position);
            CheckToShoot();
        }

        private void FixedUpdate()
        {
            Move();
        }

        protected float _lastFireTime = 0f;
        protected abstract void Shoot();

        protected bool _firstShoot = true;
        protected float _blunderAngel = 30;
        private void CheckToShoot()
        {
            if (Time.time - _lastFireTime <= _fireTemp || 
                _currentState is State.RETREAT || !_isVisible)
            {
                return;
            }

            Shoot();
            _animator.SetTrigger(Attack);
        }

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
                    _rigidbody.velocity = (PlayerController.Instance.transform.position - transform.position).normalized * -_retreatSpeed;
                    break;
            }

            // Flip the sprite towards Mouse
            _spriteRenderer.flipX = _rigidbody.velocity.x < 0;
        }

        private float _targetPositionRotationSpeed = 100f;

        private bool _isVisible = false;

        private void OnBecameVisible()
        {
            _isVisible = true;
        }

        private void OnBecameInvisible()
        {
            _isVisible = false;
        }

        private void RotateTargetPositionVector()
        {
            _startTargetDirection = Quaternion.AngleAxis(
                _randRotationDir * _targetPositionRotationSpeed * Time.deltaTime, 
                PlayerController.Instance.transform.forward) * _startTargetDirection;
        }

        private Vector3 FindTargetPosition()
        {
            return PlayerController.Instance.transform.position + _startTargetDirection;
        }
    }
}
