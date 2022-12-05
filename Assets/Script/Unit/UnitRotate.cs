using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRotate : MonoBehaviour
{
    public float rotateSpeed;
    public float timeScale;

    float _tagetAngle;
    bool _canRotate = true;

    public void Rotate(Vector2 v) {
        if (_canRotate == false)
        {
            return;
        }

        if (v.x != 0 || v.y != 0)
        {
            float angle = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * timeScale * Time.deltaTime);
        }
    }

    public void Rotate() {
        if (_canRotate == false || DoneRotate())
        {
            return;
        }

        float currAngle = transform.rotation.eulerAngles.y;
        if (currAngle > 180.00f)
        {
            currAngle -= 360.00f;
        }
        float diffAngle = _tagetAngle - currAngle;
        float reverseAngle = _tagetAngle > currAngle ? (_tagetAngle - 360.00f - currAngle) : (_tagetAngle + 360.00f - currAngle);
        bool counterclockwise = Mathf.Abs(reverseAngle) > Mathf.Abs(diffAngle)  ? (diffAngle < 0) : (reverseAngle < 0);  
        float rotSpeed = Mathf.Min(rotateSpeed * timeScale * Time.deltaTime, Mathf.Abs(diffAngle), Mathf.Abs(reverseAngle));
        if (counterclockwise)
        {
            rotSpeed *= -1;
        }
        transform.Rotate(new Vector3(0, rotSpeed, 0));
    }

    bool DoneRotate() {
        return Mathf.Abs(transform.rotation.eulerAngles.y - _tagetAngle) < Mathf.Min(0.01f, rotateSpeed * Time.deltaTime);
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
