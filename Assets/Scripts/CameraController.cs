using System;
using CameraShake;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    
    [Header("Camera Follow")]
    public Camera _camera;
    [Range(0, 1)] 
    public float focusPoint = 0.5f;
    public float DefaultFocusPoint { get; private set; }
    public Vector3 reticlePos;

    [SerializeField]
    private Transform player;
    [SerializeField]
    private float smoothing = 0.1f;
    [SerializeField]
    private float maxDistanceFromPlayer = 5.0f;

    [Header("Camera Shake")] 
    [SerializeField]
    private CameraShaker cameraShaker;

    private Vector3 _velocity = Vector3.zero;
    private float _defaultZCoordinate;

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

        _camera = GetComponentInChildren<Camera>();
        DefaultFocusPoint = focusPoint;

        if (!Convert.ToBoolean(PlayerPrefs.GetInt("ShakeCamera", 1)))
        {
            cameraShaker.enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _defaultZCoordinate = transform.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        reticlePos = _camera.ScreenToWorldPoint(InputManager.Instance.isUsingGamepad 
            ? PixelPerfectCursor.Instance.gamepadModeReticlePos 
            : PlayerController.Instance.aimDirection);
        Vector2 playerPosition = player.position;
        var midpoint = Vector2.Lerp(playerPosition, reticlePos, focusPoint);

        var difference = midpoint - playerPosition;
        difference = Vector2.ClampMagnitude(difference, maxDistanceFromPlayer);

        Vector3 result = playerPosition + difference;
        result.z = _defaultZCoordinate;
        
        transform.position = Vector3.SmoothDamp(
            transform.position,
            result,
            ref _velocity,
            smoothing,
            float.MaxValue,
            Time.unscaledDeltaTime);
    }
}
