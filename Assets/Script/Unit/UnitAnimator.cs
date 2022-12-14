using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    public Transform parent;
    public float timeScale = 1.0f;

    public AnimatorStateInfo CurrentAnimation => _currentAnimation;
    public AnimatorStateEvents AnimatorStateEvent => _animatorEvent;

    [SerializeField]
    Animator _animator;

    AnimatorStateEvents _animatorEvent;
    AnimatorStateInfo _currentAnimation;

    void Start() {
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
            return;
        }

        _animatorEvent = _animator.GetBehaviour<AnimatorStateEvents>();
        _animatorEvent.OnEnter += OnStateEnter; 
    }

    void Update() {
        //_currentAnimation = _animator.GetCurrentAnimatorStateInfo(0);
    }

    void OnAnimatorMove()
    {
        if (parent == null)
        {
            Debug.LogWarning("Parent is NULL!!");
            return;
        }
        var deltaPosition = _animator.deltaPosition;
        parent.position += deltaPosition;
        //var deltaRotation = _animator.deltaRotation;
        //parent.localRotation = deltaRotation * parent.localRotation;
    }

    public void Play(string name) {
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
            return;
        }

        if (_currentAnimation.IsName(name) == false)
        {
            _animator.Play(name);
        }

    }

    public void CrossFade(string name, float duration, int layer = -1) {
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
            return;
        }

        if (_currentAnimation.IsName(name) == false)
        {
            _animator.CrossFade(name, duration, layer);
        }
    }

    public void SetBool(string name, bool value)
    {
        _animator.SetBool(name, value);
    }

    public void SetFloat(string name, float value) {
        _animator.SetFloat(name, value);
    }

    public void SetTrigger(string name) {
        _animator.SetTrigger(name);
    }

    void OnStateEnter(AnimatorStateInfo stateInfo, int layerIndex) {
        _currentAnimation = stateInfo;
    }
}
