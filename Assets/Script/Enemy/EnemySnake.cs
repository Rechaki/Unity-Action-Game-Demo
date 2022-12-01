using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnake : EnemyUnit
{
    [SerializeField]
    Transform _head;
    [SerializeField]
    Transform _targetDirection;
    [SerializeField]
    Transform _wiggleDirection;
    [SerializeField]
    Transform _firePoint;
    [SerializeField]
    int _bulletNum = 3;
    [SerializeField]
    float _followPlayerTime = 3f;
    [SerializeField]
    float _offsetAngle = 15f;
    [SerializeField]
    float _hittedStopTime = 0.5f;
    [SerializeField]
    float _hpPositionY = 0.6f;
    [SerializeField]
    float _targetDistance;
    [SerializeField]
    float _smoothSpeed;
    [SerializeField]
    float _wiggleSpeed;
    [SerializeField]
    float _wiggleMagnitude;
    [SerializeField]
    SnakeBody[] _bodyParts;
    [SerializeField]
    SpriteRenderer[] _snakeSpriteRenderers;
    [SerializeField]
    bool _debug = false;

    GameObject _hpBarObject;
    GameObject _bulletPrefab;
    GameObject _bodyBulletPrefab;
    GameObject _player;
    Vector2 _direction;
    Vector3[] _segmentPoses;
    Vector3[] _segmentVelocity;
    List<Vector3> _circlePos;
    bool _hitted;
    int _movePointIndex;
    int _bodyPartIndex;
    int _moveNum;
    float _atkCD;
    float _stopTimer;
    float _followTimer;

    public override void InitData()
    {
        base.InitData();
        _movePointIndex = 0;
        _bodyPartIndex = 0;
        _atkCD = 0;
        _stopTimer = 0;
        _followTimer = 0;
        target = GameManager.I.bossMovePoints[_movePointIndex];
        _bulletPrefab = ResourceManager.I.Load<GameObject>(AssetPath.ENEMY_SNAKE_BULLET);
        _bodyBulletPrefab = ResourceManager.I.Load<GameObject>(AssetPath.ENEMY_SNAKE_BODY_BULLET);
        if (_debug)
        {
            var hpBarPefab = ResourceManager.I.Load<GameObject>(AssetPath.HP_BAR);
            _hpBarObject = ObjectPool.I.Create(hpBarPefab);
            _hpBarObject.GetComponent<HPBar>().SetValue(1);
            _hpBarObject.SetActive(true);
        }

        CurrentState = StateMachine.Create;
        _segmentPoses = new Vector3[_bodyParts.Length + 1];
        _segmentVelocity = new Vector3[_bodyParts.Length + 1];
        _circlePos = new List<Vector3>();
        for (float i = 0; i <= 2 * Mathf.PI; i += (Mathf.PI / 6))
        {
            float xPos = 2 * Mathf.Cos(i);
            float yPos = 2 * Mathf.Sin(i);
            Vector3 pos = new Vector3(xPos, yPos, 0);
            _circlePos.Add(pos);
        }

        Data.RefreshEvent += Refresh;
        damageColliderEvents.OnTriggerEnterEvent += OnDamageTriggerEnter;
        foreach (var body in _bodyParts)
        {
            body.damageColliderEvents.OnTriggerEnterEvent += OnDamageTriggerEnter;
        }
    }

    void Update()
    {
        if (_hitted)
        {
            OnHitEffect();
        }

        switch (CurrentState)
        {
            case StateMachine.Create:
                Create();
                break;
            case StateMachine.Move:
                Move();
                break;
            case StateMachine.FollowPlayer:
                Follow();
                break;
            case StateMachine.Attack:
                Attack();
                break;
            case StateMachine.Dead:
                Dead();
                break;
            default:
                break;
        }

        if (_debug)
        {
            _hpBarObject.transform.position = transform.position + new Vector3(0, _hpPositionY, 0);
        }

        _atkCD += Time.deltaTime;

        StateUpdate();
    }

    void Refresh(EnemyData data)
    {
        if (data.NowHP <= 0)
        {
            CurrentState = StateMachine.Dead;
        }
        else
        {
            if (_debug)
            {
                _hpBarObject.GetComponent<HPBar>().SetValue((float)data.NowHP / (float)data.MaxHP);
            }
        }
    }

    void OnDamageTriggerEnter(Collider2D collider)
    {
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

    protected override void OnDead() {

    }

    void OnHitEffect() {
        _stopTimer += Time.deltaTime;
        foreach (var sprite in _snakeSpriteRenderers)
        {
            sprite.color = Color.black;
        }
        if (_stopTimer > _hittedStopTime)
        {
            foreach (var sprite in _snakeSpriteRenderers)
            {
                sprite.color = Color.white;
            }
            _stopTimer = 0;
            _hitted = false;
        }
    }

    void Create() {
        if ((target.position - _head.position).magnitude < 0.1f)
        {
            _movePointIndex++;
            target = GameManager.I.bossMovePoints[_movePointIndex];
            _moveNum = 0;
            CurrentState = StateMachine.Move;
        }
        else
        {
            MoveToPos(target.position);
        }

    }

    void Move() {
        if (_moveNum < 4)
        {
            if ((target.position - _head.position).magnitude < 0.1f)
            {
                if (_movePointIndex < GameManager.I.bossMovePoints.Length - 1)
                {
                    _movePointIndex++;
                    target = GameManager.I.bossMovePoints[_movePointIndex];
                }
                else
                {
                    _movePointIndex = 0;
                    target = GameManager.I.bossMovePoints[_movePointIndex];
                }
                _moveNum++;
            }
            MoveToPos(target.position);
        }
        else
        {
            CurrentState = StateMachine.FollowPlayer;
        }
        
    }

    void Follow() {
        if (Data.NowHP > Data.MaxHP / 2)
        {
            MoveToPos(_player.transform.position);
            Attack();
        }
        else
        {
            if ((target.position - _head.position).magnitude < 0.1f)
            {
                if (_movePointIndex < _circlePos.Count)
                {
                    _movePointIndex++;
                    target.position = _circlePos[_movePointIndex];
                }
                else
                {
                    _movePointIndex = 0;
                    target.position = _circlePos[_movePointIndex];
                }
            }
            MoveToPos(target.position);
        }

        _followTimer += Time.deltaTime;
        if (_followTimer > _followPlayerTime)
        {
            _followTimer = 0f;
            float min = 0f;
            int index = 0;
            for (int i = 0; i < GameManager.I.bossMovePoints.Length; i++)
            {
                Vector3 distance = GameManager.I.bossMovePoints[i].position - _head.transform.position;
                var dot = Vector3.Dot(_head.right, distance);
                if (dot > 0)
                {
                    if (min == 0f || dot < min)
                    {
                        min = dot;
                        index = i;
                    }
                }
            }
            _movePointIndex = index;
            target = GameManager.I.bossMovePoints[_movePointIndex];
            _moveNum = 0;
            CurrentState = StateMachine.Move;
        }
    }

    void MoveToPos(Vector3 nextPos)
    {
        //head
        _direction = nextPos - _head.position;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _head.rotation = Quaternion.Slerp(_head.rotation, rotation, Data.NowTurnSpeed * Time.deltaTime);
        _head.position = Vector2.MoveTowards(_head.position, nextPos, Data.NowMoveSpeed * Time.deltaTime);
        if (_player == null)
        {
            var zone = Physics2D.OverlapCircleAll(transform.position, Data.NowViewRadius);
            foreach (var item in zone)
            {
                if (item.tag == "Player")
                {
                    _player = item.gameObject;
                    break;
                }
            }
        }
        //body
        _wiggleDirection.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(_wiggleSpeed * Time.deltaTime) * _wiggleMagnitude);
        _segmentPoses[0] = _targetDirection.position;
        for (int i = 1; i < _segmentPoses.Length; i++)
        {
            var pos = _segmentPoses[i - 1] + (_segmentPoses[i] - _segmentPoses[i - 1]).normalized * _targetDistance;
            _segmentPoses[i] = Vector3.SmoothDamp(_segmentPoses[i], pos, ref _segmentVelocity[i], _smoothSpeed);
            _bodyParts[i - 1].transform.position = _segmentPoses[i];
        }

    }

    void RotateToPlayer() {
        if (IsFacingToPlayer())
        {
            CurrentState = StateMachine.Attack;
        }
        else
        {
            if (_player == null)
            {
                return;
            }
            Vector3 distance = _player.transform.position - _head.transform.position;
            float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            _head.rotation = Quaternion.Slerp(_head.rotation, rotation, Data.NowTurnSpeed * Time.deltaTime);
        }
    }

    bool IsFacingToPlayer() {
        if (_player == null)
        {
            return false;
        }
        Vector3 distance = _player.transform.position - _head.transform.position;
        float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
        if (angle < _offsetAngle)
        {
            return true;
        }
        return false;
    }

    void Attack()
    {
        if (_atkCD > Data.Skill.cd)
        {
            for (int i = 0; i < _bulletNum; i++)
            {

                var bulletObject = ObjectPool.I.Create(_bulletPrefab);
                bulletObject.transform.position = _firePoint.position;
                Bullet bullet = bulletObject.GetComponent<Bullet>();
                bullet.owner = Data;
                bullet.skillData = Data.Skill;
                bullet.transform.rotation = _head.rotation;
                var dir = (_player.transform.position - _head.position).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                if (_bulletNum % 2 == 1)
                {
                    bullet.moveDirection = Quaternion.AngleAxis(_offsetAngle * (i - _bulletNum / 2), Vector3.forward) * dir;
                    bullet.transform.rotation = Quaternion.AngleAxis(angle + _offsetAngle * (i - _bulletNum / 2), Vector3.forward);
                }
                else
                {
                    bullet.moveDirection = Quaternion.AngleAxis(_offsetAngle * (i - _bulletNum / 2) + _offsetAngle / 2, Vector3.forward) * dir;
                    bullet.transform.rotation = Quaternion.AngleAxis(angle + _offsetAngle * (i - _bulletNum / 2) + _offsetAngle / 2, Vector3.forward);
                }
                bullet.speed = Data.NowAtkSpeed;
                bullet.gameObject.SetActive(true);
            }

            if (Data.NowHP < (Data.MaxHP / 2))
            {
                var index = Random.Range(0, _bodyParts.Length);
                for (float i = 0; i <= 2 * Mathf.PI; i += (Mathf.PI / 6))
                {
                    var bulletObject = ObjectPool.I.Create(_bodyBulletPrefab);
                    Bullet bullet = bulletObject.GetComponent<Bullet>();
                    bullet.owner = Data;
                    bullet.skillData = Data.Skill;
                    bullet.speed = Data.NowAtkSpeed;
                    bullet.moveDirection = new Vector3(Mathf.Cos(i), Mathf.Sin(i), 0);
                    bullet.transform.position = _bodyParts[index].transform.position;
                    bullet.gameObject.SetActive(true);

                }
                
            }

            _atkCD = 0;
        }

    }

    void Dead() {
        _stopTimer += Time.deltaTime;
        if (_stopTimer > _hittedStopTime)
        {
            if (_bodyPartIndex < _bodyParts.Length)
            {
                var prefab = ResourceManager.I.Load<GameObject>(AssetPath.ENEMY_DEAD_VFX);
                var vfxObject = ObjectPool.I.Create(prefab);
                vfxObject.transform.position = _bodyParts[_bodyPartIndex].transform.position;
                vfxObject.gameObject.SetActive(true);
                _bodyParts[_bodyPartIndex].gameObject.SetActive(false);
                _bodyPartIndex++;
            }
            else
            {
                GlobalMessenger.Launch(EventMsg.KilledTheEnemy);
                target = null;
                ObjectPool.I.Recycle(gameObject);
                ObjectPool.I.Recycle(_hpBarObject);
                GlobalMessenger.Launch(EventMsg.GameClear);
            }
            _stopTimer = 0;
        }
    }
}
