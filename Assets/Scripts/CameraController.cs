using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    [SerializeField]
    private float smoothing = 0.1f;
    [SerializeField]
    private float maxDistanceFromPlayer = 5.0f;
    
    private Vector3 _velocity = Vector3.zero;
    private float _defaultZCoordinate;
    private Camera _camera;

    private void Awake()
    {
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
        Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPosition = player.position;
        var midpoint = Vector2.Lerp(playerPosition, mousePos, 0.5f);

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
