using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class SettingButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField]
        private Color selectedColour = new Color(0.984f, 0.694f, 0.824f);
        private Color _defaultColour;
        private TMP_Text _buttonText;
    
        void Awake()
        {
            _buttonText = GetComponentInChildren<TMP_Text>();
            _defaultColour = _buttonText.color;
        }

        public void OnSelect(BaseEventData eventData)
        {
            _buttonText.color = selectedColour;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _buttonText.color = _defaultColour;
        }
    }
}
