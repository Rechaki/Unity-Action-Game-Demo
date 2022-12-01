using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArcher : EnemyUnit
{
    [SerializeField]
    Transform _firePoint;
    [SerializeField]
    float _distance;
    [SerializeField]
    float _hpPositionY;
    [SerializeField]
    float _atkDuration;
    [SerializeField]
    float _hittedStopTime = 0.5f;
    [SerializeField]
    float _targetScale = 0.5f;
    [SerializeField]
    bool _debug = false;

    GameObject _hpBarObject;
    GameObject _bulletPrefab;
    Vector3 _targetPos;
    bool _hitted;
    //float _radius;
    float _atkCD;
    float _atkTimer;
    float _stopTimer;

    Vector3[] _moveDirection = { 
        Vector2.up, Vector2.down, 
        Vector2.left, Vector2.right, 
        Vector2.one, -Vector2.one, 
        new Vector3(1f, -1f, 0f), new Vector3(-1f, 1f, 0f) 
    };

    public override void InitData() {
        base.InitData();
        _atkCD = Data.Skill.cd;
        //_radius = Data.NowViewRadius;
        _bulletPrefab = ResourceManager.I.Load<GameObject>(AssetPath.ENEMY_ARCHER_BULLET);
        if (_debug)
        {
            var hpBarPefab = ResourceManager.I.Load<GameObject>(AssetPath.HP_BAR);
            _hpBarObject = ObjectPool.I.Create(hpBarPefab);
            _hpBarObject.GetComponent<HPBar>().SetValue(1);
            _hpBarObject.SetActive(true);
        }

        CurrentState = StateMachine.Idle;
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
            //var zone = Physics2D.OverlapCircleAll(transform.position, _radius);
            var zone = Physics2D.OverlapCircleAll(transform.position, Data.NowViewRadius);
            foreach (var item in zone)
            {
                if (item.tag == "Player")
                {
                    target = item.transform;
                    break;
                }
            }
            //_radius++;
        }

        GetTargetPosition();
        CurrentState = StateMachine.Move;
    }

    void Move() {
        if ((_targetPos - transform.position).magnitude > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, Time.deltaTime * Data.NowMoveSpeed * _timeScale);
        }
        else
        {
            CurrentState = StateMachine.Attack;
        }
    }

    void Attack() {
        if (target == null)
        {
            if (_atkCD > Data.Skill.cd)
            {
                _atkCD = 0;
                for (float i = 0; i <= 2 * Mathf.PI; i += (Mathf.PI / 2))
                {
                    var bulletObject = ObjectPool.I.Create(_bulletPrefab);
                    bulletObject.transform.position = _firePoint.position;
                    Bullet bullet = bulletObject.GetComponent<Bullet>();
                    bullet.owner = Data;
                    bullet.skillData = Data.Skill;
                    bullet.moveDirection = new Vector3(Mathf.Sin(i), Mathf.Cos(i), 0);
                    bullet.speed = Data.NowAtkSpeed;
                    bullet.gameObject.SetActive(true);

                }
            }
        }
        else
        {
            if (_atkCD > Data.Skill.cd)
            {
                _atkCD = 0;
                for (float i = 0; i <= 2 * Mathf.PI; i += (Mathf.PI / 6))
                {
                    var bulletObject = ObjectPool.I.Create(_bulletPrefab);
                    bulletObject.transform.position = _firePoint.position;
                    Bullet bullet = bulletObject.GetComponent<Bullet>();
                    bullet.owner = Data;
                    bullet.skillData = Data.Skill;
                    bullet.moveDirection = new Vector3(Mathf.Sin(i), Mathf.Cos(i), 0);
                    bullet.speed = Data.NowAtkSpeed;
                    bullet.gameObject.SetActive(true);

                }
            }
        }

        _atkTimer += Time.deltaTime;
        if (_atkTimer > _atkDuration)
        {
            _atkTimer = 0;
            CurrentState = StateMachine.Idle;
            target = null;
        }

    }

    void GetTargetPosition() {
        if (target == null)
        {
            bool isAirWall = false;
            do
            {
                int index = Random.Range(0, _moveDirection.Length);
                _targetPos = transform.position + _moveDirection[index];
                var zone = Physics2D.OverlapCircleAll(_targetPos, 0.5f);
                foreach (var item in zone)
                {
                    if (item.tag == "Untagged")
                    {
                        isAirWall = true;
                        break;
                    }
                }
                isAirWall = false;
            } while (isAirWall);
        }
        else
        {
            var dis = target.transform.position - transform.position;
            _targetPos = transform.position + dis.normalized * _distance;
        }

    }
}
