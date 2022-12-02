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
    Animator _animator;
    [SerializeField]
    float _hpPositionY;

    StateMachine _currentState;
    Movement _movement;
    CharacterData _characterData;
    AnimatorStateInfo _currentAnimation;
    GameObject _hpBarObject;
    GameObject _bulletPrefab;
    protected float _timeScale = 1.0f;
    protected List<BuffData> _buffs = new List<BuffData>();
    protected Queue<BuffData> _buffsWaitingToAdd = new Queue<BuffData>();
    protected List<float> _buffTimer = new List<float>();
    float _timer = 0.0f;
    float _atkRadio;
    float _atkSpeed;
    bool _init = false;

    public void Init(string id) {
        if (!_init)
        {
            _characterData = DataManager.I.GetCharacterData(id);
            _atkRadio = _characterData.NowViewRadius;
            if (_weapon != null)
            {
                _weapon.damage = _characterData.NowAtk;
            }
            _characterData.RefreshEvent += Refresh;

            _bulletPrefab = ResourceManager.I.Load<GameObject>(AssetPath.CHARACTER_BULLET);

            _movement = GetComponent<Movement>();
            _movement.moveSpeed = _characterData.NowMoveSpeed;

            InputManager.I.LeftStcikEvent += Move;
            InputManager.I.RightBtnWEvent += Attack;
            _damageColliderEvents.OnTriggerEnterEvent += OnDamageTriggerEnter;

            var hpBarPefab = ResourceManager.I.Load<GameObject>(AssetPath.HP_BAR);
            _hpBarObject = ObjectPool.I.Create(hpBarPefab);
            _hpBarObject.GetComponent<HPBar>().SetValue(1);
            _hpBarObject.SetActive(true);

            _currentState = StateMachine.Idle;

            _init = true;
        }
    }

    void Update() {
        if (_currentState != StateMachine.Dead)
        {
            _timer += Time.deltaTime;
            if (_hpBarObject != null)
            {
                _hpBarObject.transform.position = transform.position + new Vector3(0, _hpPositionY, 0);
            }

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Attack") && _animator.IsInTransition(0))
            {
                _currentState = StateMachine.Idle;
            }

            if (_weapon != null)
            {
                _weapon.gameObject.SetActive(_currentState == StateMachine.Attack);
            }

            StateUpdate();
        }
    }

    void OnDestroy() {
        _characterData.RefreshEvent -= Refresh;
        InputManager.I.LeftStcikEvent -= Move;
        InputManager.I.RightBtnWEvent -= Attack;
        _damageColliderEvents.OnTriggerEnterEvent -= OnDamageTriggerEnter;
    }

#if UNITY_EDITOR
    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, _atkRadio);
    }
