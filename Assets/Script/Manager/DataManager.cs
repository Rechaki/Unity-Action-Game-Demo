using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public PlayerData PlayerData { get; private set; }
    public bool GameOver { get; private set; }

    Dictionary<string, CharacterBaseData> _characterDic = new Dictionary<string, CharacterBaseData>();
    Dictionary<string, EnemyBaseData> _enemyDic = new Dictionary<string, EnemyBaseData>();
    Dictionary<string, SkillData> _skillDic = new Dictionary<string, SkillData>();
    Dictionary<string, BuffData> _buffDic = new Dictionary<string, BuffData>();
    GameStateData _stateData;
    bool _inited = false;

    public void Init() {
        if (!_inited)
        {
            Load();

            GameStateInit();
            PlayerDataInit();

            _inited = true;
        }
    }

    public void Load() {
        CharacterBaseDataInit();
        EnemyDataInit();
        BuffDataInit();
        SkillDataInit();
    }

    public void Unload() {

    }

    public CharacterData GetCharacterData(string id) {
        CharacterData data = new CharacterData();
        CharacterBaseData baseData;
        if (_characterDic.TryGetValue(id, out baseData))
        {
            data = new CharacterData(baseData);
        }
        else
        {
            Debug.LogError($"No character data found for id: {id}");
        }

        return data;
    }

    public EnemyData GetEnemyData(string id) {
        EnemyData data = new EnemyData();
        EnemyBaseData baseData;
        if (_enemyDic.TryGetValue(id, out baseData))
        {
            data = new EnemyData(baseData);
        }
        else
        {
            Debug.LogError($"No enemy data found for id: {id}");
        }
        return data;
    }

    public SkillData GetSkillData(string id) {
        SkillData data = new SkillData();
        if (!_skillDic.TryGetValue(id, out data))
        {
            Debug.LogError($"No skill data found for id: {id}");
        }
        return data;
    }

    void GameStateInit() {
        _stateData = new GameStateData();
        _stateData.isClear = false;
        _stateData.isGameOver = false;
        GameOver = _stateData.isGameOver;

        GlobalMessenger.AddListener(EventMsg.GameOver, SetGameOver);
    }

    void SetGameOver() {
        GameOver = true;
    }

    void PlayerDataInit() {
        PlayerData = new PlayerData();
    }

    void CharacterBaseDataInit() {
        var data = ResourceManager.I.ReadFile(AssetPath.CHARACTER_DATA);
        foreach (var item in data)
        {
            if (item.Length > 0)
            {
                CharacterBaseData character = new CharacterBaseData();
                character.id = item[0];
                character.hp = int.Parse(item[1]);
                character.mp = int.Parse(item[2]);
                character.lv = int.Parse(item[3]);
                character.atk = int.Parse(item[4]);
                character.def = int.Parse(item[5]);
                character.moveSpeed = float.Parse(item[6]);
                character.atkSpeed = float.Parse(item[7]);
                character.viewRadius = float.Parse(item[8]);
                character.skillId = item[9];
                _characterDic.Add(character.id, character);
            }
        }
    }

    void EnemyDataInit() {
        var data = ResourceManager.I.ReadFile(AssetPath.ENEMY_DATA);
        foreach (var item in data)
        {
            if (item.Length > 0)
            {
                EnemyBaseData enemy = new EnemyBaseData();
                enemy.id = item[0];
                enemy.type = (EnemyType)int.Parse(item[1]);
                enemy.hp = int.Parse(item[2]);
                enemy.mp = int.Parse(item[3]);
                enemy.lv = int.Parse(item[4]);
                enemy.atk = int.Parse(item[5]);
                enemy.def = int.Parse(item[6]);
                enemy.moveSpeed = float.Parse(item[7]);
                enemy.atkSpeed = float.Parse(item[8]);
                enemy.turnSpeed = float.Parse(item[9]);
                enemy.viewRadius = float.Parse(item[10]);
                enemy.viewAngle = float.Parse(item[11]);
                enemy.skillId = item[12];
                _enemyDic.Add(enemy.id, enemy);
            }
        }
    }

    void BuffDataInit() {
        var data = ResourceManager.I.ReadFile(AssetPath.BUFF_DATA);
        foreach (var item in data)
        {
            if (item.Length > 0)
            {
                BuffData buff = new BuffData();
                buff.id = item[0];
                buff.name = item[1];
                buff.durationTime = float.Parse(item[2]);
                buff.attributes = (BuffAttributes)int.Parse(item[3]);
                buff.modifiedProperties = new List<PropertyData>();
                var propertyData = item[4].Split(";");
                if (propertyData.Length > 0)
                {
                    PropertyData property = new PropertyData();
                    property.property = (ModifiedProperty)int.Parse(propertyData[0]);
                    property.source = (ValueSource)int.Parse(propertyData[1]);
                    property.sourceType = (ModifiedProperty)int.Parse(propertyData[2]);
                    property.value = float.Parse(propertyData[3]);
                    buff.modifiedProperties.Add(property);
                }
                _buffDic.Add(buff.id, buff);
            }
        }
    }

    void SkillDataInit() {
        var data = ResourceManager.I.ReadFile(AssetPath.SKILL_DATA);
        foreach (var item in data)
        {
            if (item.Length > 0)
            {
                SkillData skill = new SkillData();
                skill.id = item[0];
                skill.name = item[1];
                skill.description = item[2];
                skill.cd = float.Parse(item[3]);
                skill.distance = float.Parse(item[4]);
                skill.angle = float.Parse(item[5]);
                skill.attackType = (SkillBehavior)int.Parse(item[6]);
                skill.buffs = new List<BuffData>();
                var buffIDs = item[7].Split(";");
                if (buffIDs.Length > 0)
                {
                    foreach (var id in buffIDs)
                    {
                        BuffData buffData;
                        if (_buffDic.TryGetValue(id, out buffData))
                        {
                            skill.buffs.Add(buffData);
                        }
                        else
                        {
                            Debug.LogError($"No buff data found for id: {id}");
                        }
                    }
                }
                skill.vfxName = item[8];
                skill.hitVfxName = item[9];
                skill.animationName = item[10];
                _skillDic.Add(skill.id, skill);
            }
        }
    }

}
