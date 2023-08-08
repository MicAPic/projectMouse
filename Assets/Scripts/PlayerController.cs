using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Physics & Movement")] public float movementSpeed;
    private Vector2 _movementValue;

    [Header("Shooting")] 
    public float fireRate = 1.0f;
    public float firePower = 1.0f;
    
    [SerializeField]
    private Transform shootingPoint;
    [SerializeField]
    private GameObject bulletPrefab;
    private float _lastFireTime;

    private PlayerInput _playerInput;
    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }

    // Update is called once per frame
    void Update()
    {
        if (_playerInput.actions["Shoot"].IsPressed() && Time.time - _lastFireTime >= fireRate)
        {
            Shoot();
        }
        
        // Rotate shooting point:
        shootingPoint.right = Mouse.current.position.ReadValue() - (Vector2)shootingPoint.position;
    }

    void FixedUpdate()
    {
        _rb.velocity = _movementValue * movementSpeed;
    }
    
    private void OnMove(InputValue value)
    {
        _movementValue = value.Get<Vector2>();
    }

    private void Shoot()
    {
        var bullet = BulletPool.Instance.GetBulletFromPool(0);
        var sPosition = shootingPoint.transform.position;
        var direction = CameraController.Instance.mousePos - sPosition;
        
        bullet.transform.position = sPosition;
        bullet.Enable(direction.normalized, firePower);
        
        _lastFireTime = Time.time;
    }

}
