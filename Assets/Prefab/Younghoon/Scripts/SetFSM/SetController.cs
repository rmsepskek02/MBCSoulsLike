using BS.Player;
using UnityEngine;
using UnityEngine.AI;

namespace BS.Enemy.Set
{
    public class SetController : MonoBehaviour
    {
        private ISetState currentState;
        private SetProperty property;
        private Transform player;

        //세트 체력 참조 변수
        private SetHealth setHealth;
        //죽음체크
        private bool isDead = false;

        private AttackTrigger closedAttack;

        [SerializeField] private float rotationSpeed = 2f;
        [SerializeField] private float attackCooldown = 3f;

        public float AttackCooldown => attackCooldown; // Read-only Property

        public AudioClip bossDieSFX;

        private Animator animator;

        private void Start()
        {
            Initialize();
            SetInitialState();
        }

        private void Update()
        {
            if (isDead) return; // 사망 후에는 상태 변경 방지

            RotateToPlayerIfNotAttacking();
            currentState?.Update();
        }

        private void Initialize()
        {
            // PlayerController를 찾아서 player에 할당
            PlayerController playerController = FindAnyObjectByType<PlayerController>();

            if (playerController != null)
            {
                player = playerController.transform; // PlayerController에서 Transform을 할당
            }
            else
            {
                Debug.LogError("PlayerController를 찾을 수 없습니다.");
            }

            var agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            setHealth = GetComponent<SetHealth>();
            closedAttack = GetComponentInChildren<AttackTrigger>(true);
            setHealth.OnDie += HandleDeath;
            closedAttack.OnBlocked += FaintingState;
            property = new SetProperty(this, animator, agent, player);
        }

        private void SetInitialState()
        {
            SetState(new SetChaseState(property));
        }

        public void SetState(ISetState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }

        private void RotateToPlayerIfNotAttacking()
        {
            if (animator.GetBool(SetProperty.SET_ANIM_BOOL_ATTACK) || animator.GetBool(SetProperty.SET_ANIM_BOOL_FAINTING))
                return;

            RotateToPlayer();
        }

        private void RotateToPlayer()
        {
            Vector3 directionToPlayer = (property.Player.position - transform.position).normalized;

            if (directionToPlayer == Vector3.zero) return;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }

        private void HandleDeath()
        {
            if (isDead) return;

            isDead = true;
            SetState(new SetDeathState(property));
        }

        public void FaintingState()
        {
            SetState(new SetFaintingState(property));
        }
    }
}