#endif

    void Refresh(CharacterData data) {
        if (data.NowHP <= 0)
        {
            GlobalMessenger.Launch(EventMsg.GameOver);
            //Destroy(gameObject);
            return;
        }
        _movement.moveSpeed = data.NowMoveSpeed;
        _hpBarObject.GetComponent<HPBar>().SetValue((float)data.NowHP / (float)data.MaxHP);
    }

    void Move(Vector2 v, InputManager.ActionState state) {
        if (state == InputManager.ActionState.Game)
        {
            _animator.SetFloat("Move", Mathf.Abs(v.x) + Mathf.Abs(v.y));
            _movement.Move(v);
        }
    }

    void Attack(InputManager.ActionState state) {
        if (state == InputManager.ActionState.Game)
        {
            _currentState = StateMachine.Attack;
            _animator.SetTrigger("Attack");

            if (_timer > _characterData.Skill.cd)
            {
                _timer = 0;
                var bulletObject = ObjectPool.I.Create(_bulletPrefab);
                bulletObject.transform.position = _firePoint.position;
                if (transform.localScale.x > 0)
                {
                    bulletObject.transform.localScale = new Vector3(Mathf.Abs(bulletObject.transform.localScale.x),
                        bulletObject.transform.localScale.y, bulletObject.transform.localScale.z);
                }
                else if (transform.localScale.x < 0)
                {
                    bulletObject.transform.localScale = new Vector3(-Mathf.Abs(bulletObject.transform.localScale.x),
                        bulletObject.transform.localScale.y, bulletObject.transform.localScale.z);
                }

                Bullet bullet = bulletObject.GetComponent<Bullet>();
                bullet.owner = _characterData;
                bullet.skillData = _characterData.Skill;
                bullet.moveDirection = new Vector3(-transform.localScale.x, 0, 0).normalized;
                bullet.speed = _characterData.NowAtkSpeed;
                bullet.gameObject.SetActive(true);
            }
        }
    }

    void OnDamageTriggerEnter(Collider2D collider) {
        if (collider.tag == "Enemy")
        {
            //_characterData.OnHit(collider.transform.GetComponent<EnemyUnit>());
        }
        if (collider.tag == "EnemyWeapon")
        {
            _characterData.OnHit(collider.transform.GetComponent<Weapon>().damage);
        }
        if (collider.tag == "EnemyBullet")
        {
            OnHit(collider.transform.GetComponent<Bullet>().owner, collider.transform.GetComponent<Bullet>().skillData);
        }
    }

    void StateUpdate() {
        foreach (var buff in _buffs)
        {
            foreach (var property in buff.modifiedProperties)
            {
                _characterData.ModifiedChange(property.property, property.actualValue);
            }
            buff.durationTime -= Time.deltaTime;
        }

        _buffs.RemoveAll(buff => (buff.durationTime < 0));
    }

    public void OnHit(UnitData sender, SkillData skill) {
        foreach (var newBuff in skill.buffs)
        {
            newBuff.owner = sender;
            bool isAdded = false;
            for (int i = 0; i < _buffs.Count; i++)
            {
                if (newBuff.id == _buffs[i].id)
                {
                    if (_buffs[i].attributes == BuffAttributes.Multiple)
                    {
                        _buffTimer[i] += newBuff.durationTime;
                        isAdded = true;
                        break;
                    }
                }
            }
            if (!isAdded)
            {
                _buffsWaitingToAdd.Enqueue(newBuff);
            }
        }

        SetBuffValue();

        while (_buffsWaitingToAdd.Count > 0)
        {
            var buff = _buffsWaitingToAdd.Dequeue();
            _buffs.Add(buff);
            _buffTimer.Add(buff.durationTime);
        }
    }

    void SetBuffValue() {
        foreach (var buff in _buffsWaitingToAdd)
        {
            foreach (var property in buff.modifiedProperties)
            {
                property.actualValue = (int)GetPropertyValue(_characterData, buff.owner, property);
            }
        }
    }

    float GetPropertyValue(UnitData myself, UnitData other, PropertyData data) {
        float value = 0;
        switch (data.source)
        {
            case ValueSource.Set:
                value = data.value;
                break;
            case ValueSource.MySelf:
                value = GetBaseValue(myself, data) * data.value;
                break;
            case ValueSource.Others:
                value = GetBaseValue(other, data) * data.value;
                break;
            default:
                break;
        }
        return value;
    }

    float GetBaseValue(UnitData source, PropertyData data) {
        float value = 0;
        switch (data.sourceType)
        {
            case ModifiedProperty.None:
                break;
            case ModifiedProperty.Hp:
                value = source.NowHP;
                break;
            case ModifiedProperty.MaxHp:
                value = source.MaxHP;
                break;
            case ModifiedProperty.Atk:
                value = source.NowAtk;
                break;
            case ModifiedProperty.MaxAtk:
                value = source.MaxAtk;
                break;
            case ModifiedProperty.Def:
                value = source.NowDef;
                break;
            case ModifiedProperty.MaxDef:
                value = source.MaxDef;
                break;
            case ModifiedProperty.MoveSpeed:
                value = source.NowMoveSpeed;
                break;
            case ModifiedProperty.MaxMoveSpeed:
                value = source.MaxMoveSpeed;
                break;
            case ModifiedProperty.CD:

                break;
            default:
                break;
        }
        return value;
    }
}
