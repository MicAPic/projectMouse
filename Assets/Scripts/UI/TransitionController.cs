using System;
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

        [SerializeField]
        protected RawImage transitionSprite;
        [SerializeField]
        protected float transitionDuration;

        private float _defaultPosX;

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            DontDestroyOnLoad(gameObject.transform.parent);
            _defaultPosX = transitionSprite.rectTransform.anchoredPosition.x;
        }

        public virtual void TransitionAndLoadScene(string sceneToLoad)
        {
            AudioManager.Instance.FadeOutAll(transitionDuration);
            
            transitionSprite.raycastTarget = true;
            transitionSprite.rectTransform.DOAnchorPosX(0.0f, transitionDuration)
                .SetUpdate(true)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    SceneManager.LoadScene(sceneToLoad);
                    if (Time.timeScale < 1.0f)
                    {
                        Time.timeScale = 1.0f;
                    }
                    transitionSprite.rectTransform.DOAnchorPosX(-_defaultPosX, transitionDuration)
                        .SetUpdate(true)
                        .SetEase(Ease.Linear)
                        .OnComplete(() => transitionSprite.raycastTarget = false);
                });
        }
    }
}
