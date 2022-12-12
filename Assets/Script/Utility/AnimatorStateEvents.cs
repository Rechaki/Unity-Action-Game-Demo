using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateEvents : StateMachineBehaviour
{
    public delegate void StateEvent(AnimatorStateInfo stateInfo, int layerIndex);
    public event StateEvent OnEnter;
    public event StateEvent OnUpdate;
    public event StateEvent OnExit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        OnEnter?.Invoke(stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        OnUpdate?.Invoke(stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        OnExit?.Invoke(stateInfo, layerIndex);
    }

}
