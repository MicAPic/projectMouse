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
    private Vector3 _defaultShootingPointPos;
    private Vector3 _reversedShootingPointPos;
    private float _lastFireTime;

    [Header("Visuals & Animation")]
    public bool isFlashing;
    [SerializeField]
    private Color powerUpFlashingColour;
    [SerializeField]
    private Material invincibilityMaterial;
    private Material _defaultMaterial;
    
    [SerializeField]
    private SpriteRenderer dropShadow;
    private List<SpriteRenderer> _trailElementSprites  = new();
    private List<Animator> _trailElementAnimators  = new(); 
    private SpriteRenderer _sprite;
    private Animator _animator;
    private bool _isInvincible;
    private bool _isMoving;
    private bool _isAttacking;
    
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    
    [Header("Layers")]
    [SerializeField] 
    private int enemyLayer = 6;
    [SerializeField] 
    private int enemyBulletLayer = 8;
    private int _playerLayer;
    
    [Header("Input")]
    public PlayerInput playerInput;
    
    private Rigidbody2D _rb;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _defaultMaterial = _sprite.material;
        _animator = GetComponentInChildren<Animator>();

        _playerLayer = gameObject.layer;
        
        foreach (var trailElement in FindObjectsOfType<TrailElement>())
        {
            _trailElementSprites.Add(trailElement.GetComponent<SpriteRenderer>());
            _trailElementAnimators.Add(trailElement.GetComponent<Animator>());
        }

        _defaultShootingPointPos = shootingPoint.localPosition;
        _reversedShootingPointPos = new Vector3(-_defaultShootingPointPos.x, _defaultShootingPointPos.y, 0);
    }

    // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }

    // Update is called once per frame
    void Update()
    {
        // Animation
        // a big block of semi-repeated code, it's this way to not set Animators every Update
        if (playerInput.actions["Move"].WasPressedThisFrame())
        {
            _animator.SetBool(IsMoving, true);
            foreach (var trailElementAnimator in _trailElementAnimators)
            {
                trailElementAnimator.SetBool(IsMoving, true);
            }
        }
        else if (playerInput.actions["Move"].WasReleasedThisFrame() || !playerInput.enabled)
        {
            _animator.SetBool(IsMoving, false);
            foreach (var trailElementAnimator in _trailElementAnimators)
            {
                trailElementAnimator.SetBool(IsMoving, false);
            }
        }
        if (playerInput.actions["Shoot"].WasPressedThisFrame())
        {
            _animator.SetBool(IsAttacking, true);
            foreach (var trailElementAnimator in _trailElementAnimators)
            {
                trailElementAnimator.SetBool(IsAttacking, true);
            }
        }
        else if (playerInput.actions["Shoot"].WasReleasedThisFrame() || !playerInput.enabled)
        {
            _animator.SetBool(IsAttacking, false);
            foreach (var trailElementAnimator in _trailElementAnimators)
            {
                trailElementAnimator.SetBool(IsAttacking, false);
            }
        }
        
        if (GameManager.isGameOver || GameManager.isPaused) return;

        // Input processing
        if (playerInput.actions["Shoot"].IsPressed() && Time.time - _lastFireTime >= fireRate)
        {
            Shoot();
        }
        if (playerInput.actions["Dodge"].WasPressedThisFrame())
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
        var spriteFlipCheck = CameraController.Instance.mousePos.x < transform.position.x;
        if (_sprite.flipX == spriteFlipCheck) return;
        _sprite.flipX = spriteFlipCheck;
        foreach (var trailElement in _trailElementSprites)
        {
            trailElement.flipX = spriteFlipCheck;
        }

        shootingPoint.localPosition = spriteFlipCheck ? _reversedShootingPointPos : _defaultShootingPointPos;
    }

    void FixedUpdate()
    {
        // Movement
        _rb.velocity = _movementValue * movementSpeed;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Contact damage to enemies
        if (col.gameObject.TryGetComponent(out EnemyHealth enemyHealth) && _isDodging)
        {
            enemyHealth.TakeDamage(dodgeDamage);
        }
    }
    
    void OnMove(InputValue value)
    {
        _cachedMovementValue = value.Get<Vector2>();
        if (_isDodging)
        {
            return;
        }
        _movementValue = _cachedMovementValue;
    }
    
    public void ToggleInvincibilityMaterial()
    {
        _isInvincible = !_isInvincible;
        _sprite.material = _isInvincible ? invincibilityMaterial : _defaultMaterial;
    }
    
    public void ActivateFlashing(float duration, int noOfFlashes=3, bool isPowerUp=true)
    {
        isFlashing = true;
        var individualFlashTime = duration / noOfFlashes;
        var flashingSequence = DOTween.Sequence();
        
        var flashingColour = isPowerUp ? powerUpFlashingColour : Color.clear;
        var defaultMatColour = isPowerUp ? invincibilityMaterial.color : Color.white;
        var trailDefaultColours = new Color[3];
        
        if (isPowerUp)
        {
            // Colour the sprite gray; pulsate
            flashingSequence.AppendCallback(() => _sprite.material.color = flashingColour);
            flashingSequence.Append(_sprite.material.DOColor(defaultMatColour, individualFlashTime));
        }
        else
        {
            // Colour the sprite Color.clear; flash
            for (var i = 0; i < _trailElementSprites.Count; i++)
            {
                trailDefaultColours[i] = _trailElementSprites[i].color;
            }
            
            flashingSequence.AppendCallback(() =>
            {
                _sprite.material.color = flashingColour;
                for (var i = 0; i < _trailElementSprites.Count; i++)
                {
                    _trailElementSprites[i].color = flashingColour;
                }
                dropShadow.gameObject.SetActive(false);
            });
            flashingSequence.AppendInterval(individualFlashTime / 2);
            flashingSequence.AppendCallback(() =>
            {
                _sprite.material.color = defaultMatColour;
                for (var i = 0; i < _trailElementSprites.Count; i++)
                {
                    _trailElementSprites[i].color = trailDefaultColours[i];
                }
                dropShadow.gameObject.SetActive(true);
            });
            flashingSequence.AppendInterval(individualFlashTime / 2);
        }
        flashingSequence.SetLoops(noOfFlashes);

        flashingSequence.OnComplete(() =>
        {
            isFlashing = false;
            if (isPowerUp) return;

            for (var i = 0; i < _trailElementSprites.Count; i++)
            {
                _trailElementSprites[i].color = trailDefaultColours[i];
            }
        });
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
