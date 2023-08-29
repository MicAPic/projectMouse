using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class TutorialUI : InGameUI
    {
        [SerializeField]
        private InputAction pauseInputAction;
        private bool _activateEventSystemOnUnpause;
        private float _cachedTextSpeed;

        protected override void Awake()
        {
            base.Awake();
            pauseInputAction.performed += _ => ToggleTutorialPause();
        }

        void OnEnable()
        {
            pauseInputAction.Enable();
        }
        
        void OnDisable()
        {
            pauseInputAction.Disable();
        }

        protected override void Start()
        {
        }

        public override void UnpauseGame()
        {
            ToggleTutorialPause();
        }

        private void ToggleTutorialPause()
        {
            if (ExperienceManager.Instance.isLevelingUp) return;
        
            GameManager.isPaused = !GameManager.isPaused;
            pauseScreen.SetActive(GameManager.isPaused);

            if (GameManager.isPaused)
            {
                _activateEventSystemOnUnpause = TextManager.Instance.eventSystem.activeInHierarchy;
                TextManager.Instance.eventSystem.SetActive(true);

                PlayerController.Instance.playerInput.enabled = false;

                Time.timeScale = 0.0f;
                CameraController.Instance.focusPoint = 0.0f;
        
                cancelButton.Select();
            }
            else
            {
                if (_activateEventSystemOnUnpause)
                {
                    TextManager.Instance.eventSystem.SetActive(_activateEventSystemOnUnpause);
                }
                PlayerController.Instance.playerInput.enabled = true;

                Time.timeScale = 1.0f;
                CameraController.Instance.focusPoint = CameraController.Instance.defaultFocusPoint;
            }
        }
    }
}
