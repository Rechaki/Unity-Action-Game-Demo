using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //public Material Material => _meshRenderer.material;

    [SerializeField]
    MeshRenderer _meshRenderer;
    [SerializeField]
    float _startValue = 1.25f;
    [SerializeField]
    float _endValue = 1.0f;
    [SerializeField]
    float _time = 0.767f;

    float _currentValue;
    float _changeValue;
    float _currentVelocity;
    bool _isDraw = false;

    void Start() {
        if (_meshRenderer == null)
        {
            Debug.LogError("MeshRenderer is NULL!");
            return;
        }
    }

    void Update() {
        _currentValue = _meshRenderer.material.GetFloat("_Fill");
        if (_isDraw)
        {
            _changeValue = Mathf.SmoothDamp(_currentValue, _endValue, ref _currentVelocity, Time.deltaTime);
        }
        else
        {
            _changeValue = Mathf.SmoothDamp(_currentValue, _startValue, ref _currentVelocity, Time.deltaTime);
        }
        _meshRenderer.material.SetFloat("_Fill", _changeValue);

    }

    public void DrawWeapon() {
        _isDraw = true;
    }

    public void SheathWeapon() {
        _isDraw = false;
    }

}
