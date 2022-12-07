using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    Transform _target;
    [SerializeField]
    Transform _cameraPoint;
    [SerializeField]
    float _rotateSpeed = 0.1f;
    [SerializeField]
    float _minAngleX = -30.0f;
    [SerializeField]
    float _maxAngleX = 30.0f;

    float _angleX;
    float _angleY;
    float _distance;
    bool _init = false;

    void Start() {
        Init();
    }

    void Update() {


    }

    void OnDestroy() {
        InputManager.I.RightStcikEvent -= Rotate;
    }

    public void Init() {
        if (!_init)
        {
            if (_target == null || _cameraPoint == null)
            {
                Debug.LogError("Camera target or point is NULL!");
                return;
            }

            _distance = Vector3.Distance(_target.position, _cameraPoint.position);
            InputManager.I.RightStcikEvent += Rotate;

            _init = true;
        }
    }

    void Rotate(Vector2 v, InputManager.ActionState state) {
        if (state == InputManager.ActionState.Game)
        {
            _angleY += v.x * _rotateSpeed;
            _angleX -= v.y * _rotateSpeed;
            _angleX = Mathf.Clamp(_angleX, _minAngleX, _maxAngleX);

            Vector3 euler = new Vector3(_angleX, _angleY, 0);
            transform.eulerAngles = euler;
            CollisionToObject();
        }

    }

    void CollisionToObject() {
        Vector3 direction = transform.position - _target.position;
        RaycastHit hit;
        if (Physics.Raycast(_target.position, direction.normalized, out hit, _distance, ~(1 << 5)))
        {
#if UNITY_EDITOR
            //Debug.Log(hit.collider.name);
            //Debug.Log(hit.point);
            Debug.DrawRay(_target.position, direction.normalized, Color.red, Vector3.Distance(_target.position, hit.point), false);
#endif
            float dis = Vector3.Distance(_target.position, hit.point);
            if (Mathf.Abs(dis) < _distance)
            {
                transform.position = _target.position - transform.forward * dis;
                return;
            }
        }

        transform.position = _target.position - transform.forward * _distance;
    }

    void ResetAngle() {
        transform.rotation = _cameraPoint.rotation;
    }
}
