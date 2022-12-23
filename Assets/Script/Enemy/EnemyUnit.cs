//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
//public class EnemyUnit : MonoBehaviour
//{
//    public string id;
//    public Transform target;
//    public ColliderEvents damageColliderEvents;
//    public Animator animator;

//    protected float _timeScale = 1.0f;
//    protected List<BuffData> _buffs = new List<BuffData>();
//    protected Queue<BuffData> _buffsWaitingToAdd = new Queue<BuffData>();
//    protected List<float> _buffTimer = new List<float>();

//    EnemyData _enemyData;
//    CharacterState _currentState;

//    protected delegate void StateChange(CharacterState NewState, CharacterState PrevState);
//    protected StateChange OnStateChange;

//    protected EnemyData Data => _enemyData;

//    protected CharacterState CurrentState
//    {
//        get
//        {
//            return _currentState;
//        }
//        set
//        {
//            if (OnStateChange != null)
//            {
//                OnStateChange(value, _currentState);
//            }
//            _currentState = value;

//            if (value == CharacterState.Dead)
//            {
//                OnDead();
//            }
//        }
//    }

//    protected void StateUpdate() {
//        foreach (var buff in _buffs)
//        {
//            foreach (var property in buff.modifiedProperties)
//            {
//                _enemyData.ModifiedChange(property.property, property.actualValue);
//            }
//            buff.durationTime -= Time.deltaTime;
//        }

//        _buffs.RemoveAll(buff => (buff.durationTime < 0));
//    }

//    public virtual void InitData() {
//        _enemyData = DataManager.I.GetEnemyData(id);
//    }

//    protected virtual void OnDead() {

//        var prefab = ResourceManager.I.Load<GameObject>(AssetPath.ENEMY_DEAD_VFX);
//        var vfxObject = ObjectPool.I.Create(prefab);
//        vfxObject.transform.position = transform.position;
//        vfxObject.gameObject.SetActive(true);
//        GlobalMessenger.Launch(EventMsg.KilledTheEnemy);
//    }

//    protected virtual void OnHit(int damage) {
//        _enemyData.OnHit(damage);
//    }

//    public virtual void OnHit(UnitData sender, SkillData skill) {
//        foreach (var newBuff in skill.buffs)
//        {
//            newBuff.owner = sender;
//            bool isAdded = false;
//            for (int i = 0; i < _buffs.Count; i++)
//            {
//                if (newBuff.id == _buffs[i].id)
//                {
//                    if (_buffs[i].attributes == BuffAttributes.Multiple)
//                    {
//                        _buffTimer[i] += newBuff.durationTime;
//                        isAdded = true;
//                        break;
//                    }
//                }
//            }
//            if (!isAdded)
//            {
//                _buffsWaitingToAdd.Enqueue(newBuff);
//            }
//        }

//        SetBuffValue();

//        while (_buffsWaitingToAdd.Count > 0)
//        {
//            var buff = _buffsWaitingToAdd.Dequeue();
//            _buffs.Add(buff);
//            _buffTimer.Add(buff.durationTime);
//        }
//    }

//    void SetBuffValue() {
//        foreach (var buff in _buffsWaitingToAdd)
//        {
//            foreach (var property in buff.modifiedProperties)
//            {
//                property.actualValue = (int)GetPropertyValue(_enemyData, buff.owner, property);
//            }
//        }
//    }

//    float GetPropertyValue(UnitData myself, UnitData other, PropertyData data) {
//        float value = 0;
//        switch (data.source)
//        {
//            case ValueSource.Set:
//                value = data.value;
//                break;
//            case ValueSource.MySelf:
//                value = GetBaseValue(myself, data) * data.value;
//                break;
//            case ValueSource.Others:
//                value = GetBaseValue(other, data) * data.value;
//                break;
//            default:
//                break;
//        }
//        return value;
//    }

//    float GetBaseValue(UnitData source, PropertyData data) {
//        float value = 0;
//        switch (data.sourceType)
//        {
//            case ModifiedProperty.None:
//                break;
//            case ModifiedProperty.Hp:
//                value = source.NowHP;
//                break;
//            case ModifiedProperty.MaxHp:
//                value = source.MaxHP;
//                break;
//            case ModifiedProperty.Atk:
//                value = source.NowAtk;
//                break;
//            case ModifiedProperty.MaxAtk:
//                value = source.MaxAtk;
//                break;
//            case ModifiedProperty.Def:
//                value = source.NowDef;
//                break;
//            case ModifiedProperty.MaxDef:
//                value = source.MaxDef;
//                break;
//            case ModifiedProperty.MoveSpeed:
//                value = source.NowMoveSpeed;
//                break;
//            case ModifiedProperty.MaxMoveSpeed:
//                value = source.MaxMoveSpeed;
//                break;
//            case ModifiedProperty.CD:

//                break;
//            default:
//                break;
//        }
//        return value;
//    }

//}
