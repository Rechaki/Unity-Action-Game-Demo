using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : EnemyUnit
{
    GameObject _area;

    public override void InitData() {
        base.InitData();

        if (Data.Type == EnemyType.None)
        {
            CreateAtkArea();
        }
    }

    void CreateAtkArea() {
        _area = new GameObject("AtkArea");
        _area.transform.parent = transform;
        _area.transform.localPosition = Vector3.zero;
        var collider = _area.AddComponent<SphereCollider>();
        collider.radius = Data.NowViewRadius;
        collider.isTrigger = true;

    }
}
