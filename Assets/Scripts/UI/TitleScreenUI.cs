using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

namespace UI
{
    public class TitleScreenUI : UI
    {
        [SerializeField] 
        private TMP_Text flashingText;
        private bool _cutsceneIsFinished;
        private IDisposable _eventListener;

        private void Awake()
        {
#if (UNITY_WEBGL)
            flashingText.text = "[Click anywhere]";
#endif
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(9.58f);
            _cutsceneIsFinished = true;
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
