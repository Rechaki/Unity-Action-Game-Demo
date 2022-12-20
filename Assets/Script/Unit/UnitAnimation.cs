using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : MonoBehaviour
{
    public Transform parent;
    public float timeScale = 1.0f;

    public delegate void StateChange(string PrevState, string NewState);
    //public delegate void StateChange(CharacterState PrevState, CharacterState NewState);
    public StateChange OnStateChange;

    public SingleAnimationData CurrentAnimation { get; private set; }
    public AnimatorStateEvents AnimatorStateEvent { get; private set; }

    [SerializeField]
    Animator _animator;
    [SerializeField]
    CharacterType _type = CharacterType.Human;
    [SerializeField]
    AnimationName _default = AnimationName.None;

    Dictionary<AnimationName, SingleAnimationData> _animations;

    void Start()
    {
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
            return;
        }

        AnimatorStateEvent = _animator.GetBehaviour<AnimatorStateEvents>();
        //_currentState = CharacterState.None;
        AnimatorStateEvent.OnEnter += OnStateEnter;
    }

    void Update()
    {

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
                if (data.Priority > CurrentAnimation.Priority)
                {
                    _animator.Play(name.ToString());
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
