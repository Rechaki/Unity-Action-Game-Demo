using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    Camera _camera;
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
    [SerializeField]
    float _lockonDistance = 10.0f;
    [SerializeField]
    float _zoomIn = 40;
    [SerializeField]
    float _lockonOffsetX = 1.0f;

    float _angleX;
    float _angleY;
    float _distance;
    float _defaultZoom;
    int _lockonIndex;
    bool _init = false;
    bool _isLockon = false;

    List<Transform> _lockonTargets = new List<Transform>();

    const int CollisionLayers = ~(1 << 5 | 1 << 9 | 1 << 10);
    const int LockonLayers = 1 << 9;

    void Start() {
        Init();
    }

    void Update() {


    }

    void OnDestroy() {
        InputManager.I.RightStcikEvent -= Rotate;
        InputManager.I.RightTriggerEvent -= LockonTarget;
        InputManager.I.leftShoulderEvent -= (state) => SwitchLockonTarget(true, state);
        InputManager.I.rightShoulderEvent -= (state) => SwitchLockonTarget(false, state);
    }

    public void Init() {
        if (!_init)
        {
            if (_camera == null)
            {
                _camera = transform.GetComponent<Camera>();
                if (_camera == null)
                {
                    Debug.LogError("Camera is NUll!");
                    return;
                }
            }
            if (_target == null || _cameraPoint == null)
            {
                Debug.LogError("Camera target or point is NULL!");
                return;
            }

            _distance = Vector3.Distance(_target.position, _cameraPoint.position);
            _defaultZoom = _camera.fieldOfView;

            InputManager.I.RightStcikEvent += Rotate;
            InputManager.I.RightTriggerEvent += LockonTarget;
            InputManager.I.leftShoulderEvent += (state) => SwitchLockonTarget(true, state);
            InputManager.I.rightShoulderEvent += (state) => SwitchLockonTarget(false, state);

            _init = true;
        }
    }

    void Rotate(Vector2 v, InputManager.ActionState state) {
        if (state == InputManager.ActionState.Game && _isLockon == false)
        {
            _angleY += v.x * _rotateSpeed;
            _angleX -= v.y * _rotateSpeed;
            _angleX = Mathf.Clamp(_angleX, _minAngleX, _maxAngleX);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_angleX, _angleY, 0), Time.deltaTime * _lockonSpeed);
            CollisionToObject();
        }

    }

    void CollisionToObject() {
        Vector3 direction = transform.position - _target.position;
        RaycastHit hit;
        if (Physics.Raycast(_target.position, direction.normalized, out hit, _distance, CollisionLayers))
        {
#if UNITY_EDITOR
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
            //Debug.Log(transform.forward);
            List<Transform> newTargetables = new List<Transform>();
            Collider[] colliders = Physics.OverlapSphere(_target.position, _lockonDistance, LockonLayers);
            foreach (var collider in colliders)
            {
                if (InScreen(collider.transform.position))
                {
                    if (IsBlocked(collider.transform.position) == false)
                    {
                        newTargetables.Add(collider.transform);
                    }
                }
            }

            if (_lockonTargets.Count == 0)
            {
                _lockonTargets.AddRange(newTargetables);
                _lockonIndex = 0;
            }
            else if (newTargetables.Count > 0)
            {
                var notNewList = _lockonTargets.Except(newTargetables).ToList();
                var notCurrList = newTargetables.Except(_lockonTargets).ToList();
                var currLockonTarget = _lockonTargets[_lockonIndex];
                foreach (var target in notNewList)
                {
                    _lockonTargets.Remove(target);
                }
                foreach (var target in notCurrList)
                {
                    _lockonTargets.Add(target);
                }

                _lockonIndex = _lockonTargets.IndexOf(currLockonTarget);
                _lockonIndex = _lockonIndex == -1 ? 0 : _lockonIndex;
            }

            if (_lockonTargets.Count > 0)
            {
                Vector3 targetToLockonTarget = _lockonTargets[_lockonIndex].position - _target.position;
                var to = Quaternion.LookRotation(targetToLockonTarget, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, to, Time.deltaTime * _lockonSpeed);
                _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomIn, Time.deltaTime * _lockonSpeed);

                var screenPos = _camera.WorldToScreenPoint(_lockonTargets[_lockonIndex].position);
                screenPos.z = 0;
                GlobalMessenger<Vector2>.Launch(EventMsg.LockOnEnter, screenPos);
            }

            //if (transform.forward.z >= 0)
            //{
            //    transform.position = _target.position - transform.forward * _distance + Vector3.right * _lockonOffsetX;
            //}
            //else
            //{
            //    transform.position = _target.position - transform.forward * _distance + Vector3.left * _lockonOffsetX;
            //}
            transform.position = _target.position - transform.forward * _distance;

        }
        else
        {
            if (_lockonTargets.Count > 0)
            {
                _lockonTargets.Clear();
            }
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _defaultZoom, Time.deltaTime * _lockonSpeed);
            GlobalMessenger.Launch(EventMsg.LockOnExit);
        }

    }

    bool InScreen(Vector3 v) {
        Vector3 viewPos = _camera.WorldToViewportPoint(v);
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

    bool IsBlocked(Vector3 v)
    {
        float radius = 0.2f;
        Vector3 direction = v - _target.position;
        float distance = Vector3.Distance(v, _target.position);
#if UNITY_EDITOR
        Debug.DrawRay(_target.position, direction.normalized, Color.yellow, distance, false);
#endif
        bool isBlocked = Physics.SphereCast(_target.position, radius, direction.normalized, out RaycastHit hit, distance, CollisionLayers);

        return isBlocked;
    }

    void SwitchLockonTarget(bool left, InputManager.ActionState state)
    {
        if (state == InputManager.ActionState.Game && _isLockon)
        {
            if (_lockonTargets.Count > 0)
            {
                if (left)
                {
                    _lockonIndex = _lockonIndex > 0 ? --_lockonIndex : _lockonTargets.Count - 1;
                }
                else
                {
                    _lockonIndex = _lockonIndex < _lockonTargets.Count - 1 ? ++_lockonIndex : 0;
                }

            }
        }
    }

    void ResetAngle() {
        transform.rotation = _cameraPoint.rotation;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_target.position, _lockonDistance);
    }
#endif
}
