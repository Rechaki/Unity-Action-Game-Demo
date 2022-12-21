using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ICharacter
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
    Transform _target;
    float _moveSpeed;

    const int COLLISION_LAYERS = ~(1 << 5 | 1 << 9 | 1 << 10);
    const int LOCKON_LAYERS = 1 << 9;
    const float MAX_SPEED = 1f;

    void Start() {
        if (_lookatTarget == null)
        {
            Debug.LogError("Look at target is null.");
        }
        else if (_eye == null)
        {
            Debug.LogError("eye is null.");
        }

    }

    void Update() {
        if (_target == null)
        {
            SetFireView();
        }
        else
        {
            ArriveMove();

        }
    }

    void SetFireView() {
        Vector3 farLeftRayPos = Quaternion.Euler(0, -_viewAngle / 2, 0) * _eye.forward * _viewRadius;
        for (int i = 0; i <= _viewRayNum; i++)
        {
            Vector3 rayPos = Quaternion.Euler(0, (_viewAngle / _viewRayNum) * i, 0) * farLeftRayPos;
            Ray ray = new Ray(transform.position, rayPos);
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(ray, out hit, _viewRadius, COLLISION_LAYERS);

            Vector3 pos = _eye.position + rayPos;
            if (hit.transform != null)
            {
                pos = hit.point;
            }
#if UNITY_EDITOR
            Debug.DrawLine(_eye.position, pos, Color.red); ;
#endif
            if (hit.transform != null)
            {
                if (hit.transform.tag == "Player")
                {
                    _target = hit.transform;
                }
            }

        }

    }

    void ArriveMove() {
        var direction = _target.position - transform.position;
        var distance = Vector3.Distance(_target.position, transform.position);

        if (distance < _arriveRadius)
        {
            _moveSpeed = 0;
        }
        else
        {
            if (distance > _slowRadius)
            {
                _moveSpeed = MAX_SPEED;
            }
            else
            {
                _moveSpeed = MAX_SPEED * distance / _slowRadius;
            }
        }

        direction.Normalize();
        _unitRotate.RotateTo(direction.x, direction.z);
        _unitRotate.Rotate();
        _unitAnimation.SetFloat("Move", _moveSpeed);

    }

}
