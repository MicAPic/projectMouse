using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public AudioMixer audioMixer;

        // TODO: create different snapshots for the Mixer
        // [Header("Snapshots")] 
        // public AudioMixerSnapshot normalSnapshot;
        // public AudioMixerSnapshot muffledSnapshot;

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
    }
}
