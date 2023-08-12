using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

namespace UI
{
    public class TitleScreenUI : UI
    {
        void OnEnable()
        {
            InputSystem.onAnyButtonPress.CallOnce(_ => SceneManager.LoadScene("Menu"));
        }
    }
}
