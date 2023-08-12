using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class TransitionController : MonoBehaviour
    {
        [SerializeField]
        private RawImage transitionSprite;
        [SerializeField] 
        private Color transitionColour;
        [SerializeField] 
        private float transitionDuration;
        
        // Start is called before the first frame update
        void Start()
        {
            transitionSprite.color = transitionColour;
            transitionSprite.DOColor(Color.clear, transitionDuration).SetUpdate(true);
        }

        // Update is called once per frame
        // void Update()
        // {
        //
        // }

        public void TransitionAndLoadScene(string sceneToLoad)
        {
            transitionSprite.DOColor(transitionColour, transitionDuration).SetUpdate(true)
                .OnComplete(() => SceneManager.LoadScene(sceneToLoad));
        }
    }
}
