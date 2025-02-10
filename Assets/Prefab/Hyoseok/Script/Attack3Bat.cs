using BS.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BS.vampire
{
    public class Attack3Bat : MonoBehaviour
    {
        #region Variables

        public GameObject effectPrefab;
        private Rigidbody rb;
        private Vector3 velocity;
        private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();
        public int damageAmount = 10;
        #endregion

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Initialize(Vector3 direction, float speed)
        {
            velocity = direction * speed;
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = velocity;
        }

        void OnTriggerEnter(Collider other)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Debug.Log("�÷��̾� �߰�!");

                // �ڽ� ��ü���� PlayerHealth ������Ʈ�� ã��
                PlayerHealth playerHealth = other.GetComponentInChildren<PlayerHealth>();
                if (playerHealth != null && !damagedObjects.Contains(other.gameObject))
                {
                    Debug.Log($"{damageAmount}��ŭ ������ ����");
                    playerHealth.TakeDamage(damageAmount, false);
                    damagedObjects.Add(other.gameObject);
                    StartCoroutine(ResetCollision(other.gameObject));
                }
            }
        }

        // ���� �ð� �� �浹 ���� ����
        IEnumerator ResetCollision(GameObject other)
        {
            yield return new WaitForSeconds(0.5f);
            damagedObjects.Remove(other);
        }
    }
}
