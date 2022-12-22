using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : MonoBehaviour
{
    public Transform parent;
    public float timeScale = 1.0f;

    //public delegate void StateChange(string PrevState, string NewState);
    public delegate void StateChange(AnimationName PrevState, AnimationName NewState);
    public StateChange OnStateChange;

    public SingleAnimationData CurrentAnimation { get; private set; }
    public AnimatorStateEvents AnimatorStateEvent { get; private set; }

    [SerializeField]
    Animator _animator;
    [SerializeField]
    CharacterType _type = CharacterType.Human;
    [SerializeField]
    AnimationName _default = AnimationName.None;

    float _duration = 0;
    byte _priority = 0;
    Dictionary<AnimationName, SingleAnimationData> _animations;

    void Start()
    {
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
            return;
        }

        InitAnimationList();
        //AnimatorStateEvent = _animator.GetBehaviour<AnimatorStateEvents>();
        //AnimatorStateEvent.OnEnter += OnStateEnter;
    }

    void Update()
    {
        if (_duration > 0)
        {
            _duration -= Time.deltaTime;
            if (_duration <= 0)
            {
                _priority = 0;
                ChangeAnimation(_default);
            }
        }
    }

    void OnAnimatorMove()
    {
        if (parent == null)
        {
            Debug.LogWarning("Parent is NULL!!");
            return;
        }

        parent.position += _animator.deltaPosition;
        //var deltaRotation = _animator.deltaRotation;
        //parent.localRotation = deltaRotation * parent.localRotation;
    }

    void OnStateEnter(AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    void InitAnimationList()
    {
        if (DataManager.I.AnimationDic.TryGetValue(_type, out _animations) == false)
        {
            Debug.LogWarning($"No {_type} animation.");
        }
    }

    void ChangeAnimation(AnimationName newState) {
        OnStateChange?.Invoke(CurrentAnimation.Name, newState);
        CurrentAnimation = _animations[newState];
    }

    public void Play(AnimationName name)
    {
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
            return;
        }

        if (name == CurrentAnimation.Name)
        {
            return;
        }
        else
        {
            SingleAnimationData data;
            if (_animations.TryGetValue(name, out data))
            {
                if (data.Priority > _priority)
                {
                    _animator.Play(name.ToString());
                    ChangeAnimation(name);
                    _duration = data.Duration;
                    _priority = data.Priority;
                }
            }

        }

    }

    public void CrossFade(AnimationName name, float duration = 0.2f, int layer = 0)
    {
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
            return;
        }

        if (name == CurrentAnimation.Name)
        {
            return;
        }
        else
        {
            SingleAnimationData data;
            if (_animations.TryGetValue(name, out data))
            {
                if (data.Priority > CurrentAnimation.Priority)
                {
                    _animator.CrossFade(name.ToString(), duration, layer);
                    CurrentAnimation = data;
                    _duration = data.Duration;
                    _priority = data.Priority;
                }
            }

        }
    }

    public void SetBool(string name, bool value)
    {
        _animator.SetBool(name, value);
    }

    public void SetFloat(string name, float value)
    {
        _animator.SetFloat(name, value);
    }

    public void SetTrigger(string name)
    {
        _animator.SetTrigger(name);
    }

}
