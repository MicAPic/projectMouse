using Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class TutorialUI : InGameUI
    {
        [SerializeField]
        private InputAction pauseInputAction;
        private bool _activateEventSystemOnUnpause;
        private bool _activatePlayerInputOnUnpause;

        [Header("Dialogue Animation")]
        [SerializeField]
        private CanvasGroup dialogueBox;
        [SerializeField]
        private Image tungstenRat;
        [SerializeField]
        private float ratAppearanceDuration = 0.75f;
        [SerializeField]
        private float ratDialoguePosY = 3.0f;
        private float ratDefaultPosY;

        protected override void Awake()
        {
            base.Awake();
            pauseInputAction.performed += _ => ToggleTutorialPause();
            
            ratDefaultPosY = tungstenRat.rectTransform.anchoredPosition.y;
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
            dialogueBox.alpha = 0.0f;
            tungstenRat.color = new Color(1, 1, 1, 0);
            ToggleDialogueBox(true, TransitionController.Instance.transitionDuration * 1.33f);
        }

        public Tween ToggleDialogueBox(bool state, float delay)
        {
            float boxDelay;
            float ratDelay;
            float ratFinalDestination;
            float finalAlpha;
            if (state)
            {
                boxDelay = delay;
                ratDelay = delay + 0.1f;
                ratFinalDestination = ratDialoguePosY;
                finalAlpha = 1.0f;
            }
            else
            {
                boxDelay = delay + 0.1f;
                ratDelay = delay;
                ratFinalDestination = ratDefaultPosY;
                finalAlpha = 0.0f;
            }
            
            dialogueBox.DOFade(finalAlpha, 0.3f)
                       .SetDelay(boxDelay)
                       .SetUpdate(true);
            tungstenRat.DOFade(finalAlpha, ratAppearanceDuration)
                       .SetDelay(ratDelay)
                       .SetUpdate(true);
            var ratMainTween = 
                tungstenRat.rectTransform.DOAnchorPosY(ratFinalDestination, ratAppearanceDuration)
                                         .SetDelay(ratDelay)
                                         .SetUpdate(true);
            return ratMainTween;
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
                AudioManager.Instance.ToggleLowpass(true);
                _activateEventSystemOnUnpause = TextManager.Instance.eventSystem.activeInHierarchy;
                TextManager.Instance.eventSystem.SetActive(true);

                _activatePlayerInputOnUnpause = PlayerController.Instance.playerInput.enabled;
                PlayerController.Instance.playerInput.enabled = false;

                Time.timeScale = 0.0f;
                CameraController.Instance.focusPoint = 0.0f;
        
                cancelButton.Select();
            }
            else
            {
                AudioManager.Instance.ToggleLowpass(false);
                TextManager.Instance.eventSystem.SetActive(_activateEventSystemOnUnpause);
                
                if (_activatePlayerInputOnUnpause)
                    PlayerController.Instance.playerInput.enabled = true;

                Time.timeScale = 1.0f;
                CameraController.Instance.focusPoint = CameraController.Instance.defaultFocusPoint;
            }
        }
    }
}
