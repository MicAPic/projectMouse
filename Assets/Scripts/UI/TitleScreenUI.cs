using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace UI
{
    public class TitleScreenUI : UI
    {
        [SerializeField]
        private PlayerInput inputModule;
        [SerializeField]
        private PlayableDirector director;
        [SerializeField]
        private Animator spotlightAnimator;
        
        [SerializeField] 
        private TMP_Text flashingText;
        private bool _cutsceneIsFinished;
        private IDisposable _eventListener;

        protected override void Awake()
        {
            base.Awake();
#if (UNITY_WEBGL)
            flashingText.text = "[Click anywhere]";
#endif
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds((float)director.duration - 0.33f);
            _cutsceneIsFinished = true;
        }

        private void Update()
        {
            if (inputModule.actions["Skip"].WasPressedThisFrame() && !_cutsceneIsFinished)
            {
                director.time = director.duration;
                spotlightAnimator.enabled = false;
                spotlightAnimator.GetComponent<RectTransform>().DOSizeDelta(Vector2.one * 1000f, 1.0f);
                _cutsceneIsFinished = true;
            }
        }

        void OnEnable()
        {
            _eventListener = InputSystem.onAnyButtonPress.Call(_ =>
            {
                if (_cutsceneIsFinished)
                    SceneManager.LoadScene("Menu");
            });
        }

        private void OnDisable()
        {
            _eventListener.Dispose();
        }
    }
}
