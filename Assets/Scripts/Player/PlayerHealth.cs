using BS.PlayerInput;
using BS.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static BS.Utility.AudioUtility;

namespace BS.Player
{
    public class PlayerHealth : PlayerController
    {
        #region Variables
        // 애니메이션 파라미터 이름 상수
        private static readonly string IS_BLOCKING = "IsBlocking";
        private static readonly string IS_ATTACKING = "IsAttacking";
        private static readonly string DO_BLOCK = "DoBlock";

        public AudioClip blockSound = null;
        // 블록 관련 변수
        public float blockCoolTime = 3f; // 블록 쿨타임 (기본값: 3초)
        public TextMeshProUGUI blockCoolTimeText; // 블록 쿨타임 텍스트 표시

        // 최대 체력
        [SerializeField] private float maxHealth;
        public float MaxHealth
        {
            get { return maxHealth; }
            private set { maxHealth = value; }
        }

        // 현재 체력
        [SerializeField] private float currentHealth;
        public float CurrentHealth
        {
            get { return currentHealth; }
            private set
            {
                currentHealth = value;

                // 체력이 0 이하가 되면 죽음 처리
                if (currentHealth <= 0)
                {
                    IsDeath = true;
                }
            }
        }

        public float GetRatio() => CurrentHealth / MaxHealth;

        // 죽음 여부
        private bool isDeath = false;
        public bool IsDeath
        {
            get { return isDeath; }
            private set
            {
                isDeath = value;
                if (isDeath == true)
                {
                    // 애니메이션 설정
                    animator.SetTrigger("Death");
                    m_Input.UnInputActions();
                }
            }
        }
        private float lastRatio = 1f;
        private float totalDamagePersentage = 100f;
        public float TotalDamagePersentage => totalDamagePersentage;

        // 이벤트 액션
        public UnityAction<float> OnDamaged;      // 데미지를 받을 때 호출되는 이벤트
        public UnityAction<float> OnHealed;      // 데미지를 받을 때 호출되는 이벤트
        public UnityAction OnBlocked;            // 블록 성공 시 호출되는 이벤트
        public UnityAction OnDie;            // 블록 성공 시 호출되는 이벤트

        PlayerSkillController psk;
        #endregion

        protected override void Awake()
        {
            m_Input = transform.parent.GetComponent<PlayerInputActions>();
            psk = FindFirstObjectByType<PlayerSkillController>();
        }

        private void OnEnable()
        {
            // 스킬 목록에 블록 스킬 추가
            if (!psk.skillList.ContainsKey("R"))
            {
                psk.skillList.Add("R", new Skill("Block", blockCoolTime, DoBlock));
            }
            OnDamaged += CalculateDamage; // 데미지 이벤트 구독
            OnDamaged += PlayHitAnim;
            OnBlocked += PlayBlockSound;
        }

        private void OnDisable()
        {
            OnDamaged -= CalculateDamage; // 데미지 이벤트 구독 해제
            OnDamaged -= PlayHitAnim;
            OnBlocked -= PlayBlockSound;
        }

        protected override void Start()
        {
            base.Start();
            maxHealth = 1000f; // 초기 최대 체력 설정
            currentHealth = MaxHealth; // 현재 체력을 최대 체력으로 초기화
        }

        public void PlayBlockSound()
        {
            AudioUtility.CreateSFX(blockSound, transform.position, AudioGroups.Skill);
        }

        // 블록 수행 메서드
        public void DoBlock()
        {
            if (!ps.isBlockable) return;

            if (!animator.GetBool(IS_BLOCKING))
            {
                ps.isDashable = false; // 대시 불가 설정
                RotatePlayer(); // 플레이어 회전
                ps.targetPosition = transform.position; // 블록 중 이동 방지
                animator.SetBool(IS_BLOCKING, true);
                animator.SetTrigger(DO_BLOCK);
            }
        }

        // 블록 시작 처리
        public void OnBlock()
        {
            ps.isBlocking = true;
        }

        // 블록 종료 처리
        public void UnBlock()
        {
            ps.isBlocking = false;
        }

        // 플레이어 회전 처리
        protected override void RotatePlayer()
        {
            if (!ps.isUppercuting && !ps.isBackHandSwinging && !ps.isChargingPunching && !animator.GetBool(IS_ATTACKING))
            {
                transform.parent.DOKill(complete: false); // 트랜스폼 관련 모든 트윈 제거 (완료 콜백 실행 안 함)

                // 목표 회전값 계산
                Vector3 direction = (GetMousePosition() - transform.parent.position).normalized;
                direction = new Vector3(direction.x, 0, direction.z);
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                transform.parent.DORotateQuaternion(targetRotation, rotationDuration).SetLink(gameObject); // 회전 애니메이션 실행
            }
        }

        // 최대 체력 설정
        public void SetMaxHealth(float amount)
        {
            maxHealth = amount;
            CurrentHealth = maxHealth;
        }

        // 데미지 처리 (데미지 값, 블록 가능 여부)
        public bool TakeDamage(float damage, bool isBlockable = true)
        {
            if (isDeath)
                return false;

            if (isBlockable)
            {
                if (animator.GetBool("IsBlocking") == true) // 블록 성공
                {
                    OnBlocked?.Invoke();
                    return true;
                }
                else // 블록 실패
                {
                    OnDamaged?.Invoke(damage);
                    return false;
                }
            }
            else // 블록 불가 상황
            {
                OnDamaged?.Invoke(damage);
                return false;
            }
        }

        public void TakeHeal()
        {
            CurrentHealth = maxHealth;
            lastRatio = GetRatio();
            OnHealed?.Invoke(maxHealth);
        }

        public void PlayHitAnim(float damage)
        {
            animator.SetBool("IsHit", true);
        }

        // 데미지 계산 메서드
        public void CalculateDamage(float damage)
        {
            float realDamage = Mathf.Min(CurrentHealth, damage); // 실제 데미지 계산

            CurrentHealth -= realDamage; // 체력 감소

            // 데미지 비율 변화량을 계산
            float damagePercentageChange;
            if (GetRatio() <= 0)
            {
                damagePercentageChange = totalDamagePersentage;
            }
            else
            {
                damagePercentageChange = (lastRatio - GetRatio()) * 100f;
            }

            // totalDamagePersentage에 누적시키되, 0 이하로 내려가지 않도록 함
            totalDamagePersentage = Mathf.Max(totalDamagePersentage - damagePercentageChange, 0f);
            // lastRatio 업데이트 (현재 비율로 갱신)
            lastRatio = GetRatio();
            if (CurrentHealth <= 0f) // 체력이 0 이하일 경우
            {
                CurrentHealth = 0;
                OnDie?.Invoke();
                //Die(); // 사망 처리 호출 가능
            }

            Debug.Log($"Player OnDamaged = {damage}");
            Debug.Log($"Player Hp = {CurrentHealth}");
        }
    }
}
