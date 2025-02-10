using BS.Player;
using BS.Utility;
using System.Collections;
using UnityEngine;
using static BS.Utility.AudioUtility;

public class ChargingPunchSMB : StateMachineBehaviour
{
    PlayerState ps;
    public AudioClip chargingSound;
    AudioSource source;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ps == null) // 초기화되지 않았다면 캐싱
        {
            ps = FindFirstObjectByType<PlayerState>();
        }
        source = AudioUtility.CreateSFX(chargingSound, ps.transform.position, AudioGroups.Skill, pitch:1.5f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        source.Pause();
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
