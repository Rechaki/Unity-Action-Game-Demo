using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRotate : MonoBehaviour
{
    public Camera characterCamera;
    public float rotateSpeed = 1f;
    public float timeScale = 1f;

    float _tagetAngle;
    bool _canRotate = true;

    public void Rotate(Vector2 v) {
        if (_canRotate == false)
        {
            return;
        }

        if (v.x != 0.0f || v.y != 0.0f)
        {

            if (characterCamera == null)
            {
                Debug.LogWarning("Character Camera is NULL!");
                return;
            }

            var dir = v.x * characterCamera.transform.right + v.y * characterCamera.transform.forward;
            dir.Normalize();
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * timeScale * Time.deltaTime);

        }

    }

    public void Rotate() {
        if (_canRotate == false || DoneRotate())
        {
            return;
        }

        Quaternion rotation = Quaternion.AngleAxis(_tagetAngle, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * timeScale * Time.deltaTime);
    }

    bool DoneRotate() {
        return Mathf.Abs(transform.rotation.eulerAngles.y - _tagetAngle) < 1f;
    }

    public void RotateTo(float angle) {
        _tagetAngle = angle;
    }

    public void RotateTo(float x, float z) {
        _tagetAngle = Mathf.Atan2(x, z) * 180.00f / Mathf.PI;
    }

    public void RotateBy(float angle) {
        _tagetAngle = transform.rotation.eulerAngles.y + angle;
    }

    public void EnableRotate() {
        _canRotate = true;
    }

    public void DisableRotate() {
        _canRotate = false;
        _tagetAngle = transform.rotation.eulerAngles.y;
    }
}
