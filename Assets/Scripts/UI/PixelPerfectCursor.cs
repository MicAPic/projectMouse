using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class PixelPerfectCursor : MonoBehaviour
    {
        public static PixelPerfectCursor Instance { get; private set; }

        public CanvasGroup canvasGroup;
        [SerializeField]
        private float gamepadModeReticleDistance = 222f;
        public Vector3 gamepadModeReticlePos;
        
        private bool _isInGame;
        private Vector3 _playerPos;
        private RawImage _cursorImage;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            canvasGroup = GetComponent<CanvasGroup>();
            _cursorImage = GetComponent<RawImage>();
            Toggle();
        }

        void Start()
        {
            canvasGroup.alpha = InputManager.Instance.isUsingGamepad ? 0.0f : 1.0f;
            if (PlayerController.Instance == null) return;
            _isInGame = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (InputManager.Instance.isUsingGamepad && _isInGame)
            {
                _playerPos = CameraController.Instance._camera.WorldToScreenPoint(
                    PlayerController.Instance.transform.position
                    );
                gamepadModeReticlePos = (Vector3)PlayerController.Instance.aimDirection * gamepadModeReticleDistance +
                                        _playerPos;
                _cursorImage.rectTransform.position = gamepadModeReticlePos;
            }
            else
            {
                _cursorImage.rectTransform.position = Mouse.current.position.ReadValue();
            }
        }

        public void Toggle()
        {
            _cursorImage.enabled = !_cursorImage.enabled;
        }
    }
}
