using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Actions.html

public class InputData : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction cameraAction;
    private InputAction rollAction;
    private InputAction lockOnAction;

    private ButtonState buttonEast = new ButtonState();
    private ButtonState buttonSouth = new ButtonState();
    private ButtonState buttonWest = new ButtonState();
    private ButtonState buttonNorth = new ButtonState();

    public Vector2 StickLeftValue { get; private set; }
    public Vector2 StickRightValue { get; private set; }
    public ButtonState ButtonEast => buttonEast;
    public ButtonState ButtonSouth => buttonSouth;
    public ButtonState ButtonWest => buttonWest;
    public ButtonState ButtonNorth => buttonNorth;

    void Awake()
    {
        InitInputHandle();
    }

    void OnEnable()
    {
        moveAction.Enable();
        rollAction.Enable();
        lockOnAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        rollAction.Disable();
        lockOnAction.Disable();
    }

    private void InitInputHandle()
    {
        MoveActionInit();
        CameraActionInit();
        RollActionInit();

        lockOnAction = playerInput.actions["LockOn"];

        lockOnAction.performed += ctx => {
            //m_isLockOnState = true;
            //characterAnimator.SetBool("IsLockOn", true);
            Debug.Log("LockOn");
        };
        lockOnAction.canceled += ctx => {
            //m_isLockOnState = false;
            //characterAnimator.SetBool("IsLockOn", false);
            Debug.Log("LockOn Off");
        };
    }

    private void MoveActionInit()
    {
        moveAction = playerInput.actions["Move"];
        moveAction.started += ctx => {
            StickLeftValue = ctx.ReadValue<Vector2>();
            Debug.Log("Move");
        };
        moveAction.performed += ctx => {
            StickLeftValue = ctx.ReadValue<Vector2>();
        };
        moveAction.canceled += ctx => {
            StickLeftValue = Vector2.zero;
            Debug.Log("Move Over");
        };
    }

    private void CameraActionInit()
    {
        cameraAction = playerInput.actions["Camera"];
        cameraAction.started += ctx => {
            StickRightValue = ctx.ReadValue<Vector2>();
        };
        cameraAction.performed += ctx => {
            StickRightValue = ctx.ReadValue<Vector2>();
        };
        cameraAction.canceled += ctx => {
            StickRightValue = Vector2.zero;
        };
    }

    private void RollActionInit()
    {
        rollAction = playerInput.actions["Roll"];
        rollAction.started += ctx => {
            buttonSouth.isSelect = true;
            buttonSouth.value = ctx.ReadValue<float>();
            //EventHandler.Invoke(EventMsg.ROLL);
            Debug.Log("Roll");
        };
        rollAction.canceled += ctx => {
            buttonSouth.isSelect = false;
            buttonSouth.value = ctx.ReadValue<float>();
            //EventHandler.Invoke(EventMsg.ROLL);
            Debug.Log("Roll Over");
        };
    }

}


public struct ButtonState
{
    public bool isSelect;
    public float value;
}