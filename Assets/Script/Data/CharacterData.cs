using UnityEngine;

public class CharacterData : UnitData
{
    public SkillData Skill { get; private set; }
    public event EventDataHandler<CharacterData> RefreshEvent;

    CharacterBaseData _baseData;

    public CharacterData(){}

    public CharacterData(CharacterBaseData baseData)
    {
        //_baseData = baseData;
        NowHP = baseData.hp;
        MaxHP = baseData.hp;
        NowMP = baseData.mp;
        MaxMP = baseData.mp;
        NowAtk = baseData.atk;
        MaxAtk = baseData.atk;
        NowDef = baseData.def;
        MaxDef = baseData.def;
        NowMoveSpeed = baseData.moveSpeed;
        MaxMoveSpeed = baseData.moveSpeed;
        NowAtkSpeed = baseData.atkSpeed;
        MaxAtkSpeed = baseData.atkSpeed;
        NowViewRadius = baseData.viewRadius;
        Skill = DataManager.I.GetSkillData(baseData.skillId);
        //Type = baseData.type;
    }

    ~CharacterData() {

    }

    public void OnHit(int damage) {
        NowHP -= damage;
        Update();
    }

    public void ModifiedChange(ModifiedProperty modified, float num) {
        switch (modified)
        {
            case ModifiedProperty.None:
                break;
            case ModifiedProperty.Hp:
                NowHP += (int)num;
                NowHP = NowHP < 0 ? 0 : NowHP;
                break;
            case ModifiedProperty.MaxHp:
                MaxHP += (int)num;
                MaxHP = MaxHP < 0 ? 0 : MaxHP;
                break;
            case ModifiedProperty.Atk:
                NowAtk += (int)num;
                NowAtk = NowAtk < 0 ? 0 : NowAtk;
                break;
            case ModifiedProperty.MaxAtk:
                MaxAtk += (int)num;
                MaxAtk = MaxAtk < 0 ? 0 : MaxAtk;
                break;
            case ModifiedProperty.Def:
                NowDef += (int)num;
                NowDef = NowDef < 0 ? 0 : NowDef;
                break;
            case ModifiedProperty.MaxDef:
                MaxDef += (int)num;
                MaxDef = MaxDef < 0 ? 0 : MaxDef;
                break;
            case ModifiedProperty.MoveSpeed:
                NowMoveSpeed += (int)num;
                NowMoveSpeed = NowMoveSpeed < 0 ? 0 : NowMoveSpeed;
                break;
            case ModifiedProperty.MaxMoveSpeed:
                MaxMoveSpeed += num;
                MaxMoveSpeed = MaxMoveSpeed < 0 ? 0 : MaxMoveSpeed;
                break;
            case ModifiedProperty.CD:
                break;
            default:
                break;
        }
        Update();
    }

    void Update() {
        RefreshEvent?.Invoke(this);
    }


}
