using BS.Enemy.Set;
using UnityEngine;

public class SetChaseState : ISetState
{
    private SetProperty property;

    public SetChaseState(SetProperty property)
    {
        this.property = property;
    }

    public void Enter()
    {
        property.Animator.SetBool(SetProperty.SET_ANIM_BOOL_CHASE, true);
        property.Agent.isStopped = false;
    }

    public void Update()
    {
        // 플레이어 위치로 이동
        property.Agent.SetDestination(property.Player.position);

        // 공격 쿨타임 체크
        if (Time.time >= property.LastAttackTime + property.Controller.AttackCooldown)
        {
            property.Controller.SetState(new SetAttackState(property));
            return;
        }

        // 플레이어와의 거리 체크
        float distance = Vector3.Distance(property.Player.position, property.Controller.transform.position);
        if (distance < property.Agent.stoppingDistance)
        {
            property.Controller.SetState(new SetIdleState(property));
            return;
        }
    }

    public void Exit()
    {
        property.Animator.SetBool(SetProperty.SET_ANIM_BOOL_CHASE, false);
    }
}
