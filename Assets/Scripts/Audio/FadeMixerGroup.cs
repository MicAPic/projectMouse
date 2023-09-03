using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public static class FadeMixerGroup 
    {
        public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolumeNormalized, float targetVolumeDB)
        {
            float currentTime = 0;
            audioMixer.GetFloat(exposedParam, out var currentVol);
            currentVol = Mathf.Pow(10, currentVol / 20);
            var targetValue = Mathf.Clamp(targetVolumeNormalized, 0.0001f, 1);
            var isFadeOut = targetValue < currentVol;
        
            while (currentTime < duration)
            {
                currentTime += Time.unscaledDeltaTime;
                float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
                if (isFadeOut || newVol < targetVolumeDB)
                {
                    audioMixer.SetFloat(exposedParam, (1.0f - Mathf.Sqrt(newVol)) * -80f);
                    yield return null;
                }
                else
                {
                    // prevent overshooting
                    audioMixer.SetFloat(exposedParam, targetVolumeDB);
                    yield break;
                }
            }
        }
    }
}