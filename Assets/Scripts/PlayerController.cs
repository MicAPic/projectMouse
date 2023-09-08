using System.Collections.Generic;
using DG.Tweening;
using HealthControllers;
using UnityEngine;
using UnityEngine.InputSystem;
using Bullets;
using System.Collections;
using Audio;
using UI;
using UnityEngine.Serialization;

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
    private bool activateAimOnLoad = true;
    
    [SerializeField]
    private Transform shootingPoint;
    private Vector3 _defaultShootingPointPos;
    private Vector3 _reversedShootingPointPos;
    private float _lastFireTime;
    
    [Header("Shooting/Shotgun")]
    [FormerlySerializedAs("_shootAngel")]
    [SerializeField] 
    private float shootAngle = 20;
    public bool ShotgunPowerUpEnabled { get; private set; }
    
    [Header("Shooting/Magic Bullets")]
    [FormerlySerializedAs("_numberOfBullets")]
    [SerializeField] 
    private int numberOfBullets = 3;
    [FormerlySerializedAs("_radius")]
    [SerializeField] 
    private int radius = 2;
    [FormerlySerializedAs("_bulletsRotationSpeed")]
    [SerializeField] 
    private float bulletsRotationSpeed = 2f;
    public bool MagicBulletsEnabled { get; private set; } = false;
    private List<Bullet> _bullets;
    private Vector3 _mainRotationVector;

    [Space(10)]
    
    [Header("Audio")]
    [SerializeField]
    private AudioClip shootingSoundEffect;
    [SerializeField]
    private AudioClip dodgeSoundEffect;
    [SerializeField]
    private AudioClip invincibilityEndSoundEffect;

    [Header("Visuals & Animation")]
    public float appearanceDuration = 1.0f;
    [SerializeField]
    private SpriteRenderer shadowRenderer;

    public bool isFlashing;
    [SerializeField]
    private Color powerUpFlashingColour;
    [SerializeField]
    private Material invincibilityMaterial;
    [SerializeField]
    private Material dissolveMaterial;
    private Material _defaultMaterial;
    
    [SerializeField]
    private SpriteRenderer dropShadow;
    private List<SpriteRenderer> _trailElementSprites  = new();
    private List<Animator> _trailElementAnimators  = new(); 
    
    // Magic bullets / shield animation
    private readonly float _animTimeScaler = 3;
    private float _timeFromStartBulletAnim;
    private bool _bulletSetUpStarted = false;
    //

    public bool isInvincible;
    public Animator animator;
    private SpriteRenderer _sprite;
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
        animator = GetComponentInChildren<Animator>();

        _playerLayer = gameObject.layer;
        
        foreach (var trailElement in FindObjectsOfType<TrailElement>())
        {
            _trailElementSprites.Add(trailElement.GetComponent<SpriteRenderer>());
            _trailElementAnimators.Add(trailElement.GetComponent<Animator>());
        }

        _defaultShootingPointPos = shootingPoint.localPosition;
        _reversedShootingPointPos = new Vector3(-_defaultShootingPointPos.x, _defaultShootingPointPos.y, 0);
    }

    void Start()
    {
        _bullets = new List<Bullet>(numberOfBullets);
        for (int i = 0; i < numberOfBullets; ++i)
            _bullets.Add(null);

        CameraController.Instance.focusPoint = 0.0f;
        
        shadowRenderer.DOFade(1.0f, appearanceDuration * 0.5f)
            .SetDelay(TransitionController.Instance.transitionDuration * 0.5f);
        _sprite.material = dissolveMaterial;
        _sprite.material.SetFloat("_Threshold", 1.01f);
        _sprite.material.DOFloat(0.0f, "_Threshold", appearanceDuration)
            .SetDelay(TransitionController.Instance.transitionDuration * 0.5f)
            .OnComplete(() =>
            {
                foreach (var trail in _trailElementSprites)
                {
                    trail.enabled = true;
                }
                _sprite.material = _defaultMaterial;
                if (activateAimOnLoad)
                    CameraController.Instance.focusPoint = CameraController.Instance.defaultFocusPoint;
            });
    }

    // Update is called once per frame
    void Update()
    {
        // Animation
        // a big block of semi-repeated code, it's this way to not set Animators every Update
        if (playerInput.actions["Move"].WasPressedThisFrame())
        {
            animator.SetBool(IsMoving, true);
            foreach (var trailElementAnimator in _trailElementAnimators)
            {
                trailElementAnimator.SetBool(IsMoving, true);
            }
        }
        else if (playerInput.actions["Move"].WasReleasedThisFrame() || !playerInput.enabled)
        {
            animator.SetBool(IsMoving, false);
            foreach (var trailElementAnimator in _trailElementAnimators)
            {
                trailElementAnimator.SetBool(IsMoving, false);
            }
        }
        if (playerInput.actions["Shoot"].WasPressedThisFrame())
        {
            animator.SetBool(IsAttacking, true);
            foreach (var trailElementAnimator in _trailElementAnimators)
            {
                trailElementAnimator.SetBool(IsAttacking, true);
            }
        }
        else if (playerInput.actions["Shoot"].WasReleasedThisFrame() || !playerInput.enabled)
        {
            animator.SetBool(IsAttacking, false);
            foreach (var trailElementAnimator in _trailElementAnimators)
            {
                trailElementAnimator.SetBool(IsAttacking, false);
            }
        }
        
        if (GameManager.IsGameOver || GameManager.IsPaused) return;

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
        
        // Handle the bullet shield
        if (MagicBulletsEnabled)
        {
            RotateBullets();
            MagicBulletsEnableCheck();
            MagicBulletNullCheck();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Contact damage to enemies
        if (collision.gameObject.TryGetComponent(out EnemyHealth enemyHealth) && _isDodging && dodgeDamage > 0 && collision.isTrigger)
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
        isInvincible = !isInvincible;
        _sprite.material = isInvincible ? invincibilityMaterial : _defaultMaterial;
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
            AudioManager.Instance.sfxSource.PlayOneShot(invincibilityEndSoundEffect);
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

    public void EnableShotgun()
    {
        ShotgunPowerUpEnabled = true;
    }

    private int _bulletsInShootGun = 2;
    public void AddShotgunBullet()
    {
        ++_bulletsInShootGun;
    }
    
    public void EnableMagicBullets()
    {
        MagicBulletsEnabled = true;
        SetUpBullets();
    }

    private int _numOfMagicBullets = 3;
    public void AddMagicBullet()
    {
        ++_numOfMagicBullets;
        _bullets.Add(null);
    }

    private void Shoot()
    {
        var bullet = BulletPool.Instance.GetBulletFromPool(0);
        var sPosition = shootingPoint.transform.position;
        var direction = CameraController.Instance.mousePos - sPosition;
        
        bullet.transform.position = sPosition;
        bullet.Enable(direction, firePower, damageToDeal, bulletScaleModifier);

        if (ShotgunPowerUpEnabled)
        {
            var additionalDirections = new Vector3[_bulletsInShootGun];
            additionalDirections[0] = Quaternion.AngleAxis(shootAngle, Vector3.forward) * direction; // left
            additionalDirections[1] = Quaternion.AngleAxis(-shootAngle, Vector3.forward) * direction; // right
            for(int i = 2; i < _bulletsInShootGun; ++i)
            {
                if(i % 2 == 0)
                    additionalDirections[i] = Quaternion.AngleAxis(shootAngle/(i+2 / 2), Vector3.forward) * direction;
                else
                    additionalDirections[i] = Quaternion.AngleAxis(-shootAngle/(i+2 / 2), Vector3.forward) * direction;
            }

            foreach (var additionalDirection in additionalDirections)
            {
                bullet = BulletPool.Instance.GetBulletFromPool(0);
                bullet.transform.position = sPosition;
                bullet.Enable(additionalDirection, firePower, damageToDeal, bulletScaleModifier);
            }
        }
        
        AudioManager.Instance.sfxSource.PlayOneShot(shootingSoundEffect);
        _lastFireTime = Time.time;
    }

    private void Dodge()
    {
        if (_movementValue == Vector2.zero) return;

        _isDodging = true;
        AudioManager.Instance.sfxSource.PlayOneShot(dodgeSoundEffect);
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

    // Magic shield-related methods:
    private void SetUpBullets()
    {
        numberOfBullets = _numOfMagicBullets;
        for (int i = 0; i < numberOfBullets; ++i)
        {
            _bullets[i] = BulletPool.Instance.GetBulletFromPool(0);
            _bullets[i].transform.position = transform.position/* + offset*/;
            _bullets[i].EnableWithoutForce(damageToDeal);
        }
        StartCoroutine(AnimateBulletsInst());
    }
    
    private IEnumerator AnimateBulletsInst()
    {
        _mainRotationVector = Vector3.up * radius;
        while (_timeFromStartBulletAnim / _animTimeScaler < 1)
        {
            for (int i = 0; i < numberOfBullets; ++i)
            {
                Vector3 offset = Quaternion.Euler(0, 0, 360 / numberOfBullets * i) * _mainRotationVector;
                if (_bullets[i] != null)
                {
                    var position = transform.position;
                    _bullets[i].transform.position = Vector3.Lerp(position, position + offset, _timeFromStartBulletAnim / _animTimeScaler);
                }

                _timeFromStartBulletAnim += Time.deltaTime;
            }
            yield return null;
        }

        for (int i = 0; i < numberOfBullets; ++i)
        {
            Vector3 offset = Quaternion.Euler(0, 0, 360 / numberOfBullets * i) * _mainRotationVector;
            if (_bullets[i] != null)
                _bullets[i].transform.position = transform.position + offset;
        }

        _timeFromStartBulletAnim = 0;
    }

    private void RotateBullets()
    {
        _mainRotationVector = Quaternion.AngleAxis(bulletsRotationSpeed * Time.deltaTime, Vector3.forward) * _mainRotationVector;
        for (int i = 0; i < numberOfBullets; ++i)
        {
            Vector3 offset = Quaternion.Euler(0, 0, 360 / numberOfBullets * i) * _mainRotationVector;
            if (_bullets[i] != null)
                _bullets[i].transform.position = transform.position + offset;
        }
    }
    
    private void MagicBulletsEnableCheck()
    {
        for (int i = 0; i < numberOfBullets; ++i)
        {
            if (_bullets[i] != null && !_bullets[i].gameObject.activeSelf)
                _bullets[i] = null;
        }
    }

    private void MagicBulletNullCheck()
    {
        if (_bulletSetUpStarted) return;
        for (int i = 0; i < numberOfBullets; ++i)
        {
            if (_bullets[i] != null)
                return;
        }
        StartCoroutine(StartSetUpBullets());
        _bulletSetUpStarted = true;
    }

    private IEnumerator StartSetUpBullets()
    {
        yield return new WaitForSeconds(3);
        SetUpBullets();
        _bulletSetUpStarted = false;
    }
    //
}
