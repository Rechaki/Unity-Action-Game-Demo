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
    float _targetHeight = 1.5f;
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
    Vector3 _lockonOffset = Vector3.right;

    float _angleX;
    float _angleY;
    float _distance;
    float _defaultZoom;
    int _lockonIndex;
    bool _init = false;
    bool _isLockon = false;
    Vector3 _targetPosition;

    List<Transform> _lockonTargets = new List<Transform>();

    const int COLLISION_LAYERS = ~(1 << 5 | 1 << 9 | 1 << 10);
    const int LOCKON_LAYERS = 1 << 9;

    void Start() {
        Init();
    }

    void Update() {
        _targetPosition = _target.position;
        _targetPosition.y = _targetHeight;
    }

    void OnDestroy() {
        InputManager.I.RightStcikEvent -= Rotate;
        InputManager.I.RightTriggerEvent -= LockonTarget;
        InputManager.I.leftShoulderEvent -= (state) => SwitchLockonTarget(true, state);
        InputManager.I.rightShoulderEvent -= (state) => SwitchLockonTarget(false, state);
    }

    public void Init() {
        if (_init == false)
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
            
            _distance = Vector3.Distance(_targetPosition, _cameraPoint.position);
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
        Vector3 direction = transform.position - _targetPosition;
        RaycastHit hit;
        if (Physics.Raycast(_targetPosition, direction.normalized, out hit, _distance, COLLISION_LAYERS))
        {
#if UNITY_EDITOR
            Debug.DrawRay(_targetPosition, direction.normalized, Color.red, Vector3.Distance(_targetPosition, hit.point), false);
#endif
            float dis = Vector3.Distance(_targetPosition, hit.point);
            if (Mathf.Abs(dis) < _distance)
            {
                transform.position = _targetPosition - transform.forward * dis;
                return;
            }
        }

        transform.position = _targetPosition - transform.forward * _distance;
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
            Collider[] colliders = Physics.OverlapSphere(_targetPosition, _lockonDistance, LOCKON_LAYERS);
            foreach (var collider in colliders)
            {
                var character = collider.GetComponent<ICharacter>();
                if (character != null)
                {
                    if (InScreen(character.lookatTarget.position))
                    {
                        if (IsBlocked(character.lookatTarget.position) == false)
                        {
                            newTargetables.Add(character.lookatTarget);
                        }
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
                Vector3 direction = _lockonTargets[_lockonIndex].position - _targetPosition;
                Vector3 targetToLockonTarget = direction - _lockonOffset;
                var to = Quaternion.LookRotation(targetToLockonTarget, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, to, Time.deltaTime * _lockonSpeed);
                _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _zoomIn, Time.deltaTime * _lockonSpeed);
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                _target.rotation = Quaternion.AngleAxis(angle, Vector3.up);

                //ロックオンのUIのポジション
                var screenPos = _camera.WorldToScreenPoint(_lockonTargets[_lockonIndex].position);
                screenPos.z = 0;
                GlobalMessenger<Vector2>.Launch(EventMsg.LockOnEnter, screenPos);
            }

            transform.position = _targetPosition - transform.forward * _distance;
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
        Vector3 direction = v - _targetPosition;
        float distance = Vector3.Distance(v, _targetPosition);
#if UNITY_EDITOR
        Debug.DrawRay(_targetPosition, direction.normalized, Color.yellow, distance, false);
#endif
        bool isBlocked = Physics.SphereCast(_targetPosition, radius, direction.normalized, out RaycastHit hit, distance, COLLISION_LAYERS);
        if (isBlocked)
        {
            Debug.Log(hit.transform.name);
        }

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
        Gizmos.DrawWireSphere(_targetPosition, _lockonDistance);
    }
#endif
}
