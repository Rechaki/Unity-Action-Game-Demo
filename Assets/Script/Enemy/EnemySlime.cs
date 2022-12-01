using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlime : EnemyUnit
{
    [SerializeField]
    Transform _firePoint;
    [SerializeField]
    float _distance = 1.0f;
    [SerializeField]
    float _hittedStopTime = 0.5f;
    [SerializeField]
    float _targetScale = 0.6f;
    [SerializeField]
    float _hpPositionY = 0.6f;
    [SerializeField]
    bool _debug = false;

    GameObject _hpBarObject;
    GameObject _bulletPrefab;
    Vector3 _targetPos;
    bool _hitted;
    float _radius;
    float _atkCD;
    float _stopTimer;

    public override void InitData() {
        base.InitData();
        _atkCD = Data.Skill.cd;
        _radius = Data.NowViewRadius;
        _bulletPrefab = ResourceManager.I.Load<GameObject>(AssetPath.ENEMY_SLIME_BULLET);
        transform.localScale = new Vector3(0, 0, 1);
        if (_debug)
        {
            var hpBarPefab = ResourceManager.I.Load<GameObject>(AssetPath.HP_BAR);
            _hpBarObject = ObjectPool.I.Create(hpBarPefab);
            _hpBarObject.GetComponent<HPBar>().SetValue(1);
            _hpBarObject.SetActive(true);
        }

        CurrentState = StateMachine.Create;
        Data.RefreshEvent += Refresh;
        OnStateChange += StateChanged;
        damageColliderEvents.OnTriggerEnterEvent += OnDamageTriggerEnter;
    }

    void Update() {
        if (CurrentState == StateMachine.Dead)
        {
            return;
        }

        if (_hitted)
        {
            animator.speed = 0;
            _stopTimer += Time.deltaTime;
            if (_stopTimer > _hittedStopTime)
            {
                _stopTimer = 0;
                animator.speed = 1;
                _hitted = false;
            }
        }
        else
        {
            switch (CurrentState)
            {
                case StateMachine.Create:
                    OnCreate();
                    break;
                case StateMachine.Idle:
                    DoWaitForTarget();
                    break;
                case StateMachine.Move:
                    Move();
                    break;
                case StateMachine.Attack:
                    Attack();
                    break;
                default:
                    break;
            }
        }

        if (_debug)
        {
            _hpBarObject.transform.position = transform.position + new Vector3(0, _hpPositionY, 0);
        }
        
        _atkCD += Time.deltaTime;

        StateUpdate();
    }


    void OnDestroy() {
        Data.RefreshEvent -= Refresh;
        OnStateChange -= StateChanged;
        damageColliderEvents.OnTriggerEnterEvent -= OnDamageTriggerEnter;
    }

    void Refresh(EnemyData data) {
        if (data.NowHP <= 0)
        {
            CurrentState = StateMachine.Dead;
            target = null;
            ObjectPool.I.Recycle(gameObject);
            ObjectPool.I.Recycle(_hpBarObject);
        }
        else
        {
            if (_debug)
            {
                _hpBarObject.GetComponent<HPBar>().SetValue((float)data.NowHP / (float)data.MaxHP);
            }
        }
    }

    void StateChanged(StateMachine NewState, StateMachine PrevState) {
        animator.SetBool("isIdle", NewState == StateMachine.Idle);
        animator.SetBool("isMove", NewState == StateMachine.Move);
        animator.SetBool("isAttack", NewState == StateMachine.Attack);
    }

    void OnDamageTriggerEnter(Collider2D collider) {
        if (CurrentState == StateMachine.Create)
        {
            return;
        }

        if (collider.tag == "Weapon")
        {
            OnHit(collider.GetComponent<Weapon>().damage);
            _hitted = true;
        }
        else if (collider.tag == "Bullet")
        {
            OnHit(collider.transform.GetComponent<Bullet>().owner, collider.transform.GetComponent<Bullet>().skillData);
            _hitted = true;
        }
    }

    void OnCreate() {
        if (transform.localScale.x < _targetScale)
        {
            transform.localScale += new Vector3(Time.deltaTime, Time.deltaTime, 1);
        }
        else
        {
            transform.localScale = new Vector3(_targetScale, _targetScale, 1);
            CurrentState = StateMachine.Idle;
        }
    }

    void DoWaitForTarget() {
        if (target == null)
        {
            var zone = Physics2D.OverlapCircleAll(transform.position, _radius);
            foreach (var item in zone)
            {
                if (item.tag == "Player")
                {
                    target = item.transform;
                    break;
                }
            }
            _radius++;
        }
        else
        {
            _radius = Data.NowViewRadius;
            CurrentState = StateMachine.Move;
        }
    }

    void Move() {
        GetTargetPosition();

        if ((_targetPos - transform.position).magnitude > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, Time.deltaTime * Data.NowMoveSpeed * _timeScale);
            if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            CurrentState = StateMachine.Attack;
        }
    }

    void Attack() {
        GetTargetPosition();

        if (_atkCD > Data.Skill.cd)
        {
            _atkCD = 0;
            var bulletObject = ObjectPool.I.Create(_bulletPrefab);
            bulletObject.transform.position = _firePoint.position;
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            bullet.owner = Data;
            bullet.skillData = Data.Skill;
            bullet.moveDirection = (target.position - transform.position).normalized;
            bullet.speed = Data.NowAtkSpeed;
            bullet.gameObject.SetActive(true);
        }

        if ((_targetPos - transform.position).magnitude > 0.1f)
        {
            target = null;
            CurrentState = StateMachine.Idle;
        }

    }

    void GetTargetPosition() {
        if (target != null)
        {
            if (target.position.x > transform.position.x)
            {
                _targetPos = new Vector3(target.position.x - _distance, target.position.y, target.position.z);
            }
            else if (target.position.x < transform.position.x)
            {
                _targetPos = new Vector3(target.position.x + _distance, target.position.y, target.position.z);
            }
            else
            {
                _targetPos = target.position;
            }
        }

    }
}
