using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum JumpState
{
    JumpStart,
    JumpLoop,
    JumpLand
}

public class OnClimbDownFence : StateMachineBehaviour
{
    public JumpState MyJumpState;

    private PlayerController _playerController;
    private MovementInput _movementInput;
    
    private static readonly int LandingFromFence = Animator.StringToHash("landingFromFence");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_playerController)
        {
            _playerController = animator.GetComponent<PlayerController>();
            _movementInput = animator.GetComponent<MovementInput>();
        }
        
        if(MyJumpState == JumpState.JumpStart)
            animator.SetBool(LandingFromFence, false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(MyJumpState != JumpState.JumpLoop) return;
        if(_movementInput.isGrounded)
            animator.SetBool(LandingFromFence, _movementInput.isGrounded);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (MyJumpState == JumpState.JumpLand)
        {
            _playerController.OnClimbDownFence();
            animator.SetBool(LandingFromFence, false);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
