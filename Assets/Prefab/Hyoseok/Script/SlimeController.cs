using BS.Player;
using System.Collections;
using UnityEngine;

namespace BS.Title
{
    public class SlimeController : MonoBehaviour
    {
        #region Variables
        [HideInInspector]public GameObject player;
        public GameObject attackParticlePrefab; // 파티클 프리팹
        public Transform attackParticleSpawnPoint; // 파티클 생성 위치
        public int damageAmount = 1;
        private Animator animator;
        private bool isAttacking = false; // 공격 중인지 여부를 확인하는 플래그
        #endregion

        private void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            StartCoroutine(AttackLoop()); // 공격 루프 시작
        }

        void OnTriggerEnter(Collider other)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if(playerController != null )
            {
                PlayerHealth playerHealth = playerController.GetComponentInChildren<PlayerHealth>();
                if (playerHealth != null)
                {
                    Debug.Log($"{damageAmount}만큼 데미지 입음");
                    playerHealth.TakeDamage(damageAmount, false);
                    Debug.Log($"hp={playerHealth.CurrentHealth}");
                }
            }
        }

        IEnumerator AttackLoop()
        {
            while (true)
            {
                if (!isAttacking)
                {
                    yield return StartCoroutine(Attack());
                }
                yield return new WaitForSeconds(1f); // 1초마다 공격
            }
        }

        IEnumerator Attack()
        {
            isAttacking = true; // 공격 시작
            animator.SetBool("attack", true);

            yield return new WaitForSeconds(0.7f);
            GameObject attackParticle = Instantiate(attackParticlePrefab, attackParticleSpawnPoint.position, attackParticleSpawnPoint.rotation);
            yield return new WaitForSeconds(1f);

            Destroy(attackParticle,1f); // 파티클 제거
            animator.SetBool("attack", false);
            isAttacking = false; // 공격 종료
        }
    }
}
