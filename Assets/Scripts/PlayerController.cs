using System.Collections.Generic;
using DG.Tweening;
using HealthControllers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Physics & Movement")] 
    public float movementSpeed;
    public float dodgeSpeedModifier;
    public float dodgeAccelerationTime;
    public float dodgeDecelerationTime;
    public float dodgeDamage;

    // grace period after pressing dodge where a dodge will be automatically performed once the requirements are met:
    private const float DodgeInputBufferTime = 0.15f;
    
    private float _lastDodgePressedTime;
    private bool _isDodging;
    private Vector2 _movementValue;
    private Vector2 _cachedMovementValue;

    [Header("Shooting")] 
    public float fireRate = 1.0f;
    public float firePower = 1.0f;
    public float damageToDeal = 34f;
    public float bulletScaleModifier = 1.0f;
    
    [SerializeField]
    private Transform shootingPoint;
    private float _lastFireTime;

    [Header("Animation")]
    [SerializeField]
    private Material invincibilityMaterial;
    private Material _defaultMaterial;
    private List<SpriteRenderer> _trailElementSprites  = new();
    private SpriteRenderer _sprite;
    private bool _isInvincible;
    
    [Header("Layers")]
    [SerializeField] 
    private int enemyLayer = 6;
    [SerializeField] 
    private int enemyBulletLayer = 8;
    private int _playerLayer;
    
    private PlayerInput _playerInput;
    private Rigidbody2D _rb;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _defaultMaterial = _sprite.material;

        _playerLayer = gameObject.layer;
        
        foreach (var trailElement in FindObjectsOfType<TrailElement>())
        {
            _trailElementSprites.Add(trailElement.GetComponent<SpriteRenderer>());
        }
    }

    // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameOver) return;
        
        // Input processing
        if (_playerInput.actions["Shoot"].IsPressed() && Time.time - _lastFireTime >= fireRate)
        {
            Shoot();
        }
        if (_playerInput.actions["Dodge"].WasPressedThisFrame())
        {
            _lastDodgePressedTime = Time.time;
        }

        // Dodge
        if (!_isDodging && Time.time - _lastDodgePressedTime <= DodgeInputBufferTime)
        {
            Dodge();
        }
        
        // Rotate the shooting point
        shootingPoint.right = Mouse.current.position.ReadValue() - (Vector2)shootingPoint.position;
        
        // Flip the sprite towards the mouse
        var spriteFlipX = CameraController.Instance.mousePos.x < transform.position.x;
        _sprite.flipX = spriteFlipX;
        foreach (var trailElement in _trailElementSprites)
        {
            trailElement.flipX = spriteFlipX;
        }
    }

    void FixedUpdate()
    {
        // Movement
        _rb.velocity = _movementValue * movementSpeed;
    }

    public void ToggleInvincibilityMaterial()
    {
        _isInvincible = !_isInvincible;
        _sprite.material = _isInvincible ? invincibilityMaterial : _defaultMaterial;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent(out EnemyHealth enemyHealth) && _isDodging)
        {
            enemyHealth.TakeDamage(dodgeDamage);
        }
    }

    private void OnMove(InputValue value)
    {
        _cachedMovementValue = value.Get<Vector2>();
        if (_isDodging)
        {
            return;
        }
        _movementValue = _cachedMovementValue;
    }

    private void Shoot()
    {
        var bullet = BulletPool.Instance.GetBulletFromPool(0);
        var sPosition = shootingPoint.transform.position;
        var direction = CameraController.Instance.mousePos - sPosition;
        
        bullet.transform.position = sPosition;
        bullet.Enable(direction, firePower, damageToDeal, bulletScaleModifier);
        
        _lastFireTime = Time.time;
    }

    private void Dodge()
    {
        if (_movementValue == Vector2.zero) return;
        // Debug.Log("Dodged");
        
        _isDodging = true;
        Physics2D.IgnoreLayerCollision(_playerLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(_playerLayer, enemyBulletLayer, true);
        
        var dodgeSequence = DOTween.Sequence();
        dodgeSequence.Append(DOTween.To(
            () => _movementValue,
            x => _movementValue = x,
            _movementValue * dodgeSpeedModifier,
            dodgeAccelerationTime)
            .OnComplete(() =>
            {
                Physics2D.IgnoreLayerCollision(_playerLayer, enemyLayer, false);
                Physics2D.IgnoreLayerCollision(_playerLayer, enemyBulletLayer, false);
            }
        ));
        dodgeSequence.Append(DOTween.To(
            () => _movementValue,
            x => _movementValue = x,
            _cachedMovementValue,
            dodgeDecelerationTime)
            .OnComplete(() => 
            {
                _isDodging = false;
                _movementValue = _cachedMovementValue;
            }
        ));
    }
}
