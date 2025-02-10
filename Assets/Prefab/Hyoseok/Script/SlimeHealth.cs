using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BS.vampire
{
    public class SlimeHealth : MonoBehaviour, IDamageable
    {
        #region Variables
        [SerializeField] private float maxHealth = 100f;    //최대 Hp
        [SerializeField]public float CurrentHealth { get; set; }    //현재 Hp
        //private bool isDeath = false;                       //죽음 체크

        public UnityAction<float> OnDamaged;
        public UnityAction OnDie;


        //체력 위험 경계율
        [SerializeField] private float criticalHealRatio = 0.3f;

        public Image bossHealth;
        public TextMeshProUGUI healthText;
        #endregion


        //UI HP 게이지 값
        public float GetRatio() => CurrentHealth / maxHealth;
        //위험 체크
        public bool IsCritical() => GetRatio() <= criticalHealRatio;


        private void Start()
        {
            //초기화
            CurrentHealth = maxHealth;
        }

        private void Update()
        {
            bossHealth.fillAmount = Mathf.Lerp(bossHealth.fillAmount, GetRatio(), Time.deltaTime * 5f);
            float healthPercent = (CurrentHealth / maxHealth) * 100f;
            healthText.text = $"{healthPercent:F1}%";
        }

        //damageSource: 데미지를 주는 주체
        public void TakeDamage(float damage)
        {
            float beforeHealth = CurrentHealth;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, maxHealth);
            Debug.Log($"{gameObject.name} CurrentHealth: {CurrentHealth}");

            //real Damage 구하기
            float realDamage = beforeHealth - CurrentHealth;
            if (realDamage > 0f)
            {
                //데미지 구현                
                OnDamaged?.Invoke(realDamage);
            }

            //죽음 처리
            HandleDeath();
        }

        //죽음 처리 관리
        void HandleDeath()
        {
            if (CurrentHealth <= 0f)
            {
                CurrentHealth = maxHealth;
            }
        }
    }
}