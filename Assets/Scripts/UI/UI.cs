using UnityEngine;

namespace UI
{
    public class UI : MonoBehaviour
    {
        [SerializeField]
        private Texture2D emptyCursorTexture;
        
        protected virtual void Awake()
        {
            Cursor.SetCursor(emptyCursorTexture, Vector2.one * 0.5f, CursorMode.ForceSoftware);
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
