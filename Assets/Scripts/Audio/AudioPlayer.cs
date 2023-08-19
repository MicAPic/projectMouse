using UnityEngine;

namespace Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        public float fadeInDuration;
        public string prefsVolumeName;
        [SerializeField] 
        private string exposedVolumeName;

        private float _maxVolumeModifier;
        private const float MaxVolume = 0.994f;

        // Start is called before the first frame update
        void Start()
        {
            _maxVolumeModifier = PlayerPrefs.GetFloat(prefsVolumeName, 1.0f);
            
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
                    MaxVolume * _maxVolumeModifier
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

        public void SetVolumeModifier(float volumeMod)
        {
            PlayerPrefs.SetFloat(prefsVolumeName, volumeMod);
            AudioManager.Instance.audioMixer.SetFloat(
                exposedVolumeName, 
                Mathf.Log10(MaxVolume * volumeMod) * 20);
        }
    }
}