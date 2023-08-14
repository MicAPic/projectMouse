using UnityEngine;

namespace UI
{
    public class UI : MonoBehaviour
    {
        public void LoadScene(string sceneToLoad)
        {
            if (TransitionController.Instance == null)
            {
                Debug.LogWarning("Add a Transition, you dumbass!");
                return;
            }
            TransitionController.Instance.TransitionAndLoadScene(sceneToLoad);
        }
    }
}
