using UnityEngine;
using System;
using System.Collections;
using BS.UI;
using BS.Achievement;

public class AlienHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 500; // 보스 최대 체력
    public float currentHealth;
    private Animator animator;
    private bool isInvincible = false; //무적여부
    //public GameObject EndingSequencerCamera; //엔딩연출카메라
    //public GameObject fadeInOut;
    private DungeonClearTime dungeonClearTime;
    public bool isDie = false;

    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }


    // 체력 변경 이벤트 (UI와 연동)
    public event Action<float> OnHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        NotifyHealthChanged();
        dungeonClearTime = FindAnyObjectByType<DungeonClearTime>();

        //fadeInOut.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            currentHealth -= 50.0f;
            if (currentHealth <= 0)
            {
                isDie = true;
                animator.SetInteger("Pattern", 6);
                AchievementManager.Instance.UpdateAchievement(AchievementType.KillCount, 1);
                dungeonClearTime.StopTimer();

            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            isDie = true;
            currentHealth = 0.0f;
            AchievementManager.Instance.UpdateAchievement(AchievementType.KillCount, 1);
            animator.SetInteger("Pattern", 6);
            dungeonClearTime.StopTimer();
        }
    }

    // 대미지 처리
    public void TakeDamage(float damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 체력을 0 이상, maxHealth 이하로 유지
            Debug.Log($"Boss took {damage} damage. Current health: {currentHealth}");

            NotifyHealthChanged();

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    // 체력 변경 알림
    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }

    // 보스 사망 처리
    private void Die()
    {
        isDie = true;
        //Debug.Log("Boss defeated!");
        // EndingSequencerCamera 켜기
        //EndingSequencerCamera.SetActive(false);
        AchievementManager.Instance.UpdateAchievement(AchievementType.KillCount, 1);
        animator.SetInteger("Pattern", 6);
        //페이드인아웃 활성화
        //fadeInOut.SetActive(true);

        //2초뒤 페이드인아웃 비활성화
        //yield return new WaitForSeconds(2.0f);
        dungeonClearTime.StopTimer();

        //페이드인아웃 비활성화
        //fadeInOut.SetActive(false);
        // 보스 사망 로직 추가 (예: 애니메이션, 제거)
        Destroy(gameObject, 5f); // 보스 오브젝트 제거
    }
}
