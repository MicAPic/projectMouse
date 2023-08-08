using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Physics & Movement")]
    public float movementSpeed;

    private Vector2 _movementValue;

    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
    }
    
    // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }

    // Update is called once per frame
    // private void Update()
    // {
    //     
    // }

    void OnMove(InputValue value)
    {
        _movementValue = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        _rb.velocity = _movementValue * movementSpeed;
    }
}
