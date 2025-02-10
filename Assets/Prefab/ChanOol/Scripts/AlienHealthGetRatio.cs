using BS.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BS.UI
{
    public class AlienHealthGetRatio : MonoBehaviour
    {
        #region Variables
        public AlienHealth bossHealth;
        public PlayerHealth playerHealth;
        public Image bosshealthBarImage;
        public Image playerhealthBarImage;
        public TextMeshProUGUI bossHealthText; // 보스 체력 텍스트
        public TextMeshProUGUI playerHealthText; // 플레이어 체력 텍스트
        public TextMeshProUGUI potionText;  // 포션 갯수 텍스트
        public UsePotion usePotion;
        #endregion

        private void Start()
        {
            PosionCountChecker();
            usePotion.UsedPotion += PosionCountChecker;
        }

        private void Update()
        {
            if (bossHealth != null)
            {
                float bossHealthRatio = bossHealth.currentHealth / bossHealth.maxHealth;

                bosshealthBarImage.fillAmount = bossHealthRatio;

                bossHealthText.text = $"{bossHealthRatio * 100:F0}%"; // 보스 체력 퍼센트 
            }
            // 컴포넌트가 할당되었는지 확인 후 업데이트
            if (playerHealth != null)
            {
                float playerHealthRatio = playerHealth.CurrentHealth / playerHealth.MaxHealth;

                playerhealthBarImage.fillAmount = playerHealthRatio;
                //potionText.text = playerHealth.potionCount.ToString();
                playerHealthText.text = $"{playerHealthRatio * 100}%"; // 플레이어 체력 퍼센트
            }
        }
        private void PosionCountChecker()
        {
            potionText.text = usePotion.PotionCount.ToString();
        }
    }
}
