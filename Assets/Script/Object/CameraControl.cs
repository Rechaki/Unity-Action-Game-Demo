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
    float _lockonSpeed = 10.0f;
    [SerializeField]
    float _minAngleX = -30.0f;
    [SerializeField]
    float _maxAngleX = 30.0f;

    Transform _lockonTarget;
    float _angleX;
    float _angleY;
    float _distance;
    bool _init = false;
    bool _isLockon = false;

    const int CollisionLayers = ~(1 << 5);
    const int LockonLayers = (1 << 9);

    void Start() {
        Init();
    }

    void Update() {


    }

    void OnDestroy() {
        InputManager.I.RightStcikEvent -= Rotate;
        InputManager.I.RightTriggerevent -= LockonTarget;
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
            InputManager.I.RightTriggerevent += LockonTarget;

            _init = true;
        }
    }

    void Rotate(Vector2 v, InputManager.ActionState state) {
        if (state == InputManager.ActionState.Game && _isLockon == false)
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
        if (Physics.Raycast(_target.position, direction.normalized, out hit, _distance, CollisionLayers))
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

    void LockonTarget(float value, InputManager.ActionState state) {
        if (state == InputManager.ActionState.Game)
        {
            _isLockon = value > 0.5 ? true : false;
        }

        if (_isLockon)
        {
            List<Transform> targetables = new List<Transform>();
            Collider[] colliders = Physics.OverlapSphere(_target.position, _distance, LockonLayers);
            foreach (var collider in colliders)
            {
                if (InScreen(collider.transform.position))
                {
                    if (IsBlocked(collider.transform.position) == false)
                    {
                        targetables.Add(collider.transform);
                    }
                }
            }

            Vector3 _targetToLockonTarget = _lockonTarget.position - _target.position;
            //Vector3 _camToTarget = _lockonTarget.position - transform.position;
            //Vector3 _planarCamToTarget = Vector3.ProjectOnPlane(_camToTarget, Vector3.up);
            //Quaternion _lookRotation = Quaternion.LookRotation(_camToTarget, Vector3.up);
            //float dis = Vector3.Distance(_lockonTarget.position, _cameraPoint.position);
            //var textRect = pointTexts[index].GetComponent<RectTransform>();
            //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(textRect, screenPos, uiCamera, out uipos))
            //{
            //    textRect.anchoredPosition = uipos + m_offsetY;
            //}

            var newRo = Quaternion.LookRotation(_targetToLockonTarget, Vector3.up);
            transform.rotation = Quaternion.LookRotation(_targetToLockonTarget, Vector3.up);
            //transform.rotation = Quaternion.Lerp(transform.rotation, newRo, Time.deltaTime * _lockonSpeed);
            transform.position = _target.position - transform.forward * _distance;
            //transform.position = Vector3.Lerp(transform.position, _camToTarget * dis, Time.deltaTime);
        }
    }

    bool InScreen(Vector3 v) {
        var camera = transform.GetComponent<Camera>();
        Vector3 viewPos = camera.WorldToViewportPoint(v);
        if (viewPos.x < 0 || viewPos.x > 1)
        {
            return false;
        }
        else if (viewPos.y < 0 || viewPos.y > 1)
        {
            return false;
        }
        else if (viewPos.z < 0)
        {
            return false;
        }

        return true;
    }

    bool IsBlocked(Vector3 v) {
        Vector3 direction = v - transform.position;
        float radius = 0.2f;
        bool isBlocked = Physics.SphereCast(transform.position, radius, direction, out RaycastHit hit, direction.magnitude, CollisionLayers);

        return isBlocked;
    }

    void ResetAngle() {
        transform.rotation = _cameraPoint.rotation;
    }
}
