using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public bool isUsingGamepad;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    void OnEnable()
    {
        InputUser.onChange += ChangeInputBehaviour;
    }
    
    void OnDisable()
    {
        InputUser.onChange -= ChangeInputBehaviour;
    }

    private void ChangeInputBehaviour(InputUser user, InputUserChange change, InputDevice device)
    {
        if (user.controlScheme == null) return;
        switch (user.controlScheme.Value.name)
        {
            case "Gamepad":
            case "Joystick":
                isUsingGamepad = true;
                PixelPerfectCursor.Instance.canvasGroup.alpha = 0.0f;
                break;
            default:
                isUsingGamepad = false;
                PixelPerfectCursor.Instance.canvasGroup.alpha = 1.0f;
                break;
        }
    }
}
