using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace BS.Enemy.Set
{
    public class SetFaintingState : ISetState
    {
        #region Variables
        private SetProperty property;
        private float fearDuration = 3f; // 공포 지속 시간
        private float elapsedTime = 0f;
        private Vector3 fearTargetPosition;

        public SetFaintingState(SetProperty property)
        {
            this.property = property;
        }
        #endregion

        public void Enter()
        {
            Debug.Log("기절 상태 진입!");
            property.Animator.SetBool(SetProperty.SET_ANIM_BOOL_FAINTING, true);
            SetRandomEscapePosition();
        }

        public void Update()
        {
            elapsedTime += Time.deltaTime;

            // 지속 시간이 끝나면 원래 상태로 복귀
            if (elapsedTime >= fearDuration)
            {
                float distance = Vector3.Distance(property.Player.position, property.Controller.transform.position);
                if (distance > property.Agent.stoppingDistance)
                {
                    property.Controller.SetState(new SetChaseState(property));
                }
                else
                {
                    property.Controller.SetState(new SetIdleState(property));
                }
                return;
            }

            // 목표 위치까지 도착하면 새로운 랜덤 위치 설정
            if (!property.Agent.pathPending && property.Agent.remainingDistance <= property.Agent.stoppingDistance)
            {
                SetRandomEscapePosition();
            }
        }

        public void Exit()
        {
            //기절상태에서 나갈때 공격시간을 초기화
            property.LastAttackTime = Time.time;

            property.Animator.SetBool(SetProperty.SET_ANIM_BOOL_FAINTING, false);
        }

        private void SetRandomEscapePosition()
        {
            Vector3 randomDirection = Random.insideUnitSphere * 5f; // 반경 5 유닛 내 랜덤 방향
            randomDirection += property.Controller.transform.position; // 현재 위치 기준

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
            {

                fearTargetPosition = hit.position;
                Debug.Log(fearTargetPosition + "?");
                property.Agent.isStopped = false;
                property.Agent.SetDestination(fearTargetPosition);
            }
        }
    }
}