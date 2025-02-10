using BS.Player;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 자식 객체에서 PlayerInvincibility 컴포넌트 찾기
            PlayerInvincibility playerInvincibility = other.GetComponentInChildren<PlayerInvincibility>();
            if (playerInvincibility != null)
            {
                // 플레이어 무적 만들기
                playerInvincibility.IsInvincible = true;
                Debug.Log("플레이어 무적");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 자식 객체에서 PlayerInvincibility 컴포넌트 찾기
            PlayerInvincibility playerInvincibility = other.GetComponentInChildren<PlayerInvincibility>();
            if (playerInvincibility != null)
            {
                // 플레이어 무적 해제
                playerInvincibility.IsInvincible = false;
                Debug.Log("플레이어 무적 해제");
            }
        }
    }
}
