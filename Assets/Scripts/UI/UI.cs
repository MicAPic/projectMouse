using UnityEngine;

namespace UI
{
    public abstract class UI : MonoBehaviour
    {
        public void LoadScene(string sceneToLoad)
        {
            TransitionController.Instance.TransitionAndLoadScene(sceneToLoad);
        }
    }
}
