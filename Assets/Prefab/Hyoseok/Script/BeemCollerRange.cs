using BS.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BS.vampire
{
    public class BeemCollerRange : MonoBehaviour
    {
        #region Variables
        private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();
        public int damageAmount = 10;
        #endregion
        void OnTriggerEnter(Collider other)
        {
            //Debug.LogWarning(other);
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                PlayerHealth playerHealth = playerController.GetComponentInChildren<PlayerHealth>();
                if (playerHealth != null && !damagedObjects.Contains(other.gameObject))
                {
                    Debug.Log($"{damageAmount}만큼 데미지 입음");
                    playerHealth.TakeDamage(damageAmount, false);
                    Debug.Log($"hp={playerHealth.CurrentHealth}");
                    damagedObjects.Add(other.gameObject);
                    StartCoroutine(ResetCollision(other.gameObject));
                }
            }
        }

        // 일정 시간 후 충돌 정보 리셋
        IEnumerator ResetCollision(GameObject other)
        {
            yield return new WaitForSeconds(0.5f);
            damagedObjects.Remove(other);
        }
    }

}