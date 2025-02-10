using BS.PlayerInput;
using DG.Tweening;
using UnityEngine;

namespace BS.Player
{
    /// <summary>
    /// Player Position, Rotation 조정, MousePosition 가져오기
    /// Player SO 가져오기, 공통 참조 & 함수 & 변수
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        // TODO :: ScriptableObject를 사용해보자 (movespeed)
        #region Variables
        // Debug
        private Vector3? gizmoPosition;

        // Reference
        protected Camera mainCamera;                          // 카메라 참조
        protected PlayerInputActions m_Input;                 // PlayerInputActions
        protected PlayerState ps;
        protected Animator animator;
        // Mouse
        protected Vector2 m_inputVector2;                     // Input Vector2
        [SerializeField] protected Vector3 targetPosition;    // 월드 좌표로 변환된 목표 지점
        
        // Move
        [SerializeField] protected float moveSpeed = 5f;      // 이동 속도, SD 이동 속도

        // Rotate, 보간 속도
        public float lerpSpeed = 10f;

        // Attack
        //protected bool isAttackalbe = false;                  // 공격 가능 여부
        //protected bool isAttacking = false;                   // 공격 여부

        public float rotationDuration = 0.3f;

        // UI
        //public TextMeshProUGUI dashCoolTimeText;
        #endregion

        protected virtual void Awake()
        {
            m_Input = GetComponent<PlayerInputActions>();
        }

        protected virtual void Start()
        {
            ps = GetComponentInChildren<PlayerState>();
            animator = m_Input.transform.GetChild(0).GetComponent<Animator>();
            if (mainCamera == null)
                mainCamera = Camera.main;

            targetPosition = transform.position;
            //ps.targetPosition = transform.position;
            ps.inGameMoveSpeed = moveSpeed;
        }

        protected virtual void Update()
        {
            //RotateToTargetPos();
            //RotatePlayer();
        }
        protected virtual void FixedUpdate()
        {
            //SetPlayerYPos();
        }

        // Mouse 위치를 화면 좌표에서 월드 좌표로 변환
        protected virtual Vector3 GetMousePosition()
        {
            m_inputVector2 = m_Input.MousePosition;

            Ray ray = mainCamera.ScreenPointToRay(m_inputVector2);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.CompareTag("Ground"))
                {
                    gizmoPosition = hit.point; // Gizmo 위치 저장
                    return hit.point;
                    //ps.targetPosition = hit.point;
                    //break;

                }
            }
            return Vector3.zero;
        }

        // Player 회전
        protected virtual void RotateToTargetPos()
        {
            if (animator.GetBool("IsAttacking")) return;
            if (targetPosition != null && ps.isMoving && animator.GetBool("IsBlocking") == false && !ps.isUppercuting && !ps.isBackHandSwinging && !ps.isChargingPunching)
            {
                // 목표 방향 계산
                Vector3 direction = GetMousePosition() - transform.position;

                // 방향이 존재하면 회전
                if (direction != Vector3.zero)
                {
                    // 목표 회전값 계산
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    // Slerp로 보간 속도 조절
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        lerpSpeed * Time.deltaTime
                    );
                }
            }
        }

        // DoTween 회전 처리
        protected virtual void RotatePlayer()
        {
            //if (psm.animator.GetBool("IsAttacking")) return;
            if (animator.GetBool("IsBlocking") == false
                && !ps.isUppercuting
                && !ps.isBackHandSwinging
                && !ps.isChargingPunching
                && animator.GetBool("IsAttacking") == false
                )
            {
                transform.DOKill(complete: false); // 트랜스폼과 관련된 모든 트윈 제거 (완료 콜백은 실행되지 않음)
                // 목표 회전값 계산
                Vector3 direction = (GetMousePosition() - transform.position).normalized;
                direction = new Vector3(direction.x, 0, direction.z);
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                transform.DORotateQuaternion(targetRotation, rotationDuration).SetLink(gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            if (gizmoPosition.HasValue)
            {
                Gizmos.color = Color.red; // Gizmo 색상 설정
                Gizmos.DrawSphere(gizmoPosition.Value, 0.2f); // 반지름 0.2의 구체 그리기
            }
        }

        private void SetPlayerYPos()
        {
            Vector3 position = transform.position;

            if (position.y < 0)
            {
                // y 좌표를 0으로 보간
                position.y = Mathf.Lerp(position.y, 0, Time.deltaTime * lerpSpeed);
                transform.position = position;
            }
        }
    }
}
// TODO :: SO를 생성하면서 PlayerState, PlayerHealth 등등 공통적으로 들어가는 것들을 추가시켜주자