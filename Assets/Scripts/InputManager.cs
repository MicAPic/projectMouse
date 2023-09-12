using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public bool isUsingGamepad;

    private enum Device
    {
        KeyboardAndMouse,
        ProController,
        DualShock,
        XInputController
    }
    private Device _currentDevice;
    private Dictionary<string, Dictionary<Device, string>> _actionToBindingNameTable = new() 
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
                {Device.KeyboardAndMouse, "the Left Mouse Button"},
                {Device.ProController, "R or ZR"},
                {Device.DualShock, "R1 or R2"},
                {Device.XInputController, "RB or RT"}
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
                {Device.KeyboardAndMouse, "the Right Mouse Button"},
                {Device.ProController, "ZL"},
                {Device.DualShock, "L2"},
                {Device.XInputController, "LT"}
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
        }
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
        _currentDevice = device switch
        {
            SwitchProControllerHID => Device.ProController,
            DualShockGamepad => Device.DualShock,
            XInputController => Device.XInputController,
            _ => Device.KeyboardAndMouse
        };
    }
}
