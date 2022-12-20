using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateData
{
    public bool isClear;
    public bool isGameOver;
}

public struct CharacterBaseData
{
    public string id;
    public int hp;
    public int mp;
    public int lv;
    public int atk;
    public int def;
    public float moveSpeed;
    public float atkSpeed;
    public float turnSpeed;
    public float viewRadius;
    public float viewAngle;
    public string skillId;
}

public struct EnemyBaseData
{
    public string id;
    public EnemyType type;
    public int hp;
    public int mp;
    public int lv;
    public int atk;
    public int def;
    public float moveSpeed;
    public float atkSpeed;
    public float turnSpeed;
    public float viewRadius;
    public float viewAngle;
    public string skillId;
}

public class AnimationInfo
{
    public SingleAnimationData[] Animations { get; }
    public byte Priority { get; }

    public AnimationInfo(SingleAnimationData[] animations, byte priority = 0)
    {
        Animations = animations;
        Priority = priority;
    }

    public SingleAnimationData RandomPlay()
    {
        if (Animations.Length == 0)
        {
            return SingleAnimationData.Default;
        }

        int index = Random.Range(0, Animations.Length);
        return Animations[index];
    }
}

public struct SingleAnimationData
{
    public AnimationName Name { get; }
    public float Duration { get; }
    public byte Priority { get; }

    public static SingleAnimationData Null = new SingleAnimationData(AnimationName.None, 0);
    public static SingleAnimationData Default = new SingleAnimationData(AnimationName.Idle, 0);

    public SingleAnimationData(AnimationName name, float duration = 0, byte priority = 0)
    {
        Name = name;
        Duration = duration;
        Priority = priority;
    }

    public override string ToString() => $"{nameof(Name)}, {Duration}";
}
