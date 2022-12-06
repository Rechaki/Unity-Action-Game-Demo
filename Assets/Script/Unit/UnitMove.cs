using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    public float moveSpeed;
    public float moveSmoothingSpeed;
    public float footRayLength = 0.1f;
    public float slopeMinAngle = 0f;
    public float slopeMaxAngle = 45f;
    public float timeScale = 1.0f;

    [SerializeField]
    Rigidbody _rigidbody;
    [SerializeField]
    Transform _foot;

    bool isAir = false;
    Vector3 _inputShift = Vector3.zero;
    Vector3 _smoothInputShift = Vector3.zero;
    Vector3 _footHitNormal = Vector3.zero;

    public void MoveWithPosition(Vector2 v)
    {
        _inputShift = DiagonalMoveNum(v);
        _smoothInputShift = Vector3.Lerp(_smoothInputShift, _inputShift, Time.deltaTime * moveSmoothingSpeed);
        transform.position += _smoothInputShift * moveSpeed * Time.deltaTime * timeScale;
    }

    public void MoveWithRigidbody(Vector2 v)
    {
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody is NULL !");
        }

        _inputShift = DiagonalMoveNum(v);
        if (OnSlope())
        {
            _inputShift = Vector3.ProjectOnPlane(_inputShift, _footHitNormal).normalized;
        }
        _rigidbody.velocity = _inputShift * moveSpeed * timeScale;
    }

    public void Roll() {
        //_rigidbody.AddForce(transform.forward * 10, ForceMode.Impulse);
    }

    Vector3 DiagonalMoveNum(Vector2 v)
    {
        float x = v.x;
        float y = v.y;
        if (x * y != 0)
        {
            x = x * Mathf.Sqrt(1 - (y * y) / 2.0f);
            y = y * Mathf.Sqrt(1 - (x * x) / 2.0f);
        }
        return new Vector3(x, 0, y);
    }

    bool OnSlope() {
        if (_foot == null)
        {
            Debug.LogWarning("Foot is NULL!");
            return false;
        }
        Ray ray = new Ray(_foot.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, footRayLength))
        {
#if UNITY_EDITOR
            Debug.DrawRay(_foot.position, hit.normal, Color.black, 0, false);
#endif
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            //Debug.Log(angle);
            if (angle > slopeMinAngle && angle < slopeMaxAngle)
            {
                _footHitNormal = hit.normal;
                return true;
            }
        }
        return false;
    }

    bool IsAir() {
        Ray ray = new Ray(_foot.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5.0f))
        {
#if UNITY_EDITOR
            Debug.DrawRay(_foot.position, Vector3.down, Color.red, 0, false);
#endif
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            //Debug.Log(angle);
            if (angle > slopeMinAngle && angle < slopeMaxAngle)
            {
                _footHitNormal = hit.normal;
                return true;
            }
        }
        return true;
    }
}
