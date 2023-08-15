using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PixelizeTransitionController : TransitionController
    {
        protected override void Awake()
        {
            transitionSprite.material.SetFloat("_PixelSize", 0.0f);
        }

        public override void TransitionAndLoadScene(string sceneToLoad)
        {
            StartCoroutine(TakeScreenshotAndLoad(sceneToLoad));
        }

        private IEnumerator TakeScreenshotAndLoad(string sceneToLoad)
        {
            yield return new WaitForEndOfFrame();

            var screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            var screenRegion = new Rect(0, 0, Screen.width, Screen.height);
            screenTexture.ReadPixels(screenRegion, 0, 0, false);
            screenTexture.Apply(); // render the texture on GPU
            transitionSprite.texture = screenTexture;
            transitionSprite.color = Color.white;

            transitionSprite.material.DOFloat(0.15f, "_PixelSize", transitionDuration)
                .OnComplete(() => SceneManager.LoadScene(sceneToLoad));
        }
    }
}
