using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    public float moveSpeed;
    public float moveSmoothingSpeed;
    public float timeScale = 1.0f;
    [SerializeField]
    Rigidbody _rigidbody;
    
    Vector3 _inputShift = Vector3.zero;
    Vector3 _smoothInputShift = Vector3.zero;

    void Start()
    {

    }

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
            return;
        }
        _inputShift = DiagonalMoveNum(v);
        _rigidbody.velocity = _inputShift * moveSpeed * timeScale;

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
}
