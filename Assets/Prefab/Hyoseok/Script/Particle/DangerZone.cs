using BS.Player;
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (!enabled) return;

        if (other.CompareTag("Player"))
        {
            // 플레이어에게 데미지 적용
            PlayerHealth playerHealth = other.GetComponentInChildren<PlayerHealth>();
            PlayerInvincibility playerInvincibility = other.GetComponentInChildren<PlayerInvincibility>();

            if (playerHealth != null && playerInvincibility != null)
            {
                if (!playerInvincibility.IsInvincible) 
                {
                    playerHealth.TakeDamage(99999f, false);
                    Debug.Log("플레이어 사망");
                }
                else
                {
                    Debug.Log("플레이어는 무적 상태입니다.");
                }
            }
        }
    }
}
