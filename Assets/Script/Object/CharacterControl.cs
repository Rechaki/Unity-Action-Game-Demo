using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    [SerializeField]
    Camera _characterCamera;
    [SerializeField]
    UnitMove _movement;
    [SerializeField]
    UnitRotate _rotate;
    [SerializeField]
    UnitAnimator _animator;
    [SerializeReference]
    Weapon _weapon;
    [SerializeField]
    ColliderEvents _damageColliderEvents;

    //StateMachine _currentState;
    protected float _timeScale = 1.0f;
    protected List<BuffData> _buffs = new List<BuffData>();
    protected Queue<BuffData> _buffsWaitingToAdd = new Queue<BuffData>();
    protected List<float> _buffTimer = new List<float>();
    bool _isLockon = false;
    bool _init = false;

    const string DefaultState = "Idle";

    private void Start() {
        Init();
    }

    void Update() {

    }

    void OnDestroy() {
        InputManager.I.LeftStcikEvent -= MoveAndRotate;
        InputManager.I.RightTriggerEvent -= SwitchLockonTarget;
        InputManager.I.RightBtnEEvent -= LightAttack;
        InputManager.I.RightBtnSEvent -= Roll;
        InputManager.I.RightBtnWEvent -= SwitchAttack;
        InputManager.I.RightBtnNEvent -= HeavyAttack;

        _animator.AnimatorStateEvent.OnEnter -= DrawWeapon;

        //_damageColliderEvents.OnTriggerEnterEvent -= OnDamageTriggerEnter;
    }

    public void Init() {
        if (!_init)
        {
            InputManager.I.LeftStcikEvent += MoveAndRotate;
            InputManager.I.RightTriggerEvent += SwitchLockonTarget;
            InputManager.I.RightBtnEEvent += LightAttack;
            InputManager.I.RightBtnSEvent += Roll;
            InputManager.I.RightBtnWEvent += SwitchAttack;
            InputManager.I.RightBtnNEvent += HeavyAttack;

            _animator.AnimatorStateEvent.OnEnter += DrawWeapon;

            //_damageColliderEvents.OnTriggerEnterEvent += OnDamageTriggerEnter;

            _init = true;
        }
    }

    void MoveAndRotate(Vector2 v, InputManager.ActionState state) {
        if (state == InputManager.ActionState.Game)
        {
            //_animator.Play("Idle");
            _animator.SetFloat("Move", Mathf.Abs(v.x) + Mathf.Abs(v.y));
            _animator.SetFloat("X", v.x);
            _animator.SetFloat("Y", v.y);

            if (_isLockon == false && _animator.CurrentAnimation.IsName(DefaultState))
            {
                _rotate.Rotate(v);
            }
            else if(_isLockon)
            {
                transform.rotation = Quaternion.Euler(0, _characterCamera.transform.eulerAngles.y, 0);
            }
        }
    }

    void LockOnMove(Vector2 v, InputManager.ActionState state)
    {
        _animator.SetFloat("X", v.x);
        _animator.SetFloat("Y", v.y);
    }

    void SwitchLockonTarget(float value, InputManager.ActionState state)
    {
        if (state == InputManager.ActionState.Game)
        {
            _animator.SetFloat("RightTriggerValue", value);
            _isLockon = value > 0.5 ? true : false;
        }

    }

    void Roll(InputManager.ActionState state) {
        if (state == InputManager.ActionState.Game)
        {
            _animator.SetTrigger("RightBtnS");
        }
    }

    void SwitchAttack(InputManager.ActionState state)
    {
        if (state == InputManager.ActionState.Game)
        {
            _animator.SetTrigger("RightBtnW");
        }
    }

    void LightAttack(InputManager.ActionState state)
    {
        if (state == InputManager.ActionState.Game)
        {
            _animator.SetTrigger("RightBtnE");
        }
    }

    void HeavyAttack(InputManager.ActionState state)
    {
        if (state == InputManager.ActionState.Game)
        {
            _animator.SetTrigger("RightBtnN");
        }
    }

    void DrawWeapon(AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateInfo.IsName("Draw Sword 2"))
        {
            _weapon.DrawWeapon();
        }
        else if (stateInfo.IsName("Sheath Sword 2"))
        {
            _weapon.SheathWeapon();
        }
    }

    void OnDamageTriggerEnter(Collider collider) {

    }

    void StateUpdate() {

    }

    public void OnHit(UnitData sender, SkillData skill) {

    }

    void SetBuffValue() {

    }

}
