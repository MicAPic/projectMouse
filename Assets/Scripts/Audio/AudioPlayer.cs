using DG.Tweening;
using UnityEngine;

namespace Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        public float fadeInDuration;
        public string prefsVolumeName;
        [SerializeField] 
        private string exposedVolumeName;

        private float _volumeModifier;
        private const float MaxVolume = 0.0f;
        private const float MinVolume = -80.0f;

        // Start is called before the first frame update
        void Start()
        {
            _volumeModifier = PlayerPrefs.GetFloat(prefsVolumeName, 1.0f);
            
            AudioManager.Instance.audioMixer.SetFloat(exposedVolumeName, MinVolume);
            FadeIn(fadeInDuration);
        }

        public void FadeIn(float duration)
        {
            AudioManager.Instance.audioMixer.DOSetFloat(
                exposedVolumeName, 
                (1.0f - Mathf.Sqrt(_volumeModifier)) * -80f, 
                duration).SetUpdate(true).SetEase(Ease.Linear);
        }

        public void FadeOut(float duration)
        {
            StartCoroutine(FadeMixerGroup.StartFade(
                AudioManager.Instance.audioMixer,
                exposedVolumeName,
                duration,
                MinVolume,
                MinVolume
                ));
        }

        public void SetVolumeModifier(float volumeMod)
        {
            PlayerPrefs.SetFloat(prefsVolumeName, volumeMod);
            AudioManager.Instance.audioMixer.SetFloat(
                exposedVolumeName, 
                (1.0f - Mathf.Sqrt(volumeMod)) * -80f);
        }
    }
}