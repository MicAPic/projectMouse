using System;
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
        [SerializeField] 
        private Color transitionColour;
        private Color _clearColourVariant;

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            DontDestroyOnLoad(gameObject.transform.parent);
            _clearColourVariant = new Color(transitionColour.r, transitionColour.g, transitionColour.b, 0.0f);
        }

        public virtual void TransitionAndLoadScene(string sceneToLoad)
        {
            transitionSprite.raycastTarget = true;
            transitionSprite.DOColor(transitionColour, transitionDuration).SetUpdate(true).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    SceneManager.LoadScene(sceneToLoad);
                    transitionSprite.color = transitionColour;
                    transitionSprite.DOColor(_clearColourVariant, transitionDuration).SetUpdate(true).SetEase(Ease.Linear)
                        .OnComplete(() => transitionSprite.raycastTarget = false);
                });
        }
    }
}
