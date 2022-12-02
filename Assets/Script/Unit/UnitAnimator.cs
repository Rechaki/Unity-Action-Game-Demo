using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    public float timeScale = 1.0f;

    [SerializeField]
    Animator _animator;

    AnimatorStateInfo _currentAnimation;

    public void Play(string name)
    {
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
}
