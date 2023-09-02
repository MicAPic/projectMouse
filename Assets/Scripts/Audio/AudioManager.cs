using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public AudioMixer audioMixer;
        
        [Header("Snapshots")]
        [SerializeField]
        private float snapshotTransitionTime = 0.5f;
        [SerializeField]
        private AudioMixerSnapshot defaultSnapshot;
        [SerializeField]
        private AudioMixerSnapshot muffledSnapshot;

        [Header("Audio Sources")]
        public AudioSource sfxSource;
        public AudioSource musicSource;
        
        [Header("Audio Players")]
        private AudioPlayer _sfxPlayer;
        private AudioPlayer _musicPlayer;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            _sfxPlayer = transform.GetChild(0).GetComponent<AudioPlayer>();
            _musicPlayer = transform.GetChild(1).GetComponent<AudioPlayer>();
        }

        public void FadeOutAll(float transitionDuration)
        {
            _musicPlayer.FadeOut(transitionDuration);
            _sfxPlayer.FadeOut(transitionDuration);
        }

        public void ToggleLowpass(bool state, float customDuration=0.0f)
        {
            var duration = customDuration > 0 ? customDuration : snapshotTransitionTime;
            if (state)
            {
                muffledSnapshot.TransitionTo(duration);
            }
            else
            {
                defaultSnapshot.TransitionTo(duration);
            }
        }
    }
}
