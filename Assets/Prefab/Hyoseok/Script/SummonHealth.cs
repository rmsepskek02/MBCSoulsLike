using UnityEngine;
using UnityEngine.Events;

namespace BS.vampire
{
    public class SummonHealth : MonoBehaviour,IDamageable
    {
        #region Variables
        [SerializeField] private float maxHealth = 100f;    //최대 Hp
        public float CurrentHealth { get; private set; }    //현재 Hp
        private bool isDeath = false;                       //죽음 체크

        public UnityAction<float> OnDamaged;
        public UnityAction OnDie;
      

        //체력 위험 경계율
        [SerializeField] private float criticalHealRatio = 0.3f;

      
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
            //죽음 체크
            if (isDeath)
                return;

            if (CurrentHealth <= 0f)
            {
                isDeath = true;
                Destroy( gameObject);
                //죽음 구현
                OnDie?.Invoke();
            }
        }
    }
}