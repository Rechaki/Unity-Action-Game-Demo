using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    public float timeScale = 1.0f;

    [SerializeField]
    Animator _animator;

    string _currentAnimation = "Idle";

    void Update() {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Idle"))
        {
            _currentAnimation = "Idle";
        }
    }

    void OnAnimatorMove() {
        var deltaPosition = _animator.deltaPosition;

        transform.position += deltaPosition / Time.deltaTime;
    }

    public Vector3 DeltaPosition() {
        return _animator.deltaPosition;
    }

    public void Play(string name) {
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
            return;
        }

        if (_currentAnimation.Equals(name) == false)
        {
            _animator.Play(name);
            _currentAnimation = name;
        }

    }

    public void CrossFade(string name, float duration, int layer = -1) {
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
            return;
        }

        if (_currentAnimation.Equals(name) == false)
        {
            _animator.CrossFade(name, duration, layer);
            _currentAnimation = name;
        }
    }

    public void SetFloat(string name, float value) {
        _animator.SetFloat(name, value);
    }
}
