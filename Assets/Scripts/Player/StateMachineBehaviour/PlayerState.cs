using UnityEngine;

namespace BS.Player
{
    /// <summary>
    /// Player의 상태
    /// </summary>
    public class PlayerState : MonoBehaviour
    {
        #region Variables
        // Idle
        //public bool isIdle { get; set; }                        // Idle 여부
        //
        //// Move
        //public Vector3 targetPosition;                          // 이동 목표 지점
        //public bool isMovable { get; set; } = true;             // 이동 가능 여부
        //public bool isMoving { get; set; }                      // 이동 진행 여부
        //public float inGameMoveSpeed;                           // BD 이동 속도
        //
        //// Sprint
        //public bool isSprintable { get; set; } = true;          // 스프린트 가능 여부
        //public bool isSprinting { get; set; }                   // 스프린트 진행 여부
        //// Walk
        //public bool isWalkable { get; set; } = true;            // 걷기 가능 여부
        //public bool isWalking { get; set; }                     // 걷는 진행 여부
        //
        //// Dash
        //public bool isDashable { get; set; } = true;            // 대쉬 가능 여부
        //public bool isDashing { get; set; }                     // 대쉬 진행 여부
        //public bool isInvincible { get; set; }                  // 무적 여부

        //// Block
        //public bool isBlockable { get; set; } = true;           // 블락 가능 여부
        //public bool isBlocking { get; set; }                    // 블락 진행 여부
        //public bool isBlockingAnim { get; set; }                // 블락 애니메이션 진행 여부
        //public float currentBlockCoolTime = 0f;                 // BD 블락 쿨타임
        //
        //// Attack
        //public bool isAttackable { get; set; } = true;          // 공격 가능 여부
        //public bool isAttacking { get; set; }                   // 공격 진행 여부
        //public int ComboAttackIndex = 1;                        // ComboAttack 인덱스 
        //
        //// Uppercut
        //public bool isUppercutable { get; set; } = true;        // 어퍼컷 가능 여부
        //public bool isUppercuting { get; set; }                 // 어퍼컷 진행 여부
        //public float currentUppercutCoolTime = 0f;              // BD 어퍼컷 쿨타임
        //
        //// ChargingPunch
        //public bool isChargingPunchable { get; set; } = true;   // 차징 펀지 가능 여부
        //public bool isChargingPunching { get; set; }            // 차징 펀지 진행 여부
        //public float currentChargingPunchCoolTime = 0f;         // BD 차징펀치 쿨타임
        //
        //// BackHandSwing
        //public bool isBackHandSwingable { get; set; } = true;   // 백핸드스윙 가능 여부
        //public bool isBackHandSwinging { get; set; }            // 백핸드스윙 진행 여부
        //public float currentBackHandSwingCoolTime = 0f;         // BD 백스윙 쿨타임


        public bool isIdle { get; set; }                        // Idle 여부

        // Move
        public Vector3 targetPosition;                          // 이동 목표 지점
        public bool isMovable = true;             // 이동 가능 여부
        public bool isMoving = false;                   // 이동 진행 여부
        public float inGameMoveSpeed;                           // BD 이동 속도

        // Sprint
        public bool isSprintable = true;         // 스프린트 가능 여부
        public bool isSprinting = false;                  // 스프린트 진행 여부
        // Walk
        public bool isWalkable = true;           // 걷기 가능 여부
        public bool isWalking = false;                     // 걷는 진행 여부

        // Dash
        public bool isDashable = true;           // 대쉬 가능 여부
        public bool isDashing = false;                   // 대쉬 진행 여부
        public bool isInvincible = false;                // 무적 여부
        
        // Block
        public bool isBlockable = true;          // 블락 가능 여부
        public bool isBlocking = false;                    // 블락 진행 여부
        public float currentBlockCoolTime = 0f;                 // BD 블락 쿨타임

        // Attack
        public bool isAttackable = true;          // 공격 가능 여부
        public bool isAttacking = false;                  // 공격 진행 여부
        public int comboAttackIndex = 1;                        // ComboAttack 인덱스 

        // Uppercut
        public bool isUppercutable = true;       // 어퍼컷 가능 여부
        public bool isUppercuting = false;               // 어퍼컷 진행 여부
        public float currentUppercutCoolTime = 0f;              // BD 어퍼컷 쿨타임

        // ChargingPunch
        public bool isChargingPunchable = true;   // 차징 펀지 가능 여부
        public bool isChargingPunching = false;          // 차징 펀지 진행 여부
        public float currentChargingPunchCoolTime = 0f;         // BD 차징펀치 쿨타임

        // BackHandSwing
        public bool isBackHandSwingable = true;   // 백핸드스윙 가능 여부
        public bool isBackHandSwinging = false;// 백핸드스윙 진행 여부
        public float currentBackHandSwingCoolTime = 0f;         // BD 백스윙 쿨타임
        public string currentSkillName;
        public Transform prevTransform;

        // Skill 사용 가능 여부
        public bool canSkill = true;
        // 타격 여부
        public bool isHit = false;
        #endregion
        void Awake()
        {

        }
        void Start()
        {

        }

        void Update()
        {

        }
    }
}
// TODO :: 모든 player 상태를 가지고있도록
// PlayerHealth는 PlayerState의 hp를 바꾸는 함수를 정의
// PlayerController 자식오브젝트 (랜더링을 담당하는 오브젝트)에 할당할 것
