using System;
using Audio;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace UI
{
    public class IntroUI : UI
    {
        [SerializeField]
        private IntroManager introTextManager;
        [SerializeField]
        private PlayerInput inputModule;
        
        [Header("Skipping the intro")]
        [SerializeField]
        private TMP_Text skipText;
        [SerializeField]
        private float skipTextAppearanceDuration;
        [SerializeField]
        private float skipTextStayDuration;
        [SerializeField]
        private float skipToMusicTrackTime = 30.65599f;

        private string _defaultSkipText;
        private IDisposable _eventListener;
        private Tween _skipTextAppearanceTween;
        private Tween _skipTextFadeTween;

        void Start()
        {
            _defaultSkipText = skipText.text;
            introTextManager.StartDialogue();
        }

        void Update()
        {
            if (inputModule.actions["Skip"].WasPressedThisFrame())
            {
                AudioManager.Instance.musicSource.time = skipToMusicTrackTime;
                introTextManager.Transition();
            }
        }

        void OnEnable()
        {
            _eventListener = InputSystem.onAnyButtonPress.Call(_ =>
            {
                _skipTextAppearanceTween.Kill();
                _skipTextFadeTween.Kill();
                skipText.text = _defaultSkipText.Replace(
                    "_",
                    InputManager.Instance.GetBindingNameFor("Skip").ToUpper());
                _skipTextAppearanceTween = skipText.rectTransform.DOAnchorPosY(0.0f, skipTextAppearanceDuration);
                _skipTextFadeTween = skipText.DOFade(0.5f, skipTextAppearanceDuration)
                    .OnComplete(() =>
                    {
                        _skipTextAppearanceTween = skipText.rectTransform.DOAnchorPosY(21.0f, skipTextAppearanceDuration)
                            .SetDelay(skipTextStayDuration);
                        _skipTextFadeTween = skipText.DOFade(0.0f, skipTextAppearanceDuration)
                            .SetDelay(skipTextStayDuration);
                    });
            });
        }

        private void OnDisable()
        {
            _skipTextAppearanceTween.Kill();
            _skipTextFadeTween.Kill();
            
            _eventListener.Dispose();
        }
    }
}
