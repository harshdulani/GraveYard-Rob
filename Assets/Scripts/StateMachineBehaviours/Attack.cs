﻿using UnityEngine;

public enum AttackType
{
    LightAttack,
    HeavyAttack,
    Digging
};

public class Attack : StateMachineBehaviour
{
    public AttackType attackType;

    private PlayerCombat _playerCombat;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_playerCombat)
            _playerCombat = animator.GetComponent<PlayerCombat>();
        
        switch (attackType)
        {
            case AttackType.LightAttack:
            case AttackType.HeavyAttack:
                _playerCombat.CompleteAttack();
                break;
            case AttackType.Digging:
                _playerCombat.CompleteDigging();
                break;
        }

        Debug.Log(stateInfo.IsName("HeavyAttack"));
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
