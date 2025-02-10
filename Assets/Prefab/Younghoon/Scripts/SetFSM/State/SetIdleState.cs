using BS.Enemy.Set;
using UnityEngine;

public class SetIdleState : ISetState
{
    private SetProperty property;

    public SetIdleState(SetProperty property)
    {
        this.property = property;
    }

    public void Enter()
    {
        property.Animator.SetBool(SetProperty.SET_ANIM_BOOL_IDLE, true);
    }

    public void Update()
    {
        // 공격 쿨타임 체크
        if (Time.time >= property.LastAttackTime + property.Controller.AttackCooldown)
        {
            property.Controller.SetState(new SetAttackState(property));
            return;
        }

        // 플레이어와의 거리 체크
        float distance = Vector3.Distance(property.Player.position, property.Controller.transform.position);
        if (distance >= property.Agent.stoppingDistance)
        {
            property.Controller.SetState(new SetChaseState(property));
            return;
        }
    }

    public void Exit()
    {
        property.Animator.SetBool(SetProperty.SET_ANIM_BOOL_IDLE, false);
    }
}
