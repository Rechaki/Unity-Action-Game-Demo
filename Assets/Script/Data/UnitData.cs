using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData : BaseData
{
    public int NowHP { get; protected set; }
    public int NowMP { get; protected set; }
    public int NowAtk { get; protected set; }
    public int NowDef { get; protected set; }
    public float NowMoveSpeed { get; protected set; }
    public float NowAtkSpeed { get; protected set; }
    public float NowViewRadius { get; protected set; }
    public float NowViewAngle { get; protected set; }

    public float NowTurnSpeed { get; protected set; }

    public int MaxHP { get; protected set; }
    public int MaxMP { get; protected set; }
    public int MaxAtk { get; protected set; }
    public int MaxDef { get; protected set; }
    public float MaxMoveSpeed { get; protected set; }
    public float MaxAtkSpeed { get; protected set; }
    public string SkillID { get; protected set; }
}
