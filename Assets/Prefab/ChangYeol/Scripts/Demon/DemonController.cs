using BS.Achievement;
using BS.Player;
using BS.PlayerInput;
using BS.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BS.Demon
{
    public enum DEMON
    {
        Idle,
        Attack01,
        Attack02,
        Attack03,
        Teleport,
        GetDamaged,
        Die
    }
    public abstract class DemonController : MonoBehaviour,IDamageable
    {
        public abstract void NextPhase();
        #region Variables
        [HideInInspector]public DEMON currentState = DEMON.Idle; // 현재 상태
        [HideInInspector]public Animator animator; // 애니메이터

        public float attackRange = 3f; // 공격 범위
        public float[] attackCooldown = new float[3]; //쿨타임
        [HideInInspector]public float[] lastAttackTime = new float[4];
        private List<DEMON> demons = new List<DEMON>() { DEMON.Attack01, DEMON.Attack02 , DEMON.Teleport };
        protected List<DEMON> pesosDemon = new List<DEMON>() { DEMON.Attack01, DEMON.Attack02, DEMON.Teleport };

        protected int index;  //demons의 랜덤 값

        public float maxHealth = 100f; // 최대 체력
        public float currentHealth { get; private set; } // 현재 체력
        public bool hasRecovered = false; // 회복 실행 여부 플래그
        public static bool isDie = false;     //죽음 여부및 한 번만 죽이기
        public Image healthBarFill;       // 체력바의 Foreground Image
        public TextMeshProUGUI healthText; // 체력 퍼센트 Text (TextMeshPro 사용)

        protected DemonPattern pattern;
        public DungeonClearTime clearTime;
        private bool istimer = false;
        public SceneManager sceneManager;
        public DemonGameManager gameManager;
        public AudioSource source;
        #endregion
        private void Start()
        {
            //참조
            animator = GetComponent<Animator>();
            pattern = GetComponent<DemonPattern>();
            currentHealth = maxHealth; // 초기 체력 설정
            isDie = false;
            istimer = false;
            hasRecovered = false;
            currentState = DEMON.Idle;
        }

        private void Update()
        {
            UpdateHealthBar();
            if (isDie || gameManager.gameEnded) return;
            if (currentHealth <= maxHealth * 0.5f && !hasRecovered)
            {
                RecoverHealth();
                istimer = true;
            }
            if (hasRecovered)
            {
                if(istimer)
                {
                    StartCoroutine(DemonCurrentState(true, 2f));
                    istimer=false;
                    return;
                }
                else
                {
                    StartCoroutine(DemonCurrentState(false));
                }
                return;
            }
            StartCoroutine(DemonCurrentState(false));
        }
        public IEnumerator DemonCurrentState(bool istimeing, float time = 0)
        {
            if (istimeing)
            {
                yield return new WaitForSeconds(time);
                switch (currentState)
                {
                    case DEMON.Idle:
                        HandleIdleState();
                        break;
                    //랜덤 위치에서 볼 생성 후 터진다
                    case DEMON.Attack01:
                        ChangeState(DEMON.Idle);
                        break;
                    // 볼 던진 위치에서 터진다
                    case DEMON.Attack02:
                        ChangeState(DEMON.Idle);
                        break;
                    case DEMON.Attack03:
                        ChangeState(DEMON.Idle);
                        break;
                    case DEMON.Teleport:
                        ChangeState(DEMON.Idle);
                        break;
                    case DEMON.GetDamaged:
                        ChangeState(DEMON.Idle);
                        break;
                    case DEMON.Die:
                        break;
                }
            }
            if (!istimeing)
            {
                switch (currentState)
                {
                    case DEMON.Idle:
                        HandleIdleState();
                        break;
                    //랜덤 위치에서 볼 생성 후 터진다
                    case DEMON.Attack01:
                        ChangeState(DEMON.Idle);
                        break;
                    // 볼 던진 위치에서 터진다
                    case DEMON.Attack02:
                        ChangeState(DEMON.Idle);
                        break;
                    case DEMON.Attack03:
                        ChangeState(DEMON.Idle);
                        break;
                    case DEMON.Teleport:
                        ChangeState(DEMON.Idle);
                        break;
                    case DEMON.GetDamaged:
                        ChangeState(DEMON.Idle);
                        break;
                    case DEMON.Die:
                        break;
                }
            }
        }
        // 상태 전환
        #region StateMode
        public void ChangeState(DEMON newState)
        {
            if (currentState == newState) return;

            currentState = newState;

            // 상태에 따른 애니메이션 재생
            animator.SetTrigger(newState.ToString());
        }
        public void ChangeFloatState(DEMON newState, float newfloat)
        {
            if (currentState == newState) return;

            currentState = newState;

            // 상태에 따른 애니메이션 재생
            animator.SetFloat(newState.ToString(), newfloat);
        }
        public void ChangeBoolState(DEMON newState, bool newbool)
        {
            if (currentState == newState) return;

            currentState = newState;

            // 상태에 따른 애니메이션 재생
            animator.SetBool(newState.ToString(), newbool);
        }
        #endregion
        // 상태 처리
        private void HandleIdleState()
        {
            ResetTriggers();
            if(hasRecovered)
            {
                NextPhase();
                return;
            }
            /*if(Vector3.Distance(transform.position,pattern.player.position) < attackRange)
            {
                ChangeState(DEMON.Attack03);
                return;
            }*/
            index = Random.Range(0, demons.Count);
            if (Time.time - lastAttackTime[index] >= attackCooldown[index]/* &&
                Vector3.Distance(transform.position, pattern.player.position) > attackRange*/)
            {
                switch (index)
                {
                    case 0:
                        ChangeState(demons[0]);
                        break;
                    case 1:
                        ChangeState(demons[1]);
                        break;
                    case 2:
                        ChangeState(demons[2]);
                        break;
                }
                return;
            }
            else
            {
                ChangeState(DEMON.Idle);
            }
        }
        // 애니메이션 초기화
        public void ResetTriggers()
        {
            animator.ResetTrigger("Attack01");
            animator.ResetTrigger("Attack02");
            animator.ResetTrigger("Attack03");
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("Teleport");
            animator.SetFloat("GetDamaged", 0);
        }
        public void TakeDamage(float damage)
        {
            if (sceneManager.drectingCamera.activeSelf) return;
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 체력 범위 제한
            Debug.Log($"currentHealth : {currentHealth}");
            ChangeFloatState(DEMON.GetDamaged,damage);
            if (currentHealth <= 0)
            {
                if (!isDie)
                {
                    PrepareClear();
                }
            }
        }
        void UpdateHealthBar()
        {
            // 부드러운 FillAmount 업데이트
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, currentHealth / maxHealth, Time.deltaTime * 5f);

            // 퍼센트 텍스트 업데이트
            float healthPercent = (currentHealth / maxHealth) * 100;
            healthText.text = $"{healthPercent:F1}%";
        }
        private void RecoverHealth()
        {
            // 감소한 체력의 절반만큼 회복
            currentHealth = maxHealth * 0.75f;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 체력 범위 제한
            hasRecovered = true; // 회복 플래그 활성화
            animator.SetBool("IsRecovered", hasRecovered);
        }
        void PrepareClear()
        {
            isDie = true;
            PlayerHealth health = pattern.player.GetComponentInChildren<PlayerHealth>();
            AchievementManager.Instance.UpdateAchievement(AchievementType.HealthBased, health.TotalDamagePersentage);
            /************************************************************************************/
            // TODO : UpdateAchievementData(KillCount, 1) 불러오기
            AchievementManager.Instance.UpdateAchievement(AchievementType.KillCount, 1);
            /************************************************************************************/
            clearTime.StopTimer();
            sceneManager.drectingCamera.SetActive(true);
            ChangeBoolState(DEMON.Die, isDie);
            PlayerInputActions inputActions = pattern.player.GetComponent<PlayerInputActions>();
            inputActions.UnInputActions();
            source.clip = pattern.audioManager.SetAudioClip(7);
            source.Play();
            Invoke("Clear",5f);
        }
        void Clear()
        {
            Destroy(this.gameObject);
            clearTime.CompleteDungeon();
        }
    }
}