using BS.Player;
using BS.PlayerInput;
using UnityEngine;

public class PlayerAttackSMB : StateMachineBehaviour
{
    PlayerState ps;
    PlayerInputActions m_Input;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ps == null) // 초기화되지 않았다면 캐싱
        {
            ps = FindFirstObjectByType<PlayerState>();
        }
        if (m_Input == null)
        {
            m_Input = FindFirstObjectByType<PlayerInputActions>();
        }

        // Combo 공격이 4번째 모션인 경우
        if (ps.comboAttackIndex == 4)
        {
            ps.comboAttackIndex = 1;
        }
        // Combo 공격이 1,2,3번째 모션인 경우
        else
        {
            ps.comboAttackIndex++;
        }
        animator.ResetTrigger("DoRun");
        animator.ResetTrigger("DoWalk");
        animator.ResetTrigger("DoSprint");
        animator.SetBool("IsMoving", false);
    }

    //OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (ps != null)
        {
            ps.isMovable = true;
        }
        animator.SetBool("IsAttacking", false);
        if (m_Input.RightClick)
        {
            if (m_Input.C)
            {
                animator.SetTrigger("DoWalk");
                animator.SetBool("IsWalking", true);
            }
            else if (m_Input.Shift)
            {
                animator.SetTrigger("DoSprint");
                animator.SetBool("IsSprinting", true);
            }
            else
            {
                animator.SetTrigger("DoRun");
                animator.SetBool("IsRun", true);
            }
        }
    }
}