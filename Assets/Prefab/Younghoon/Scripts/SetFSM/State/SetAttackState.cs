using BS.Player;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;

namespace BS.Enemy.Set
{
    public class SetAttackState : ISetState
    {
        private SetProperty property;
        private bool isAttacking;

        private AnimatorStateInfo currentState;

        private string lastAttack = string.Empty; // 마지막 공격을 저장할 변수

        public SetAttackState(SetProperty property)
        {
            this.property = property;
        }

        public void Enter()
        {
            property.Animator.SetBool(SetProperty.SET_ANIM_BOOL_ATTACK, true);
            property.Agent.isStopped = true;
        }

        public void Update()
        {
            // 플레이어와의 거리 계산
            float distance = Vector3.Distance(property.Player.position, property.Controller.transform.position);

            if (!isAttacking)
            {
                // 거리에 따라 공격 패턴 선택
                SelectAndPerformAttack(distance);
                //PerformMidRangeAttack();
                //PerformLongRangeAttack();
                //PerformCloseRangeAttack();
                //PerformSpecialAttack();
            }

            // 현재 애니메이션 상태 업데이트
            currentState = property.Animator.GetCurrentAnimatorStateInfo(0);

            // 애니메이션 타임을 체크하여 공격이 끝났다면 상태 전환
            if (AttackStateChecker() && currentState.normalizedTime >= 0.9f)
            {
                if (distance > property.Agent.stoppingDistance)
                {
                    property.Controller.SetState(new SetChaseState(property));
                }
                else
                {
                    property.Controller.SetState(new SetIdleState(property));
                }
            }
            //if (AttackStateChecker())
            //{
            //    // 애니메이션 타임을 체크하여 공격이 끝났다면 상태 전환
            //    if (currentState.normalizedTime >= 0.9f)
            //    {
            //        property.Controller.SetState(new SetChaseState(property));
            //    }
            //}
        }

        public void Exit()
        {
            ResetAttackState();
        }

        /// <summary>
        /// 플레이어와의 거리에 따라 공격 패턴 선택 및 수행
        /// </summary>
        /// <summary>
        /// 플레이어와의 거리에 따라 공격 패턴 선택 및 수행
        /// </summary>
        private void SelectAndPerformAttack(float distance)
        {
            isAttacking = true;

            // 거리와 공격 유형, 실행 메서드 매핑
            var attackCandidates = new List<(float range, string type, Action perform)>
            {
                (property.CloseRange, SetProperty.SET_ANIM_TRIGGER_SLASHATTACKTHREETIMES, PerformCloseRangeAttack),
                (property.MidRange, SetProperty.SET_ANIM_TRIGGER_PULLATTACK, PerformMidRangeAttack),
                (property.LongRange, SetProperty.SET_ANIM_TRIGGER_LIGHTNINGMAGIC, PerformLongRangeAttack),
                (float.MaxValue, SetProperty.SET_ANIM_TRIGGER_ROAR, PerformSpecialAttack) // 특수 공격은 거리 제한 없음
            };

            // 가능한 공격을 필터링
            var availableAttacks = attackCandidates
                .Where(a => distance <= a.range && property.LastAttackType != a.type)
                .ToList();

            // 공격 실행
            if (availableAttacks.Count > 0)
            {
                var selectedAttack = availableAttacks[UnityEngine.Random.Range(0, availableAttacks.Count)];
                selectedAttack.perform?.Invoke(); // 공격 실행
                property.LastAttackType = selectedAttack.type; // 마지막 공격 타입 갱신
            }
            else
            {
                property.LastAttackType = null;
                SelectAndPerformAttack(distance);
                Debug.LogWarning("No valid attack available.");
            }
        }

        private void PerformCloseRangeAttack()
        {
            TriggerAttackAnimation(SetProperty.SET_ANIM_TRIGGER_SLASHATTACKTHREETIMES);
        }

        private void PerformMidRangeAttack()
        {
            TriggerAttackAnimation(SetProperty.SET_ANIM_TRIGGER_PULLATTACK);
        }

        private void PerformLongRangeAttack()
        {
            TriggerAttackAnimation(SetProperty.SET_ANIM_TRIGGER_LIGHTNINGMAGIC);
        }

        private void PerformSpecialAttack()
        {
            TriggerAttackAnimation(SetProperty.SET_ANIM_TRIGGER_ROAR);
        }

        private void TriggerAttackAnimation(string triggerName)
        {
            property.Animator.SetTrigger(triggerName);
        }

        private bool AttackStateChecker()
        {
            // 여러 애니메이션 트리거를 체크하여 상태를 확인
            return currentState.IsName(SetProperty.SET_ANIM_TRIGGER_SLASHATTACK) ||
                   currentState.IsName(SetProperty.SET_ANIM_TRIGGER_SLASHATTACKTHREETIMES) ||
                   currentState.IsName(SetProperty.SET_ANIM_TRIGGER_PULLATTACK) ||
                   currentState.IsName(SetProperty.SET_ANIM_TRIGGER_LIGHTNINGMAGIC) ||
                   currentState.IsName(SetProperty.SET_ANIM_TRIGGER_ROAR);
        }

        private void ResetAttackState()
        {
            property.LastAttackTime = Time.time;

            property.Animator.SetBool(SetProperty.SET_ANIM_BOOL_ATTACK, false);
            ResetAllAttackTriggers();
        }

        private void ResetAllAttackTriggers()
        {
            property.Animator.ResetTrigger(SetProperty.SET_ANIM_TRIGGER_SLASHATTACK);
            property.Animator.ResetTrigger(SetProperty.SET_ANIM_TRIGGER_SLASHATTACKTHREETIMES);
            property.Animator.ResetTrigger(SetProperty.SET_ANIM_TRIGGER_PULLATTACK);
            property.Animator.ResetTrigger(SetProperty.SET_ANIM_TRIGGER_LIGHTNINGMAGIC);
            property.Animator.ResetTrigger(SetProperty.SET_ANIM_TRIGGER_ROAR);
        }
    }
}
