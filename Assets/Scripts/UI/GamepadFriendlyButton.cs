using Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class GamepadFriendlyButton : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
    {
        public Button button;
        
        [SerializeField]
        private AudioClip clickSoundEffect;
        [SerializeField]
        private AudioClip silence;

        private AudioClip _currentClip;

        void Awake()
        {
            button = GetComponent<Button>();
            _currentClip = clickSoundEffect;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            button.Select();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Debug.Log(eventData.selectedObject.name);
            AudioManager.Instance.sfxSource.PlayOneShot(_currentClip);
        }

        public void ToggleSoundEffect(bool state)
        {
            _currentClip = state ? clickSoundEffect : silence;
        }
    }
}
