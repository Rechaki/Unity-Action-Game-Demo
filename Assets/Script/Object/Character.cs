using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
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

    private void Start() {
        Init();
    }

    public void Init() {
        if (!_init)
        {
            InputManager.I.LeftStcikEvent += MoveAndRotate;
            InputManager.I.RightBtnWEvent += Attack;
            InputManager.I.RightBtnSEvent += Roll;
            //_damageColliderEvents.OnTriggerEnterEvent += OnDamageTriggerEnter;

            _currentState = StateMachine.Idle;

            _init = true;
        }
    }

    void Update() {
        if (_currentState != StateMachine.Dead)
        {
            _timer += Time.deltaTime;

            //AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            //if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Attack") && _animator.IsInTransition(0))
            //{
            //    _currentState = StateMachine.Idle;
            //}

            StateUpdate();
        }
    }

    void OnDestroy() {
        if (InputManager.I != null)
        {
            InputManager.I.LeftStcikEvent -= MoveAndRotate;
            InputManager.I.RightBtnWEvent -= Attack;
            InputManager.I.RightBtnSEvent -= Roll;
            //_damageColliderEvents.OnTriggerEnterEvent -= OnDamageTriggerEnter;
        }
    }

//#if UNITY_EDITOR
//    void OnDrawGizmos() {
//        Gizmos.DrawWireSphere(transform.position, _atkRadio);
//    }
//#endif

    void MoveAndRotate(Vector2 v, InputManager.ActionState state) {
        if (state == InputManager.ActionState.Game)
        {
            //_animator.Play("MoveBlendTree");
            //_animator.SetFloat("Move", Mathf.Abs(v.x) + Mathf.Abs(v.y));
            //_movement.MoveWithRigidbody(v);
            //_rotate.Rotate(v);
        }
    }

    void Roll(InputManager.ActionState state) {
        if (state == InputManager.ActionState.Game)
        {
            _animator.Play("Roll");
            //_movement.Roll();
        }
    }

    void Attack(InputManager.ActionState state) {

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
