using BS.PlayerInput;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BS.Player
{
    public class PlayerSkills : MonoBehaviour
    {
        // State
        PlayerState ps;
        private Vector2 m_inputVector2;                     // Input Vector2
        private PlayerInputActions m_Input;                 // PlayerInputActions

        // 어퍼컷
        public float uppercutCoolTime = 3f;                         // SD 어퍼컷 쿨타임
        public float uppercutDamage = 50f;

        // 백스윙
        public float backHandSwingCoolTime = 1f;                    // SD 백스윙 쿨타임
        public float backHandSwingDamage = 30f;

        // 차징펀치
        public float chargingPunchCoolTime = 3f;                    // SD 차징펀치 쿨타임
        public float chargingPunchDamage = 80f;

        //public float backHandSwingDistance = 5f;                  // 백스윙 거리

        public Camera mainCamera;                                   // Camera 변수
        public float rotationDuration = 0.1f;                       // 회전 지속 시간
        public TextMeshProUGUI uppercutCoolTimeText;
        public TextMeshProUGUI backHandSwingCoolTimeText;
        public TextMeshProUGUI chargingPunchCoolTimeText;
        PlayerSkillController psk;
        Animator animator;
        public float stopDistance = 1f; // 벽과 최소 거리 유지
        //public bool isHit = false;
        public Vector3 hitPos;
        Vector3 rayOrigin;
        //private Vector3 predictedVelocity;
        void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            psk = FindFirstObjectByType<PlayerSkillController>();
            m_Input = transform.parent.GetComponent<PlayerInputActions>();
            ps = FindFirstObjectByType<PlayerState>();
        }
        private void OnEnable()
        {
            // 스킬 구조체에 맞게 스킬을 추가
            if (!psk.skillList.ContainsKey("Q"))
            {
                psk.skillList.Add("Q", new Skill("Uppercut", uppercutCoolTime, DoUppercut, uppercutDamage));
            }
            if (!psk.skillList.ContainsKey("W"))
            {
                psk.skillList.Add("W", new Skill("BackHandSwing", backHandSwingCoolTime, DoBackHandSwing, backHandSwingDamage));
            }
            if (!psk.skillList.ContainsKey("E"))
            {
                psk.skillList.Add("E", new Skill("ChargingPunch", chargingPunchCoolTime, DoChargingPunch, chargingPunchDamage));
            }
        }
        private void Start()
        {
            animator = GetComponent<Animator>();
            rayOrigin = transform.position + Vector3.up;
        }

        // 애니메이션 실행중 호출 Update
        void OnAnimatorMove()
        {
            // Root Motion 데이터를 PlayerController에 반영
            if (animator)
            //&& characterTransform)
            {
                //if (ps.isHit)
                //{
                //    // 루트 모션을 무시하고 충돌 지점에서 고정
                //    animator.applyRootMotion = false; // 루트 모션 비활성화
                //    Vector3 temp = (hitPos - ps.prevTransform.position).normalized * 1.2f;
                //    transform.parent.position = hitPos - temp;
                //}
                //else
                //{
                    // 일반 루트 모션 업데이트
                    //animator.applyRootMotion = true; // 루트 모션 활성화
                    //predictedVelocity = animator.deltaPosition / Time.deltaTime;
                    transform.parent.transform.position = animator.rootPosition; // 캐릭터의 Root Motion 위치
                //}
            }
        }
        private void Update()
        {
            if (animator.GetBool("IsChargingPunch"))
            {
                rayOrigin = transform.position + Vector3.up;
                Vector3 moveDirection = (ps.targetPosition - transform.position).normalized;
                RaycastHit[] hits = Physics.RaycastAll(rayOrigin, moveDirection, 5f);

                foreach (RaycastHit hit in hits)
                {
                    // 충돌한 오브젝트의 레이어 가져오기
                    int hitLayer = hit.collider.gameObject.layer;

                    // Wall 또는 Enemy 레이어가 아닌 경우 continue
                    if ((hitLayer != LayerMask.NameToLayer("Wall")) && (hitLayer != LayerMask.NameToLayer("Enemy")))
                    {
                        continue;
                    }
                    // 충돌한 물체 처리
                    if (hit.distance <= stopDistance)
                    {
                        animator.applyRootMotion = false; // 루트 모션 비활성화
                        Vector3 hitPoint = new Vector3(hit.point.x, 0f, hit.point.z);
                        Vector3 temp = (hitPoint - ps.prevTransform.position).normalized * 1.2f;
                        temp.y = 0f;
                        transform.parent.position = hitPoint - temp;
                        return;
                    }
                }
            }
            if(!animator.applyRootMotion) animator.applyRootMotion = true; // 루트 모션 활성화
        }
        Vector3 GetMousePosition()
        {
            m_inputVector2 = m_Input.MousePosition;

            Ray ray = mainCamera.ScreenPointToRay(m_inputVector2);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.CompareTag("Ground"))
                {
                    return hit.point;
                }
            }
            return Vector3.zero;
        }

        public void DoUppercut()
        {
            if (animator.GetBool("IsAttacking")) return;
            if (animator.GetBool("IsDashing") == false
                && animator.GetBool("IsBlocking") == false
                && animator.GetBool("IsChargingPunch") == false
                && animator.GetBool("IsBackHandSwing") == false)
            {
                ps.targetPosition = GetMousePosition();
                RotatePlayer();
                animator.SetTrigger("DoUppercut");
                animator.SetBool("IsUppercuting", true);
                ps.canSkill = false;
                animator.SetBool("IsMoving", false);
                ps.isMovable = false;
            }
        }

        public void DoChargingPunch()
        {
            if (animator.GetBool("IsAttacking")) return;
            if (animator.GetBool("IsDashing") == false
                && animator.GetBool("IsBlocking") == false
                && animator.GetBool("IsUppercuting") == false
                && animator.GetBool("IsBackHandSwing") == false)
            {
                ps.targetPosition = GetMousePosition();
                RotatePlayer();
                animator.SetTrigger("DoChargingPunch");
                animator.SetBool("IsChargingPunch", true);
                ps.canSkill = false;
                animator.SetBool("IsMoving", false);
                ps.isMovable = false;
            }
        }

        public void DoBackHandSwing()
        {
            if (animator.GetBool("IsAttacking")) return;
            if (animator.GetBool("IsDashing") == false
                && animator.GetBool("IsBlocking") == false
                && animator.GetBool("IsUppercuting") == false
                && animator.GetBool("IsChargingPunch") == false)
            {
                ps.targetPosition = GetMousePosition();
                RotatePlayer();
                animator.SetTrigger("DoBackHandSwing");
                animator.SetBool("IsBackHandSwing", true);
                ps.canSkill = false;
                animator.SetBool("IsMoving", false);
                ps.isMovable = false;
            }
        }

        // DoTween 회전 처리
        void RotatePlayer()
        {
            transform.parent.transform.DOKill(complete: false); // 트랜스폼과 관련된 모든 트윈 제거 (완료 콜백은 실행되지 않음)

            if (ps.isUppercuting || ps.isBackHandSwinging || ps.isChargingPunching) return;

            // 목표 회전값 계산
            Vector3 direction = (ps.targetPosition - transform.parent.transform.position).normalized;
            direction = new Vector3(direction.x, 0, direction.z);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.parent.transform.DORotateQuaternion(targetRotation, rotationDuration)
                        .SetAutoKill(true)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(() => { });
        }

        // 다른행동이 불가능 하도록 설정
        public void CannotOtherAct()
        {
            ps.isDashable = false;
            ps.isBlockable = false;
        }
        // 다른행동이 가능 하도록 설정
        public void CanOtherAct()
        {
            ps.isDashable = true;
            ps.isBlockable = true;
        }
    }
}
