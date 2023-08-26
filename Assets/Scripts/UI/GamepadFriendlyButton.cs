using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class GamepadFriendlyButton : MonoBehaviour, IPointerEnterHandler
    {
        private Button _button;

        void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _button.Select();
        }
    }
}
