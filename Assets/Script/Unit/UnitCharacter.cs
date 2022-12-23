using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCharacter : MonoBehaviour
{
    public Transform lookatTarget => _lookatTarget;

    [SerializeField]
    Transform _lookatTarget;
    [SerializeField]
    Transform _eye;
    [SerializeField]
    UnitRotate _unitRotate;
    [SerializeField]
    UnitAnimation _unitAnimation;
    [SerializeField]
    float _viewAngle = 120f;
    [SerializeField]
    float _viewRadius = 5f;
    [SerializeField]
    int _viewRayNum = 20;
    [SerializeField]
    float _arriveRadius = 1f;
    [SerializeField]
    float _slowRadius = 2f;
    [SerializeField]
    ColliderEvents _damageCollider;

    protected Transform _target;
    Vector3 _directionToTarget;
    float _distanceToTarget;
    float _moveSpeed;
    float _thinkTimer;

    const int COLLISION_LAYERS = ~(1 << 5 | 1 << 9 | 1 << 10);
    const int LOCKON_LAYERS = 1 << 9;
    const float MAX_SPEED = 1f;
    const float THINK_TIME = 3f;
}
