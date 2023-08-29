using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class PixelPerfectCursor : MonoBehaviour
    {
        private RawImage cursorImage;

        void Awake()
        {
            cursorImage = GetComponent<RawImage>();
            cursorImage.enabled = true;
        }
    
        // Update is called once per frame
        void Update()
        {
            cursorImage.rectTransform.position = Mouse.current.position.ReadValue();
        }
    }
}
