using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

public class InputManager : Singleton<InputManager>
{
    public delegate void DevicesChanged(InputDevice device, InputDeviceChange changeType);
    public delegate void InputDataWithVector2(Vector2 value, ActionState state);
    public delegate void InputDataWithFloat(float value, ActionState state);
    public delegate void InputButtonPerformed(ActionState state);

    public event DevicesChanged DevicesChangedEvent;
    public event InputDataWithVector2 LeftStcikEvent;
    public event InputDataWithVector2 RightStcikEvent;
    public event InputDataWithFloat RightBtnEEvent;
    public event InputDataWithFloat RightBtnSEvent; 
    public event InputDataWithFloat RightBtnWEvent;
    public event InputDataWithFloat RightBtnNEvent;

    public enum Devices
    {
        Keyboard = 0,
        Gamepad = 1,
    }
    public Devices DevicesType { get; private set; }

    public enum ActionState
    {
        None = 0,
        UI = 1,
        Game = 2,
    }
    public ActionState CurrentActionState { get; private set; }

    Keyboard Keyboard => Keyboard.current;
    Mouse Mouse => Mouse.current;
    Gamepad Gamepad => Gamepad.current;

    Vector2 _leftStcikValue = Vector2.zero;
    Vector2 _rightStcikValue = Vector2.zero;
    float _rightBtnE = 0;
    float _rightBtnS = 0;
    float _rightBtnW = 0;
    float _rightBtnN = 0;

    void Start() {
        Init();
    }

    void Update() {
        switch (DevicesType)
        {
            case Devices.Keyboard:
                KeyboardInput();
                break;
            case Devices.Gamepad:
                GamepadInput();
                break;
            default:
                Debug.LogError("Trying to set device but is not recognized by Unity InputSystem!");
                break;
        }

        LeftStcikEvent?.Invoke(_leftStcikValue, CurrentActionState);
        RightStcikEvent?.Invoke(_rightStcikValue, CurrentActionState);
        RightBtnEEvent?.Invoke(_rightBtnE, CurrentActionState);
        RightBtnSEvent?.Invoke(_rightBtnS, CurrentActionState);
        RightBtnWEvent?.Invoke(_rightBtnW, CurrentActionState);
        RightBtnNEvent?.Invoke(_rightBtnN, CurrentActionState);
    }

    //void OnDestroy() {

    //}

    void Init() {
        RefreshInputType();
        CurrentActionState = ActionState.UI;
        InputSystem.onDeviceChange += OnDevicesChanged;

        GlobalMessenger.AddListener(EventMsg.SwitchToUI, () => { CurrentActionState = ActionState.UI; });
        GlobalMessenger.AddListener(EventMsg.SwitchToGameIn, () => { CurrentActionState = ActionState.Game; });
    }

    void RefreshInputType() {
        if (Mouse != null && Keyboard != null)
        {
            DevicesType = Devices.Keyboard;
        }
        if (Gamepad != null)
        {
            DevicesType = Devices.Gamepad;
        }
    }

    void OnDevicesChanged(InputDevice device, InputDeviceChange changeType) {
        switch (changeType)
        {
            case InputDeviceChange.Added:
            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
            case InputDeviceChange.Reconnected:
                RefreshInputType();
                DevicesChangedEvent?.Invoke(device, changeType);
                break;
        }

    }

    void KeyboardInput() {
        Vector2 inputValue = Vector2.zero;
        if (Keyboard.wKey.isPressed || Keyboard.upArrowKey.isPressed)
        {
            inputValue.y = 1;
        }
        else if (Keyboard.sKey.isPressed || Keyboard.downArrowKey.isPressed)
        {
            inputValue.y = -1;
        }
        if (Keyboard.aKey.isPressed || Keyboard.leftArrowKey.isPressed)
        {
            inputValue.x = -1;
        }
        else if (Keyboard.dKey.isPressed || Keyboard.rightArrowKey.isPressed)
        {
            inputValue.x = 1;
        }
        _leftStcikValue = inputValue;
        _rightStcikValue = Mouse.position.ReadValue();
        _rightBtnE = (Keyboard.cKey.isPressed && Keyboard.cKey.wasPressedThisFrame || Keyboard.lKey.isPressed && Keyboard.lKey.wasPressedThisFrame) ? 1 : 0;
        _rightBtnS = (Keyboard.xKey.isPressed && Keyboard.xKey.wasPressedThisFrame || Keyboard.kKey.isPressed && Keyboard.kKey.wasPressedThisFrame) ? 1 : 0;
        _rightBtnW = (Keyboard.zKey.isPressed && Keyboard.zKey.wasPressedThisFrame || Keyboard.jKey.isPressed && Keyboard.jKey.wasPressedThisFrame) ? 1 : 0;
        _rightBtnN = (Keyboard.sKey.isPressed && Keyboard.sKey.wasPressedThisFrame || Keyboard.iKey.isPressed && Keyboard.iKey.wasPressedThisFrame) ? 1 : 0;
    }

    void GamepadInput() {
        _leftStcikValue = Gamepad.leftStick.ReadValue();
        _rightStcikValue = Gamepad.rightStick.ReadValue();
        _rightBtnE = Gamepad.buttonEast.ReadValue();
        _rightBtnS = Gamepad.buttonSouth.ReadValue();
        _rightBtnW = Gamepad.buttonWest.ReadValue();
        _rightBtnN = Gamepad.buttonNorth.ReadValue();
    }

}

