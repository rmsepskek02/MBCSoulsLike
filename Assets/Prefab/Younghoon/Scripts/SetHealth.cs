using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;


namespace BS.Enemy.Set
{
    public class SetHealth : MonoBehaviour, IDamageable
    {
        #region Variables

        [SerializeField] private float maxHealth;
        public float MaxHealth
        {
            get { return maxHealth; }
            private set { maxHealth = value; }
        }

        private float currentHealth;
        public float CurrentHealth
        {
            get { return currentHealth; }
            private set
            {
                //죽음 체크
                if (isDeath)
                    return;

                currentHealth = value;

                if (currentHealth <= 0f)
                {
                    isDeath = true;
                    OnDie?.Invoke();
                }
            }
        }

        private bool isDeath = false;           //죽음 체크

        public float GetRatio() => CurrentHealth / MaxHealth;

        private Image bosshealthBarImage;           // 보스 체력바
        private TextMeshProUGUI bossHealthText;     // 보스 체력 텍스트

        //UnityAction
        public UnityAction<float> OnDamaged;
        public UnityAction OnDie;

        #endregion

        private void Start()
        {
            currentHealth = MaxHealth;


        }

        #region Test용 데미지 주기 삭제 필
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                TakeDamage(maxHealth / 2);
            }


        }
        #endregion


        public void TakeDamage(float damage)
        {
            //죽었으면 실행하지 않음
            if (isDeath) return;

            //데미지 계산
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);

            OnDamaged?.Invoke(damage);
        }


    }
}