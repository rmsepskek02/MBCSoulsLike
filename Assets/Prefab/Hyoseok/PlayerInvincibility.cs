using UnityEngine;

namespace BS.Player
{
    public class PlayerInvincibility : MonoBehaviour
    {
        private bool isInvincible = false;
        public bool IsInvincible
        {
            get { return isInvincible; }
            set { isInvincible = value; }
        }

        private PlayerHealth playerHealth;

        private void Awake()
        {
            playerHealth = GetComponent<PlayerHealth>();
        }

        // 데미지를 받을 때 무적 여부를 체크하는 메서드 추가
        public bool TakeDamage(float damage, bool isBlockable)
        {
            if (isInvincible)
            {
                Debug.Log("플레이어는 무적 상태입니다.");
                return false;
            }

            // 기존 PlayerHealth의 TakeDamage 메서드 호출
            return playerHealth.TakeDamage(damage, isBlockable);
        }
    }
}
