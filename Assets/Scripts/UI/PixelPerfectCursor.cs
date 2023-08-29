using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class PixelPerfectCursor : MonoBehaviour
    {
        public static PixelPerfectCursor Instance { get; private set; }
        private RawImage cursorImage;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            cursorImage = GetComponent<RawImage>();
            Toggle();
        }
    
        // Update is called once per frame
        void Update()
        {
            cursorImage.rectTransform.position = Mouse.current.position.ReadValue();
        }

        public void Toggle()
        {
            cursorImage.enabled = !cursorImage.enabled;
        }
    }
}
