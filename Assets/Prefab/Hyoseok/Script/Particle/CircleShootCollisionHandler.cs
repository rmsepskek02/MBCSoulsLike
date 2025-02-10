using System.Collections.Generic;
using UnityEngine;
using BS.Player;
using BS.Utility;

namespace BS.Particle
{
    public class CircleShootCollisionHandler : MonoBehaviour
    {
        private new ParticleSystem particleSystem;
        private HashSet<GameObject> collidedObjects = new HashSet<GameObject>();
        public AudioClip hitSound;
        // 패턴별 데미지
        public int damageAmount = 10;

        void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        void OnParticleCollision(GameObject other)
        {
            Debug.Log(other.name);

            // 자식 객체에서 PlayerHealth 컴포넌트 찾기
            PlayerHealth playerHealth = other.GetComponentInChildren<PlayerHealth>();
            if (playerHealth != null)
            {
                AudioUtility.CreateSFX(hitSound, transform.position, AudioUtility.AudioGroups.Sound);
                Debug.Log($"{damageAmount}만큼 데미지 입음");
                playerHealth.TakeDamage(damageAmount, false);
                Debug.Log($"hp={playerHealth.CurrentHealth}");

            }

        }



    }
}
