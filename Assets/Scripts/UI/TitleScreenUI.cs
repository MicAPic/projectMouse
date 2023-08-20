using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

namespace UI
{
    public class TitleScreenUI : UI
    {
        private bool _cutsceneIsFinished;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(9.58f);
            _cutsceneIsFinished = true;
        }

        void OnEnable()
        {
            InputSystem.onAnyButtonPress.CallOnce(_ =>
            {
                if (_cutsceneIsFinished)
                    SceneManager.LoadScene("Menu");
            });
        }
    }
}
