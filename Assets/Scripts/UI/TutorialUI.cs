using Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
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
        [FormerlySerializedAs("tungstenRat")]
        public Image bubiImage;
        [SerializeField]
        private CanvasGroup dialogueBox;
        [FormerlySerializedAs("ratAppearanceDuration")] 
        [SerializeField]
        private float bubiAppearanceDuration = 0.75f;
        [FormerlySerializedAs("ratDialoguePosY")]
        [SerializeField]
        private float bubiDialoguePosY = 3.0f;
        private float bubiDefaultPosY;

        protected override void Awake()
        {
            base.Awake();
            pauseInputAction.performed += _ => ToggleTutorialPause();
            
            bubiDefaultPosY = bubiImage.rectTransform.anchoredPosition.y;
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
            bubiImage.color = new Color(1, 1, 1, 0);
            ToggleDialogueBox(true, TransitionController.Instance.transitionDuration * 1.33f);
        }

        public Tween ToggleDialogueBox(bool state, float delay)
        {
            float boxDelay;
            float ratDelay;
            float bubiFinalDestination;
            float finalAlpha;
            if (state)
            {
                boxDelay = delay;
                ratDelay = delay + 0.1f;
                bubiFinalDestination = bubiDialoguePosY;
                finalAlpha = 1.0f;
            }
            else
            {
                boxDelay = delay + 0.1f;
                ratDelay = delay;
                bubiFinalDestination = bubiDefaultPosY;
                finalAlpha = 0.0f;
            }
            
            dialogueBox.DOFade(finalAlpha, 0.3f)
                       .SetDelay(boxDelay)
                       .SetUpdate(true);
            bubiImage.DOFade(finalAlpha, bubiAppearanceDuration)
                       .SetDelay(ratDelay)
                       .SetUpdate(true);
            var bubiMainTween = 
                bubiImage.rectTransform.DOAnchorPosY(bubiFinalDestination, bubiAppearanceDuration)
                                         .SetDelay(ratDelay)
                                         .SetUpdate(true);
            return bubiMainTween;
        }

        public override void UnpauseGame()
        {
            ToggleTutorialPause();
        }

        private void ToggleTutorialPause()
        {
            if (ExperienceManager.Instance.isLevelingUp) return;
        
            GameManager.IsPaused = !GameManager.IsPaused;
            pauseScreen.SetActive(GameManager.IsPaused);

            if (GameManager.IsPaused)
            {
                AudioManager.Instance.ToggleLowpass(true);
                _activateEventSystemOnUnpause = TextManager.Instance.eventSystem.activeInHierarchy;
                TextManager.Instance.eventSystem.SetActive(true);

                _activatePlayerInputOnUnpause = PlayerController.Instance.playerInput.currentActionMap.name == "InGame";
                PlayerController.Instance.playerInput.SwitchCurrentActionMap("UI");

                Time.timeScale = 0.0f;
                CameraController.Instance.focusPoint = 0.0f;
                
                cancelButton.Select();
            }
            else
            {
                AudioManager.Instance.ToggleLowpass(false);
                TextManager.Instance.eventSystem.SetActive(_activateEventSystemOnUnpause);
                
                if (_activatePlayerInputOnUnpause)
                    PlayerController.Instance.playerInput.SwitchCurrentActionMap("InGame");

                Time.timeScale = 1.0f;
                CameraController.Instance.focusPoint = CameraController.Instance.DefaultFocusPoint;
            }
        }
    }
}
