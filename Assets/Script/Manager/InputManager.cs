using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

public class InputManager
{
    static InputManager instance = null;
    public static InputManager I
    {
        get
        {
            if (instance == null)
            {
                new InputManager();
            }
            return instance;
        }
    }

    public delegate void DevicesChanged(InputDevice device, InputDeviceChange changeType);
    public delegate void InputDataWithVector2(Vector2 value, ActionState state);
    public delegate void InputDataWithFloat(float value, ActionState state);
    public delegate void InputButtonPerformed(ActionState state);

    public event DevicesChanged DevicesChangedEvent;
    public event InputDataWithVector2 LeftStcikEvent;
    public event InputDataWithVector2 RightStcikEvent;
    public event InputDataWithFloat LeftTriggerEvent;
    public event InputDataWithFloat RightTriggerEvent;
    public event InputButtonPerformed leftShoulderEvent;
    public event InputButtonPerformed rightShoulderEvent;
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

    Vector2 keyboardInputValue = Vector2.zero;
    float _leftStcickVelocityX = 0;
    float _leftStcickVelocityY = 0;
    float _leftTriggerValue = 0;
    float _rightTriggerValue = 0;

    const float KeyboardInputSmoothTime = 0.5f;

    public InputManager() {
        instance = this;
    }

    public void Init() {
        RefreshInputType();
        CurrentActionState = ActionState.Game;
        InputSystem.onDeviceChange += OnDevicesChanged;

        GlobalMessenger.AddListener(EventMsg.SwitchToUI, () => { CurrentActionState = ActionState.UI; });
        GlobalMessenger.AddListener(EventMsg.SwitchToGameIn, () => { CurrentActionState = ActionState.Game; });
    }

    public void Update() {
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

    public void Destroy() {

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
        #region Stcik
        //Y
        if (Keyboard.wKey.isPressed || Keyboard.upArrowKey.isPressed)
        {
            //keyboardInputValue.y = 1;
            keyboardInputValue.y = Mathf.SmoothDamp(keyboardInputValue.y, 1.0f, ref _leftStcickVelocityY, KeyboardInputSmoothTime);
        }
        else if (Keyboard.sKey.isPressed || Keyboard.downArrowKey.isPressed)
        {
            //keyboardInputValue.y = -1;
            keyboardInputValue.y = Mathf.SmoothDamp(keyboardInputValue.y, -1.0f, ref _leftStcickVelocityY, KeyboardInputSmoothTime);
        }
        else
        {
            //keyboardInputValue.y = 0;
            keyboardInputValue.y = Mathf.SmoothDamp(keyboardInputValue.y, 0.0f, ref _leftStcickVelocityY, KeyboardInputSmoothTime);
            keyboardInputValue.y = keyboardInputValue.y > 0.1f ? keyboardInputValue.y : 0;
        }
        //X
        if (Keyboard.aKey.isPressed || Keyboard.leftArrowKey.isPressed)
        {
            //keyboardInputValue.x = -1;
            keyboardInputValue.x = Mathf.SmoothDamp(keyboardInputValue.x, -1.0f, ref _leftStcickVelocityX, KeyboardInputSmoothTime);
        }
        else if (Keyboard.dKey.isPressed || Keyboard.rightArrowKey.isPressed)
        {
            //keyboardInputValue.x = 1;
            keyboardInputValue.x = Mathf.SmoothDamp(keyboardInputValue.x, 1.0f, ref _leftStcickVelocityX, KeyboardInputSmoothTime);
        }
        else
        {
            //keyboardInputValue.x = 0;
            keyboardInputValue.x = Mathf.SmoothDamp(keyboardInputValue.x, 0.0f, ref _leftStcickVelocityX, KeyboardInputSmoothTime);
            keyboardInputValue.x = keyboardInputValue.x > 0.1f ? keyboardInputValue.x : 0;
        }
        
        if(Keyboard.escapeKey.isPressed)
        {
            Application.Quit();
        }

        _leftStcikValue = keyboardInputValue;
        _rightStcikValue.x = Mouse.delta.x.ReadValue();
        _rightStcikValue.y = Mouse.delta.y.ReadValue();

        LeftStcikEvent?.Invoke(_leftStcikValue, CurrentActionState);
        RightStcikEvent?.Invoke(_rightStcikValue, CurrentActionState);

        #endregion

        #region Trigger
        if (Keyboard.leftShiftKey.isPressed)
        {
            RightTriggerEvent?.Invoke(1.0f, CurrentActionState);
        }
        else
        {
            RightTriggerEvent?.Invoke(0.0f, CurrentActionState);
        }
        #endregion

        #region Shoulder
        if (Keyboard.vKey.isPressed && Keyboard.vKey.wasPressedThisFrame)
        {
            leftShoulderEvent?.Invoke(CurrentActionState);
        }
        if (Keyboard.bKey.isPressed && Keyboard.bKey.wasPressedThisFrame)
        {
            rightShoulderEvent?.Invoke(CurrentActionState);
        }
        #endregion

        if (Mouse.rightButton.isPressed && Mouse.rightButton.wasPressedThisFrame)
        {
            RightBtnEEvent?.Invoke(CurrentActionState);
        }

        if (Keyboard.spaceKey.isPressed && Keyboard.spaceKey.wasPressedThisFrame)
        {
            RightBtnSEvent?.Invoke(CurrentActionState);
        }

        if (Keyboard.qKey.isPressed && Keyboard.qKey.wasPressedThisFrame)
        {
            RightBtnWEvent?.Invoke(CurrentActionState);
        }

        if (Mouse.leftButton.isPressed && Mouse.leftButton.wasPressedThisFrame)
        {
            RightBtnNEvent?.Invoke(CurrentActionState);
        }

    }

    void GamepadInput() {
        #region Stcik
        _leftStcikValue = Gamepad.leftStick.ReadValue();
        _rightStcikValue.x = -Gamepad.rightStick.ReadValue().x;
        _rightStcikValue.y = -Gamepad.rightStick.ReadValue().y;

        LeftStcikEvent?.Invoke(_leftStcikValue, CurrentActionState);
        RightStcikEvent?.Invoke(_rightStcikValue, CurrentActionState);
        #endregion

        #region Trigger
        _leftTriggerValue = Gamepad.leftTrigger.ReadValue();
        _rightTriggerValue = Gamepad.rightTrigger.ReadValue();

        LeftTriggerEvent?.Invoke(_leftTriggerValue, CurrentActionState);
        RightTriggerEvent?.Invoke(_rightTriggerValue, CurrentActionState);
        #endregion

        #region Shoulder
        if (Gamepad.leftShoulder.isPressed && Gamepad.leftShoulder.wasPressedThisFrame)
        {
            leftShoulderEvent?.Invoke(CurrentActionState);
        }
        if (Gamepad.rightShoulder.isPressed && Gamepad.leftShoulder.wasPressedThisFrame)
        {
            rightShoulderEvent?.Invoke(CurrentActionState);
        }
        #endregion
        
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

