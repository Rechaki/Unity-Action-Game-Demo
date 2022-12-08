using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    [SerializeField]
    Weapon _weapon;
    [SerializeField]
    Transform _firePoint;
    [SerializeField]
    ColliderEvents _damageColliderEvents;
    [SerializeField]
    UnitMove _movement;
    [SerializeField]
    UnitRotate _rotate;
    [SerializeField]
    UnitAnimator _animator;

    StateMachine _currentState;
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
        if (InputManager.I != null)
        {
            InputManager.I.LeftStcikEvent -= MoveAndRotate;
            InputManager.I.RightBtnWEvent -= SwitchAttack;
            InputManager.I.RightBtnSEvent -= Roll;
            InputManager.I.RightBtnNEvent -= Attack;
            //_damageColliderEvents.OnTriggerEnterEvent -= OnDamageTriggerEnter;
        }
    }

    public void Init() {
        if (!_init)
        {
            InputManager.I.LeftStcikEvent += MoveAndRotate;
            InputManager.I.RightBtnWEvent += SwitchAttack;
            InputManager.I.RightBtnSEvent += Roll;
            InputManager.I.RightBtnNEvent += Attack;
            //_damageColliderEvents.OnTriggerEnterEvent += OnDamageTriggerEnter;

            _currentState = StateMachine.Idle;

            _init = true;
        }
    }

    void MoveAndRotate(Vector2 v, InputManager.ActionState state) {
        if (state == InputManager.ActionState.Game)
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

    void Attack(InputManager.ActionState state)
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