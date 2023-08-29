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
        private Button startButton;
        [SerializeField]
        private Button tutorialButton;
        
        [Header("Settings")]
        [SerializeField]
        private RectTransform settingsSubmenu;
        [SerializeField]
        private Button settingsCloseButton;
        [SerializeField]
        private CanvasGroup settingsContent;
        [SerializeField]
        private float settingsAnimationDuration = 0.5f;

        protected override void Awake()
        {
            base.Awake();
            if (PlayerPrefs.HasKey("PlayedTutorial")) return;
            
            startButton.interactable = false;
            startButton.GetComponentInChildren<TMP_Text>().text = "???";
            tutorialButton.Select();
            tutorialButton.onClick.AddListener(() =>
            {
                PlayerPrefs.SetInt("PlayedTutorial", 1);
            });
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
                settingsSubmenu.gameObject.SetActive(true);
                settingsContent.alpha = 0.0f;
                settingsContent.DOFade(1.0f, settingsAnimationDuration / 2.0f);
                settingsSubmenu.DOAnchorPosY(0.0f, settingsAnimationDuration);
                settingsSubmenu.DOScale(Vector3.one, settingsAnimationDuration)
                    .OnComplete(() => settingsCloseButton.Select());
            }
            else
            {
                settingsContent.DOFade(0.0f, settingsAnimationDuration / 2.0f);
                settingsSubmenu.DOAnchorPosY(-41.0f, settingsAnimationDuration);
                settingsSubmenu.DOScale(Vector3.zero, settingsAnimationDuration)
                    .OnComplete(() =>
                    {
                        settingsSubmenu.gameObject.SetActive(false);
                        startButton.Select();
                    });
            }
        }
    }
}
