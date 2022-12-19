using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    public Transform parent;
    public float timeScale = 1.0f;

    public delegate void StateChange(string PrevState, string NewState);
    //public delegate void StateChange(CharacterState PrevState, CharacterState NewState);
    public StateChange OnStateChange;

    public string CurrentState { get; private set; }
    //public CharacterState CurrentState => _currentState;
    public AnimatorStateInfo CurrentAnimation => _currentAnimation;
    public AnimatorStateEvents AnimatorStateEvent => _animatorEvent;
    
    [SerializeField]
    Animator _animator;

    AnimatorStateEvents _animatorEvent;
    //CharacterState _currentState;
    AnimatorStateInfo _currentAnimation;

    List<string> _animatorStates = new List<string>();

    void Start() {
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
            return;
        }
        LoadStateName();
        _animatorEvent = _animator.GetBehaviour<AnimatorStateEvents>();
        //_currentState = CharacterState.None;
        _animatorEvent.OnEnter += OnStateEnter; 
    }

    void Update() {
        
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
        foreach (var state in _animatorStates)
        {
            if (stateInfo.IsName(state))
            {
                CurrentState = state;
                return;
            }
        }
        

    }

    void LoadStateName() {
        string path = "Data/" + _animator.runtimeAnimatorController.name;
        Debug.Log(path);
        TextAsset file = Resources.Load(path) as TextAsset;
        if (file != null)
        {
            List<string[]> data = new List<string[]>();
            System.IO.StringReader sr = new System.IO.StringReader(file.text);
            while (sr.Peek() >= 0)
            {
                string line = sr.ReadLine();
                data.Add(line.Split(','));
            }

            foreach (var line in data)
            {
                foreach (var item in line)
                {
                    _animatorStates.Add(item);
                }
            }
        }
        
    }
}
