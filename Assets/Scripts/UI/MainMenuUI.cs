using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : UI
    {
        [Header("Main")]
        [SerializeField]
        private Button[] menuButtons;
        [SerializeField]
        private CanvasGroup raycastBlocker;
        [SerializeField]
        private Animator titleAnimator;

        [Header("Settings")]
        [SerializeField]
        private RectTransform settingsSubmenu;
        [SerializeField]
        private GamepadFriendlyButton settingsCloseButton;
        [SerializeField]
        private CanvasGroup settingsContent;

        [Header("Animation")]
        [SerializeField]
        private float buttonAnimationDuration = 0.75f;
        [SerializeField]
        private float buttonAnimationDelayIncrement = 0.25f;
        [SerializeField]
        private float buttonAnimationOffsetX = -5.0f;
        [SerializeField]
        private float settingsAnimationDuration = 0.5f;

        private float _settingsDefaultPosY;

        private static readonly int ToggledOptions = Animator.StringToHash("ToggledOptions");

        protected override void Awake()
        {
            base.Awake();
            _settingsDefaultPosY = settingsSubmenu.anchoredPosition.y;
            
            if (PlayerPrefs.HasKey("PlayedTutorial")) return;
            
            menuButtons[0].interactable = false;
            menuButtons[0].GetComponentInChildren<TMP_Text>().text = "???";
            menuButtons[1].Select();
            menuButtons[1].onClick.AddListener(() =>
            {
                PlayerPrefs.SetInt("PlayedTutorial", 1);
            });
        }

        void Start()
        {
            var offset = new Vector2(buttonAnimationOffsetX, 0.0f);
            var currentDelay = 0.0f;
            Tween fadeTween = null;
            foreach (var button in menuButtons)
            {
                var rect = button.GetComponent<RectTransform>();
                var defaultPosX = rect.anchoredPosition.x;
                rect.anchoredPosition += offset;
                rect.DOAnchorPosX(defaultPosX, buttonAnimationDuration).SetDelay(currentDelay);
                
                var buttonGroup = button.GetComponent<CanvasGroup>();
                buttonGroup.alpha = 0.0f;
                fadeTween = buttonGroup.DOFade(1.0f, buttonAnimationDuration).SetDelay(currentDelay);

                currentDelay += buttonAnimationDelayIncrement;
            }

            fadeTween.OnComplete(() => raycastBlocker.blocksRaycasts = false);
        }

        public void Quit()
        {
#if (UNITY_EDITOR)
            EditorApplication.ExitPlaymode();
#elif (UNITY_STANDALONE)
            Application.Quit();
#elif (UNITY_WEBGL)
            Screen.fullScreen = false;
            Application.ExternalEval("window.open('" + "about:blank" + "','_self')");
#endif
        }

        public void ToggleSettings()
        {
            if (Mathf.Abs(settingsSubmenu.localScale.magnitude - 0) <
                Mathf.Abs(settingsSubmenu.localScale.magnitude - Vector3.one.magnitude))
            {
                menuButtons[2].GetComponent<GamepadFriendlyButton>().ToggleSoundEffect(false);
                titleAnimator.SetBool(ToggledOptions, true);
                raycastBlocker.blocksRaycasts = true;
                settingsSubmenu.gameObject.SetActive(true);
                settingsContent.alpha = 0.0f;
                settingsContent.DOFade(1.0f, settingsAnimationDuration / 2.0f);
                settingsSubmenu.DOAnchorPosY(0.0f, settingsAnimationDuration);
                settingsSubmenu.DOScale(Vector3.one, settingsAnimationDuration)
                    .OnComplete(() =>
                    {
                        settingsContent.blocksRaycasts = true;
                        settingsCloseButton.button.enabled = true;
                        settingsCloseButton.button.Select();
                        settingsCloseButton.ToggleSoundEffect(true);
                    });
            }
            else
            {
                titleAnimator.SetBool(ToggledOptions, false);
                settingsCloseButton.ToggleSoundEffect(false);
                settingsCloseButton.button.enabled = false;
                settingsContent.blocksRaycasts = false;
                settingsContent.DOFade(0.0f, settingsAnimationDuration / 2.0f);
                settingsSubmenu.DOAnchorPosY(_settingsDefaultPosY, settingsAnimationDuration);
                settingsSubmenu.DOScale(Vector3.zero, settingsAnimationDuration)
                    .OnComplete(() =>
                    {
                        raycastBlocker.blocksRaycasts = false;
                        settingsSubmenu.gameObject.SetActive(false);
                        menuButtons[2].Select();
                        menuButtons[2].GetComponent<GamepadFriendlyButton>().ToggleSoundEffect(true);
                    });
            }
        }
    }
}
