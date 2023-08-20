using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AudioSetting : MonoBehaviour
    {
        [SerializeField] 
        private AudioPlayer linkedPlayer;
        [SerializeField] 
        private TMP_Text label;
        [SerializeField] 
        private Button leftButton;
        [SerializeField] 
        private Button rightButton;

        private float _currentVolumeModifier;
        
        void Awake()
        {
            _currentVolumeModifier = PlayerPrefs.GetFloat(linkedPlayer.prefsVolumeName, 1.0f);
            SetLabel();
            SetButtons();
        }

        public void AdjustVolume(float modifierIncrement)
        {
            _currentVolumeModifier += modifierIncrement;
            linkedPlayer.SetVolumeModifier(_currentVolumeModifier);
            SetLabel();
            SetButtons();
        }

        private void SetLabel()
        {
            label.text = (_currentVolumeModifier * 100).ToString("N0");
        }
        
        private void SetButtons()
        {
            switch (_currentVolumeModifier)
            {
                case <= 0:
                    leftButton.interactable = false;
                    return;
                case >= 1:
                    rightButton.interactable = false;
                    return;
            }

            leftButton.interactable = true;
            rightButton.interactable = true;
        }
    }
}
