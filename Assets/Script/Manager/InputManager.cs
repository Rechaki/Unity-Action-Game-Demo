﻿using System;
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
    public event InputButtonPerformed RightBtnEEvent;
    public event InputButtonPerformed RightBtnSEvent; 
    public event InputButtonPerformed RightBtnWEvent;
    public event InputButtonPerformed RightBtnNEvent;

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

    KeyControl[] keyboardRightEBtns = { Keyboard.current.cKey, Keyboard.current.lKey };
    KeyControl[] keyboardRightSBtns = { Keyboard.current.xKey, Keyboard.current.kKey };
    KeyControl[] keyboardRightWBtns = { Keyboard.current.zKey, Keyboard.current.jKey };
    KeyControl[] keyboardRightNBtns = { Keyboard.current.sKey, Keyboard.current.iKey };

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

    }

    void OnDestroy() {

    }

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
        else if(Keyboard.escapeKey.isPressed)
        {
            Application.Quit();
        }
        _leftStcikValue = inputValue;
        _rightStcikValue = Mouse.position.ReadValue();

        LeftStcikEvent?.Invoke(_leftStcikValue, CurrentActionState);
        RightStcikEvent?.Invoke(_rightStcikValue, CurrentActionState);

        foreach (var button in keyboardRightEBtns)
        {
            if (button.isPressed && button.wasPressedThisFrame)
            {
                RightBtnEEvent?.Invoke(CurrentActionState);
                break;
            }
        }

        foreach (var button in keyboardRightSBtns)
        {
            if (button.isPressed && button.wasPressedThisFrame)
            {
                RightBtnSEvent?.Invoke(CurrentActionState);
                break;
            }
        }

        foreach (var button in keyboardRightWBtns)
        {
            if (button.isPressed && button.wasPressedThisFrame)
            {
                RightBtnWEvent?.Invoke(CurrentActionState);
                break;
            }
        }

        foreach (var button in keyboardRightNBtns)
        {
            if (button.isPressed && button.wasPressedThisFrame)
            {
                RightBtnNEvent?.Invoke(CurrentActionState);
                break;
            }
        }

    }

    void GamepadInput() {
        _leftStcikValue = Gamepad.leftStick.ReadValue();
        _rightStcikValue = Gamepad.rightStick.ReadValue();

        LeftStcikEvent?.Invoke(_leftStcikValue, CurrentActionState);
        RightStcikEvent?.Invoke(_rightStcikValue, CurrentActionState);

        if (Gamepad.buttonEast.isPressed && Gamepad.buttonEast.wasPressedThisFrame)
        {
            RightBtnEEvent?.Invoke(CurrentActionState);
        }
        if (Gamepad.buttonSouth.isPressed && Gamepad.buttonSouth.wasPressedThisFrame)
        {
            RightBtnSEvent?.Invoke(CurrentActionState);
        }
        if (Gamepad.buttonWest.isPressed && Gamepad.buttonWest.wasPressedThisFrame)
        {
            RightBtnWEvent?.Invoke(CurrentActionState);
        }
        if (Gamepad.buttonNorth.isPressed && Gamepad.buttonNorth.wasPressedThisFrame)
        {
            RightBtnNEvent?.Invoke(CurrentActionState);
        }

    }

}

