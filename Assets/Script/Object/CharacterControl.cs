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
    [SerializeField]
    ColliderEvents _damageColliderEvents;

    StateMachine _currentState;
    Vector2 _direction;
    protected float _timeScale = 1.0f;
    protected List<BuffData> _buffs = new List<BuffData>();
    protected Queue<BuffData> _buffsWaitingToAdd = new Queue<BuffData>();
    protected List<float> _buffTimer = new List<float>();
    float _timer = 0.0f;
    bool _init = false;

    const string DefaultState = "Idle";

    private void Start() {
        Init();
    }

    void Update() {
        //_timer += Time.deltaTime;

    }

    void OnDestroy() {
        InputManager.I.LeftStcikEvent -= MoveAndRotate;
        InputManager.I.RightBtnEEvent -= LightAttack;
        InputManager.I.RightBtnSEvent -= Roll;
        InputManager.I.RightBtnWEvent -= SwitchAttack;
        InputManager.I.RightBtnNEvent -= HeavyAttack;
        //_damageColliderEvents.OnTriggerEnterEvent -= OnDamageTriggerEnter;
    }

    public void Init() {
        if (!_init)
        {
            InputManager.I.LeftStcikEvent += MoveAndRotate;
            InputManager.I.RightBtnEEvent += LightAttack;
            InputManager.I.RightBtnSEvent += Roll;
            InputManager.I.RightBtnWEvent += SwitchAttack;
            InputManager.I.RightBtnNEvent += HeavyAttack;
            //_damageColliderEvents.OnTriggerEnterEvent += OnDamageTriggerEnter;

            _currentState = StateMachine.Idle;

            _init = true;
        }
    }

    void MoveAndRotate(Vector2 v, InputManager.ActionState state) {
        if (state == InputManager.ActionState.Game &&
            _animator.CurrentAnimation.IsName(DefaultState))
        {
            //_animator.Play("Idle");
            _animator.SetFloat("Move", Mathf.Abs(v.x) + Mathf.Abs(v.y));
            _rotate.Rotate(v);
        }
    }

    void LockOnMove(Vector2 v, InputManager.ActionState state)
    {
        _animator.SetFloat("X", v.x);
        _animator.SetFloat("Y", v.y);
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

    void OnDamageTriggerEnter(Collider collider) {

    }

    void StateUpdate() {

    }

    public void OnHit(UnitData sender, SkillData skill) {

    }

    void SetBuffValue() {

    }

}
