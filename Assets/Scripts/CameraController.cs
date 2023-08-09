using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    
    public Vector3 mousePos;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private float smoothing = 0.1f;
    [SerializeField]
    private float maxDistanceFromPlayer = 5.0f;
    [SerializeField] [Range(0, 1)] 
    private float focusPoint = 0.5f;
    
    private Vector3 _velocity = Vector3.zero;
    private float _defaultZCoordinate;
    private Camera _camera;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        if (player == null)
        {
            Debug.LogError("No Player is assigned to the camera");
            enabled = false;
            return;
        }

        _camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _defaultZCoordinate = transform.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        mousePos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 playerPosition = player.position;
        var midpoint = Vector2.Lerp(playerPosition, mousePos, focusPoint);

        var difference = midpoint - playerPosition;
        difference = Vector2.ClampMagnitude(difference, maxDistanceFromPlayer);

        Vector3 result = playerPosition + difference;
        result.z = _defaultZCoordinate;
        
        transform.position = Vector3.SmoothDamp(
            transform.position,
            result,
            ref _velocity,
            smoothing);
    }
}
