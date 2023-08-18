using UnityEngine;

namespace Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        public float fadeInDuration;
        [SerializeField] 
        private string exposedVolumeName;
        [SerializeField] 
        private string prefsVolumeName;

        private float _maxVolume;

        // Start is called before the first frame update
        void Start()
        {
            _maxVolume = PlayerPrefs.GetFloat(prefsVolumeName, 0.994f);
            
            AudioManager.Instance.audioMixer.SetFloat(exposedVolumeName, Mathf.Log10(0.0001f) * 20);
            FadeIn(fadeInDuration);
        }

        public void FadeIn(float duration)
        {
            StartCoroutine(FadeMixerGroup.StartFade
                (
                    AudioManager.Instance.audioMixer, 
                    exposedVolumeName, 
                    duration, 
                    _maxVolume
                )
            );
        }

        public void FadeOut(float duration)
        {
            StartCoroutine(FadeMixerGroup.StartFade
                (
                    AudioManager.Instance.audioMixer, 
                    exposedVolumeName, 
                    duration, 
                    0.0001f
                )
            );
        }

        public void SetVolume(float volume)
        {
            PlayerPrefs.SetFloat(prefsVolumeName, volume);
            AudioManager.Instance.audioMixer.SetFloat(exposedVolumeName, Mathf.Log10(volume) * 20);
        }
    }
}