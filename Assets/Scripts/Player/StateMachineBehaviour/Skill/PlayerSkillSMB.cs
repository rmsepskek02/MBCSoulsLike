using BS.Player;
using BS.PlayerInput;
using UnityEngine;

public class PlayerSkillSMB : StateMachineBehaviour
{
    PlayerState ps;
    PlayerInputActions m_Input;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ps == null) // 초기화되지 않았다면 캐싱
        {
            ps = FindFirstObjectByType<PlayerState>();
        }
        if (m_Input == null)
        {
            m_Input = FindFirstObjectByType<PlayerInputActions>();
        }
        animator.SetBool("IsRun", false);
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
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
            ps.targetPosition = ps.transform.position;
            ps.currentSkillName = "";
            ps.canSkill = true;
            ps.isHit = false;
        }
        animator.SetBool("IsUppercuting", false);
        animator.SetBool("IsChargingPunch", false);
        animator.SetBool("IsBackHandSwing", false);

        if (m_Input.RightClick)
        {
            if (m_Input.C)
                animator.SetTrigger("DoWalk");
            else if (m_Input.Shift)
                animator.SetTrigger("DoSprint");
            else
                animator.SetTrigger("DoRun");
        }
    }
}