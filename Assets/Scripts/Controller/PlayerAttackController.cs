using UnityEngine;

namespace BS.Player
{
    /// <summary>
    /// Player를 공격 컨트롤
    /// </summary>
    public class PlayerAttackController : PlayerController
    {
        #region Variables

        public float comboableTime = 0f;                    // 연계 공격 가능 시간
        public float _comboableTime = 1f;                   // SD 연계 공격 가능 시간
        public double lastAttackTime = 0;                   // 마지막 공격 시간

        private bool isMousePressed = false;
        private float timeSinceLastClick = 0f;
        private float clickInterval = 0.12f;                // 클릭 반복 간격
        #endregion

        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
            comboableTime = _comboableTime;
        }

        protected override void Update()
        {
            SetTargetPosition();
        }
        
        protected override void FixedUpdate()
        {
            // 마우스 좌클릭 상태 확인
            if (m_Input.LeftClick)
            {
                if (!isMousePressed)
                {
                    // 좌클릭이 처음 눌린 순간
                    ComboAttack();
                    isMousePressed = true;
                    timeSinceLastClick = 0f; // 초기화
                }
                else
                {
                    // 계속 눌려있을 때, 일정 시간 간격마다 함수 호출
                    timeSinceLastClick += Time.deltaTime;
                    if (timeSinceLastClick >= clickInterval)
                    {
                        ComboAttack();
                        timeSinceLastClick = 0f; // 시간 초기화
                    }
                }
            }
            else
            {
                // 마우스 버튼을 떼었을 때 초기화
                isMousePressed = false;
            }
        }

        // Target Position 설정
        private void SetTargetPosition()
        {
            if (m_Input.LeftClick && ps.isAttackable)
            {
                RotatePlayer();
                if (animator.GetBool("IsAttacking") == false)
                {
                    animator.SetBool("IsAttacking", true);
                }
            }
        }

        void ComboAttack()
        {
            if (animator.GetBool("IsAttacking") == true)
            {
                animator.SetBool("IsMoving", false);
                //ps.isMoving = false;
                ps.isMovable = false;

                // 공격을 한지 {comboableTime}초이상 지났을 경우 다시 combo1로 들어가게끔 설정
                if ((Time.time - lastAttackTime) > comboableTime && ps.comboAttackIndex != 1)
                {
                    ps.comboAttackIndex = 1;
                }

                // 공격 Trigger 발동
                animator.SetInteger("ComboAttack", ps.comboAttackIndex);
                animator.SetTrigger("DoAttack");
                lastAttackTime = Time.time;

                // combo4까지 모두 끝난 경우
                if (ps.comboAttackIndex > 4)
                    ps.comboAttackIndex = 1;
            }
        }
    }
}