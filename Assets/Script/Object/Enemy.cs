using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ICharacter
{
    public Transform lookatTarget => _lookatTarget;

    [SerializeField]
    Transform _lookatTarget;
    [SerializeField]
    Transform _eye;
    [SerializeField]
    UnitRotate _unitRotate;
    [SerializeField]
    UnitAnimation _unitAnimation;
    [SerializeField]
    float _viewAngle = 120f;
    [SerializeField]
    float _viewRadius = 5f;
    [SerializeField]
    int _viewRayNum = 20;
    [SerializeField]
    float _arriveRadius = 1f;
    [SerializeField]
    float _slowRadius = 2f;
    [SerializeField]
    float _atkSpiritMax = 2f;
    [SerializeField]
    ColliderEvents _damageCollider;

    Transform _target;
    Vector3 _directionToTarget;
    float _distanceToTarget;
    float _moveSpeed;
    float _thinkTimer;
    float _atkSpirit;

    const int COLLISION_LAYERS = ~(1 << 5 | 1 << 9 | 1 << 10);
    const int LOCKON_LAYERS = 1 << 9;
    const float MAX_SPEED = 1f;
    const float THINK_TIME = 3f;

    void Start() {
        if (_lookatTarget == null)
        {
            Debug.LogError("Look at target is null.");
        }
        else if (_eye == null)
        {
            Debug.LogError("eye is null.");
        }

        _unitAnimation.Init();

        //_unitAnimation.OnStateChange += StateChange;
        //_damageCollider.OnTriggerEnterEvent += OnDamage;
        _unitAnimation.AnimatorStateEvent.OnEnter += Think;

    }

    void Update() {
        _unitAnimation.SetBool("HaveTarget", _target != null);
        if (_target == null)
        {
            SetFireView();
        }
        else
        {
            _directionToTarget = _target.position - transform.position;
            _distanceToTarget = Vector3.Distance(_target.position, transform.position);
            _unitAnimation.SetFloat("Distance", _distanceToTarget);

            if (_thinkTimer > 0)
            {
                if (_distanceToTarget < _arriveRadius)
                {
                    //_unitAnimation.CrossFade(AnimationName.StepBackward);
                }
                else
                {
                    //_unitAnimation.CrossFade(AnimationName.FightIdle);
                    _directionToTarget.Normalize();
                    _unitRotate.RotateTo(_directionToTarget.x, _directionToTarget.z);
                    _unitRotate.Rotate();
                }
                _thinkTimer -= Time.deltaTime;
                _unitAnimation.SetFloat("ThinkTime", _thinkTimer);
            }
            else
            {
                ArriveMove();
                _atkSpirit = Random.Range(0, _atkSpiritMax);
                _unitAnimation.SetFloat("AtkSpirit", _atkSpirit);
            }

        }
    }

    void OnDestroy()
    {
        //_unitAnimation.OnStateChange -= StateChange;
        _damageCollider.OnTriggerEnterEvent -= OnDamage;
    }

    void StateChange(AnimationName PrevState, AnimationName NewState) {
        if (PrevState == AnimationName.Punch || PrevState == AnimationName.Swiping)
        {
            Think();
        }
    }

    void SetFireView() {
        Vector3 farLeftRayPos = Quaternion.Euler(0, -_viewAngle / 2, 0) * _eye.forward * _viewRadius;
        for (int i = 0; i <= _viewRayNum; i++)
        {
            Vector3 rayPos = Quaternion.Euler(0, (_viewAngle / _viewRayNum) * i, 0) * farLeftRayPos;
            Ray ray = new Ray(_eye.position, rayPos);
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(ray, out hit, _viewRadius, COLLISION_LAYERS);

#if UNITY_EDITOR
            Vector3 pos = _eye.position + rayPos;
            if (hit.transform != null)
            {
                pos = hit.point;
            }
            Debug.DrawLine(_eye.position, pos, Color.red); ;
#endif
            if (hit.transform != null)
            {
                if (hit.transform.tag == "Player")
                {
                    if (IsBlocked(hit.point) == false)
                    {
                        _target = hit.transform;
                    }
                    
                }
            }

        }

    }

    void ArriveMove() {

        if (_distanceToTarget - _arriveRadius < 0.5f)
        {
            _moveSpeed = 0;
            //_unitAnimation.Play(AnimationName.Punch);
        }
        else
        {
            if (_distanceToTarget > _slowRadius)
            {
                _moveSpeed = MAX_SPEED;
            }
            else
            {
                _moveSpeed = MAX_SPEED * _distanceToTarget / _slowRadius;
            }
        }

        _directionToTarget.Normalize();
        _unitRotate.RotateTo(_directionToTarget.x, _directionToTarget.z);
        _unitRotate.Rotate();
        _unitAnimation.SetFloat("Move", _moveSpeed);

    }

    bool IsBlocked(Vector3 target) {
#if UNITY_EDITOR
        Debug.DrawRay(target, _directionToTarget.normalized, Color.yellow, _distanceToTarget, false);
#endif
        bool isBlocked = Physics.Raycast(target, _directionToTarget.normalized, out RaycastHit hit, _distanceToTarget, COLLISION_LAYERS);
        if (isBlocked)
        {
            Debug.Log(hit.transform.name);
        }

        return isBlocked;
    }

    void Think(AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Idle"))
        {
            _thinkTimer = Random.Range(1, THINK_TIME);
            _unitAnimation.SetFloat("ThinkTime", _thinkTimer);

        }
    }

    void Think() {
        _thinkTimer = Random.Range(1, THINK_TIME);
        _unitAnimation.SetFloat("ThinkTime", _thinkTimer);
    }

    void OnDamage(Collider collider)
    {
        if (collider.tag == "Weapon")
        {
            //_unitAnimation.Play(AnimationName.HitToBody);
        }
    }

    void PathFollowing() {

    }

}
