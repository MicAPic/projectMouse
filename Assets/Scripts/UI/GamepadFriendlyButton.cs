using Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class GamepadFriendlyButton : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
    {
        public Button button;
        
        [FormerlySerializedAs("clickSoundEffect")]
        [SerializeField]
        private AudioClip selectSoundEffect;
        [SerializeField]
        private AudioClip submitSoundEffect;
        [SerializeField]
        private AudioClip silence;

        private AudioClip _currentClip;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => AudioManager.Instance.sfxSource.PlayOneShot(submitSoundEffect));
            _currentClip = selectSoundEffect;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            button.Select();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            AudioManager.Instance.sfxSource.PlayOneShot(_currentClip);
        }

        public void ToggleSoundEffect(bool state)
        {
            _currentClip = state ? selectSoundEffect : silence;
        }
    }
}
