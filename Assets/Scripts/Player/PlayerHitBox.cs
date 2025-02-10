using UnityEngine;
using System.Collections.Generic;
using BS.Utility;
using static BS.Utility.AudioUtility;
using BS.Managers;

namespace BS.Player
{
    public class PlayerHitBox : MonoBehaviour
    {
        [Header("Hitbox Settings")]
        public string enemyLayerName = "Enemy"; // 적의 레이어 이름
        public string wallLayerName = "Wall"; // 적의 레이어 이름
        public GameObject controller;
        private PlayerState ps;
        private PlayerSkills psk;
        private Animator animator;
        private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>(); // 이미 대미지를 준 적을 추적

        public AudioClip[] hitSounds;
        CameraManager cm;
        PlayerSkillController pskController;
        private void Start()
        {
            ps = controller.transform.GetChild(0).GetComponent<PlayerState>();
            psk = controller.transform.GetChild(0).GetComponent<PlayerSkills>();
            animator = controller.GetComponentInChildren<Animator>();
            cm = Camera.main.GetComponent<CameraManager>();
            pskController = FindFirstObjectByType<PlayerSkillController>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // 적 레이어인지 확인
            if (collision.gameObject.layer == LayerMask.NameToLayer(enemyLayerName)
                //&& psm.animator.GetBool("IsAttacking")
                && ps.isChargingPunching
                )
            {
                var enemyHealth = collision.gameObject.GetComponent<IDamageable>();
                if (enemyHealth != null)
                {
                    // 이미 대미지를 준 적이라면 중복 대미지 방지
                    if (damagedEnemies.Contains(collision.gameObject))
                        return;

                    // 현재 스킬의 대미지 가져오기
                    float currentSkillDamage = GetCurrentSkillDamage();
                    if (currentSkillDamage > 0)
                    {
                        //Vector3 hitPoint = collision.contacts[0].point;
                        //hitPoint.y = 0;
                        //psk.hitPos = hitPoint;
                        //psk.isHit = true;
                        // 적에게 대미지 입힘
                        enemyHealth.TakeDamage((float)currentSkillDamage);

                        // 대미지를 준 적을 기록
                        damagedEnemies.Add(collision.gameObject);

                        // 쿨다운이 끝나거나 일정 시간이 지난 후 다시 공격 가능하도록 설정
                        Invoke(nameof(ResetDamagedEnemies), 0.5f); // 0.5초 후 초기화
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(animator != null)
            {
                if (animator.GetBool("IsAttacking") == false
                && animator.GetBool("IsBackHandSwing") == false
                && animator.GetBool("IsUppercuting") == false
                && animator.GetBool("IsChargingPunch") == false)
                {
                    return;
                }
            }
            // 적 레이어인지 확인
            if (other.gameObject.layer == LayerMask.NameToLayer(enemyLayerName))
            {
                var enemyHealth = other.gameObject.GetComponent<IDamageable>();
                if (enemyHealth != null)
                {
                    // 이미 대미지를 준 적이라면 중복 대미지 방지
                    if (damagedEnemies.Contains(other.gameObject))
                        return;

                    // 현재 스킬의 대미지 가져오기
                    float currentSkillDamage = GetCurrentSkillDamage();
                    if (currentSkillDamage > 0)
                    {
                        if (currentSkillDamage > 60)
                        {
                            cm.ShakeCamera(0.3f, 5f);
                        }
                        Vector3 hitPoint = other.ClosestPoint(transform.position);
                        // SFX
                        PlayHitSound(hitPoint);
                        // 적에게 대미지 입힘
                        enemyHealth.TakeDamage((int)currentSkillDamage);

                        // 대미지를 준 적을 기록
                        damagedEnemies.Add(other.gameObject);

                        // 쿨다운이 끝나거나 일정 시간이 지난 후 다시 공격 가능하도록 설정
                        Invoke(nameof(ResetDamagedEnemies), 0.5f); // 0.5초 후 초기화
                    }
                }
            }
        }

        private void PlayHitSound(Vector3 hitPoint)
        {
            AudioClip hitSound = null;
            if (ps.currentSkillName == "E")
            {
                hitSound = hitSounds[5];
            }
            else if (ps.currentSkillName == "W")
            {
                hitSound = hitSounds[4];
            }
            else if (ps.currentSkillName == "Q")
            {
                hitSound = hitSounds[6];
            }
            else
            {
                int randomNumber = Mathf.FloorToInt(Random.value * 4);
                hitSound = hitSounds[randomNumber];
            }

            AudioUtility.CreateSFX(hitSound, hitPoint, AudioGroups.Skill);
        }


        // 현재 실행 중인 스킬의 대미지를 반환
        private float GetCurrentSkillDamage()
        {
            // PlayerSkillController의 현재 스킬 확인
            if (pskController.skillList.TryGetValue(ps.currentSkillName, out var skill))
            {
                return skill.damage;
            }

            // 현재 스킬이 없다면 기본 대미지 반환
            return 10;
        }

        // 공격 대상을 초기화 (쿨다운 이후 재공격 가능)
        private void ResetDamagedEnemies()
        {
            damagedEnemies.Clear();
        }
    }
}
