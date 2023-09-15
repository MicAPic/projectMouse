using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.DualShock;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WSA || PACKAGE_DOCS_GENERATION
using UnityEngine.InputSystem.Switch;
#endif
using UnityEngine.InputSystem.XInput;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public bool isUsingGamepad;

    // For setting and resetting DS4 / DualSense light bar
    [SerializeField]
    private Color lightBarColour = new(0.667f, 0.20f, 0.416f);
    private bool _lightBarWasSet;
    // Vibration
    private Coroutine _vibrationCoroutine;
    
    private enum Device
    {
        KeyboardAndMouse,
        ProController,
        DualShock,
        XInputController
    }
    private Device _currentDevice;
    private readonly Dictionary<string, Dictionary<Device, string>> _actionToBindingNameTable = new() 
    {
        {
            "Move",
            new Dictionary<Device, string>
            {
                {Device.KeyboardAndMouse, "WASD keys"},
                {Device.ProController, "Left Stick"},
                {Device.DualShock, "Left stick"},
                {Device.XInputController, "Left stick"}
            }
        },
        {
            "MoveAlt",
            new Dictionary<Device, string>
            {
                {Device.KeyboardAndMouse, "Arrow keys"},
                {Device.ProController, "+Control Pad"},
                {Device.DualShock, "Directional buttons"},
                {Device.XInputController, "D-pad"}
            }
        },
        {
            "Aim",
            new Dictionary<Device, string>
            {
                {Device.KeyboardAndMouse, "Mouse"},
                {Device.ProController, "Right Stick"},
                {Device.DualShock, "Right stick"},
                {Device.XInputController, "Right stick"}
            }
        },
        {
            "Shoot",
            new Dictionary<Device, string>
            {
                {Device.KeyboardAndMouse, "the <u>Left Mouse Button"},
                {Device.ProController, "<u>R or ZR"},
                {Device.DualShock, "<u>R1 or R2"},
                {Device.XInputController, "<u>RB or RT"}
            }
        },
        {
            "Dodge",
            new Dictionary<Device, string>
            {
                {Device.KeyboardAndMouse, "Space"},
                {Device.ProController, "L"},
                {Device.DualShock, "L1"},
                {Device.XInputController, "LB"}
            }
        },
        {
            "DodgeAlt",
            new Dictionary<Device, string>
            {
                {Device.KeyboardAndMouse, "the <u>Right Mouse Button"},
                {Device.ProController, "<u>ZL"},
                {Device.DualShock, "<u>L2"},
                {Device.XInputController, "<u>LT"}
            }
        },
        {
            "Skip",
            new Dictionary<Device, string>
            {
                {Device.KeyboardAndMouse, "Tab"},
                {Device.ProController, "+"},
                {Device.DualShock, "Options"},
                {Device.XInputController, "Start"}
            }
        },
        {
            "FastForward",
            new Dictionary<Device, string>
            {
                {Device.KeyboardAndMouse, "F"},
                {Device.ProController, "X"},
                {Device.DualShock, "Triangle"},
                {Device.XInputController, "Y"}
            }
        },
        {
            "Select",
            new Dictionary<Device, string>
            {
                {Device.KeyboardAndMouse, "Space"},
                {Device.ProController, "A"},
                {Device.DualShock, "Cross"},
                {Device.XInputController, "A"}
            }
        },
    };

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _currentDevice = Device.KeyboardAndMouse;
    }
    
    void OnEnable()
    {
        InputUser.onChange += ChangeInputBehaviour;
    }
    
    void OnDisable()
    {
        InputUser.onChange -= ChangeInputBehaviour;
    }

    void OnApplicationQuit()
    {
        if (!_lightBarWasSet) return;
        DualShockGamepad.current.SetLightBarColor(Color.black);
        ForceStopVibration();
    }

    public void ForceStopVibration()
    {
        StopCoroutine(_vibrationCoroutine);
        Gamepad.current.SetMotorSpeeds(0.0f, 0.0f); // since ResetHaptics doesnt work well
    }

    public void ExecuteDamageHaptics()
    {
        _vibrationCoroutine = StartCoroutine(Vibrate(0.0f, 0.2f, 0.1f));
    }

    public string GetBindingNameFor(string actionName)
    {
        return _actionToBindingNameTable[actionName][_currentDevice];
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
        
        if (device == null) return;
        switch (device)
        {
            #if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WSA || PACKAGE_DOCS_GENERATION
            case SwitchProControllerHID:
                _currentDevice = Device.ProController;
                break;
            #endif
            case DualShockGamepad dualShock:
                _currentDevice = Device.DualShock;
                dualShock.SetLightBarColor(lightBarColour);
                _lightBarWasSet = true;
                break;
            case XInputController or Gamepad:
                _currentDevice = Device.XInputController;
                break;
            default:
                _currentDevice = Device.KeyboardAndMouse;
                break;
        }
    }

    private IEnumerator Vibrate(float lowFrequency, float highFrequency, float duration)
    {
        Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);
        yield return new WaitForSecondsRealtime(duration);
        Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
    }
}
