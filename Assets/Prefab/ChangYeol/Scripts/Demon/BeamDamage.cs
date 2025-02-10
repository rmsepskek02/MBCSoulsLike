using BS.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BS.Demon
{
    public class BeamDamage : MonoBehaviour
    {
        #region Variables
        private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();
        public Collider beamCollider;
        public int damageAmount = 10;
        #endregion
        private void OnTriggerEnter(Collider other)
        {
            // 자식 객체에서 PlayerHealth 컴포넌트를 찾음
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && !damagedObjects.Contains(other.gameObject))
            {
                Debug.Log($"{damageAmount}만큼 데미지 입음");
                playerHealth.TakeDamage(damageAmount, false);
                damagedObjects.Add(other.gameObject);
                StartCoroutine(ResetCollision(other.gameObject));
            }
        }
        IEnumerator ResetCollision(GameObject other)
        {
            yield return new WaitForSeconds(0.5f);
            damagedObjects.Remove(other);
        }
    }
}