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
        protected AudioClip selectSoundEffect;
        [SerializeField]
        protected AudioClip submitSoundEffect;
        [SerializeField]
        private AudioClip silence;

        protected AudioClip currentClip;

        protected virtual void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => AudioManager.Instance.sfxSource.PlayOneShot(submitSoundEffect));
            currentClip = selectSoundEffect;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            button.Select();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            AudioManager.Instance.sfxSource.PlayOneShot(currentClip);
        }

        public void ToggleSoundEffect(bool state)
        {
            currentClip = state ? selectSoundEffect : silence;
        }
    }
}
