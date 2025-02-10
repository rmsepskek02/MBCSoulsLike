using BS.Audio;
using BS.Demon;
using BS.Player;
using BS.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace BS.Particle
{
    public class DotParticles : MonoBehaviour
    {
        #region Variables
        private new ParticleSystem particleSystem;
        private HashSet<GameObject> collidedObjects = new HashSet<GameObject>();
        // 패턴별 데미지
        public int maxDamage = 1;
        public int minDamage = 2;

        [SerializeField]private DemonAudioManager audioManager;
        #endregion
        void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        void OnParticleCollision(GameObject other)
        {
            if (DemonController.isDie) return;
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // 자식 객체에서 PlayerHealth 컴포넌트 찾기
                PlayerHealth playerHealth = other.GetComponentInChildren<PlayerHealth>();
                if (playerHealth != null)
                {
                    AudioUtility.CreateSFX(audioManager.SetAudioClip(6), playerHealth.transform.position, audioManager.SetGroups(6));
                    int damage = Random.Range(minDamage, maxDamage);
                    Debug.Log($"{damage}만큼 데미지 입음");
                    playerHealth.TakeDamage(damage, false);
                    Debug.Log($"hp={playerHealth.CurrentHealth}");
                }
            }
        }
    }
}