using UnityEngine;

namespace UI
{
    public class UI : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Cursor.visible = false;
        }
        
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
