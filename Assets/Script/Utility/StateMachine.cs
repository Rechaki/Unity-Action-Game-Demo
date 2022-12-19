using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : StateMachineBehaviour
{
    public CharacterState state;
    public bool activating = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        activating = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        activating = false;
    }
}
