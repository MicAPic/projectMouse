using Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class TransitionController : MonoBehaviour
    {
        public static TransitionController Instance { get; private set; }

        public float transitionDuration;
        [SerializeField]
        protected RawImage transitionSprite;

        private float _defaultPosX;

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject.transform.parent.gameObject);
                return;
            }

            Instance = this;
            
            DontDestroyOnLoad(gameObject.transform.parent.gameObject);
            _defaultPosX = transitionSprite.rectTransform.anchoredPosition.x;
        }

        public virtual void TransitionAndLoadScene(string sceneToLoad)
        {
            AudioManager.Instance.FadeOutAll(transitionDuration);
            AudioManager.Instance.ToggleLowpass(false, transitionDuration);
            
            transitionSprite.raycastTarget = true;
            var transitionSequence = DOTween.Sequence();
            transitionSequence.SetUpdate(true);
            transitionSequence.Append(transitionSprite.rectTransform.DOAnchorPosX(0.0f, transitionDuration)
                                                                    .SetEase(Ease.Linear));

            transitionSequence.AppendCallback(() =>
            {
                SceneManager.LoadScene(sceneToLoad);

                GameManager.isPaused = false;
                GameManager.isGameOver = false;
                    
                if (Time.timeScale < 1.0f)
                {
                    Time.timeScale = 1.0f;
                }
            });
            transitionSequence.Append(transitionSprite.rectTransform.DOAnchorPosX(-_defaultPosX, transitionDuration)
                                                                    .SetEase(Ease.Linear));
            transitionSequence.OnComplete(() => transitionSprite.raycastTarget = false);
        }
    }
}
