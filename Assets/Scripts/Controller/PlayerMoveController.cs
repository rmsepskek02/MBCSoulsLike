using DG.Tweening;
using UnityEngine;

namespace BS.Player
{
    public class PlayerMoveController : PlayerController
    {
        [Header("Dash Settings")]
        public float dashDuration = 0.5f;
        public float dashCoolTime = 3f;
        public float dashDistance = 5f;
        public float invincibilityDuration = 0.5f;

        [Header("Collision Settings")]
        public float stopDistance = 1f; // 벽과 최소 거리 유지

        private PlayerSkillController psk;

        private static readonly string IS_MOVING = "IsMoving";
        private static readonly string IS_RUNNING = "IsRun";
        private static readonly string IS_WALKING = "IsWalking";
        private static readonly string IS_SPRINTING = "IsSprinting";
        private static readonly string IS_ATTACKING = "IsAttacking";
        private static readonly string IS_DASH = "IsDashing";
        private static readonly string DO_RUN = "DoRun";
        private static readonly string DO_WALK = "DoWalk";
        private static readonly string DO_SPRINT = "DoSprint";

        protected override void Awake()
        {
            base.Awake();
            psk = FindFirstObjectByType<PlayerSkillController>();
        }

        protected override void Start()
        {
            base.Start();
        }

        private void OnEnable()
        {
            if (!psk.skillList.ContainsKey("Space"))
            {
                psk.skillList.Add("Space", new Skill("Dash", dashCoolTime, DoDash));
            }
        }

        protected override void Update()
        {
            SetTargetPosition();
            MoveToTargetPos();
        }

        // Target Position 설정
        private void SetTargetPosition()
        {
            if (m_Input.RightClick && ps.isMovable
                && animator.GetBool(IS_DASH) == false
                && animator.GetBool("IsBlocking") == false)
            {
                ps.targetPosition = GetMousePosition();
                RotatePlayer();

                if (!animator.GetBool(IS_MOVING))
                {
                    animator.SetBool(IS_MOVING, true);
                    if (!animator.GetBool(IS_RUNNING))
                    {
                        animator.SetTrigger(DO_RUN);
                        animator.SetBool(IS_RUNNING, true);
                    }
                }
            }
            SetMoveState();
        }

        // Player 이동 (벽 충돌 감지 추가)
        private void MoveToTargetPos()
        {
            if (animator.GetBool(IS_MOVING)
                && !ps.isUppercuting &&
                !ps.isBackHandSwinging && !ps.isChargingPunching
                && !animator.GetBool(IS_ATTACKING)
                && animator.GetBool(IS_DASH) == false
                && animator.GetBool("IsBlocking") == false)
            {
                Vector3 moveDirection = (ps.targetPosition - transform.position).normalized;
                float moveSpeed = ps.inGameMoveSpeed * Time.deltaTime;

                // Ray 발사 위치 (캐릭터 중심이 아니라 1 유닛 위)
                Vector3 rayOrigin = transform.position + Vector3.up;

                // 벽 충돌 감지
                if (Physics.Raycast(rayOrigin, moveDirection, out RaycastHit hit, moveSpeed + stopDistance, LayerMask.GetMask("Wall")))
                {
                    Debug.DrawRay(rayOrigin, moveDirection * stopDistance, Color.red);

                    if (hit.distance <= stopDistance)
                    {
                        StopMovement();
                        return;
                    }

                    moveSpeed = hit.distance - stopDistance;
                }
                else
                {
                    Debug.DrawRay(rayOrigin, moveDirection * stopDistance, Color.green);
                }

                transform.position += moveDirection * moveSpeed;

                if (Vector3.Distance(transform.position, ps.targetPosition) < 0.05f)
                {
                    StopMovement();
                }
            }
        }

        private void StopMovement()
        {
            animator.ResetTrigger(DO_RUN);
            animator.ResetTrigger(DO_WALK);
            animator.ResetTrigger(DO_SPRINT);
            animator.SetBool(IS_MOVING, false);
            animator.SetBool(IS_RUNNING, false);
            animator.SetBool(IS_WALKING, false);
            animator.SetBool(IS_SPRINTING, false);
        }

        // Player 상태 변경 (걷기/뛰기/달리기)
        private void SetMoveState()
        {
            if (m_Input.C && !animator.GetBool(IS_WALKING) && animator.GetBool(IS_MOVING))
            {
                ChangeMoveState(0.5f, DO_WALK, IS_WALKING);
            }
            else if (m_Input.Shift && !animator.GetBool(IS_SPRINTING) && animator.GetBool(IS_MOVING))
            {
                if (!(m_Input.C && m_Input.Shift))
                {
                    ChangeMoveState(2f, DO_SPRINT, IS_SPRINTING);
                }
            }
            else if (!m_Input.C && !m_Input.Shift)
            {
                ResetToRunState();
            }
        }

        private void ChangeMoveState(float speedMultiplier, string trigger, string stateBool)
        {
            SetMoveSpeed(speedMultiplier);
            animator.SetTrigger(trigger);
            animator.SetBool(stateBool, true);
            animator.SetBool(IS_WALKING, stateBool == IS_WALKING);
            animator.SetBool(IS_RUNNING, stateBool == IS_RUNNING);
            animator.SetBool(IS_SPRINTING, stateBool == IS_SPRINTING);
        }

        private void ResetToRunState()
        {
            SetMoveSpeed(1f);

            if (animator.GetBool(IS_WALKING))
            {
                animator.SetBool(IS_WALKING, false);
                animator.SetTrigger(DO_RUN);
                animator.SetBool(IS_RUNNING, true);
            }

            if (animator.GetBool(IS_SPRINTING))
            {
                animator.SetBool(IS_SPRINTING, false);
                animator.SetTrigger(DO_RUN);
                animator.SetBool(IS_RUNNING, true);
            }
        }

        // Player 속도 변경
        private void SetMoveSpeed(float rate)
        {
            ps.inGameMoveSpeed = moveSpeed * rate;
        }

        #region Dash
        private void DoDash()
        {
            if (animator.GetBool(IS_DASH) == false
                && animator.GetBool("IsBlocking") == false
                && ps.isDashable)
            {
                StartDash(GetMousePosition());
            }
        }

        private void StartDash(Vector3 targetPoint)
        {
            animator.SetBool(IS_DASH, true);
            ps.isInvincible = true;

            Vector3 dashDirection = (targetPoint - transform.position).normalized;
            Vector3 dashTarget = transform.position + dashDirection * dashDistance;
            Vector3 rayOrigin = transform.position + Vector3.up;

            // 벽 감지
            if (Physics.Raycast(rayOrigin, dashDirection, out RaycastHit hit, dashDistance, LayerMask.GetMask("Wall", "Enemy")))
            {
                Debug.DrawRay(rayOrigin, dashDirection * hit.distance, Color.red);
                dashTarget = transform.position + dashDirection * (hit.distance - stopDistance);
            }
            else
            {
                Debug.DrawRay(rayOrigin, dashDirection * dashDistance, Color.green);
            }

            transform.DOMove(dashTarget, dashDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(EndDash)
            .SetLink(gameObject);

            Invoke(nameof(DisableInvincibility), invincibilityDuration);
        }

        private void EndDash()
        {
            animator.SetBool(IS_DASH, false);
        }

        private void DisableInvincibility()
        {
            ps.isInvincible = false;
        }
        #endregion
    }
}
