using BS.Player;
using BS.vampire;
using BS.Enemy.Set;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BS.UI
{
    public class SetHealthUIManager : MonoBehaviour
    {
        #region Variables
        private SetHealth bossHealth;
        private PlayerHealth playerHealth;
        private Image bosshealthBarImage;
        private Image playerhealthBarImage;
        private TextMeshProUGUI bossHealthText; // 보스 체력 텍스트
        private TextMeshProUGUI playerHealthText; // 플레이어 체력 텍스트
        private UsePotion potion;
        private TextMeshProUGUI potionText;  // 포션 갯수 텍스트
        #endregion


        private void Start()
        {
            FindAndResetComponents();
        }

        private void FindAndResetComponents()
        {
            bossHealth = FindAnyObjectByType<SetHealth>();
            playerHealth = FindAnyObjectByType<PlayerHealth>();
            if (bossHealth == null || playerHealth == null)
            {
                Debug.Log("HealthScript 연결 오류");
            }

            bossHealth.OnDamaged += BossOnDamaged;
            playerHealth.OnDamaged += PlayerOnDamaged;
            playerHealth.OnHealed += PlayerOnHealed;

            #region BossComponents 연결
            GameObject bossHealthBar = GameObject.Find("BossHealthBar");

            if (bossHealthBar != null)
            {
                // bossHealthBar의 모든 하위 오브젝트에서 컴포넌트를 가진 오브젝트들을 찾음
                // FirstOrDefault()는 첫 번째로 조건을 만족하는 요소를 반환하고, 없으면 null을 반환
                bosshealthBarImage = bossHealthBar.GetComponentsInChildren<Image>()
                    .FirstOrDefault(img => img.gameObject.name == "FillAmount");

                bossHealthText = bossHealthBar.GetComponentsInChildren<TextMeshProUGUI>()
                    .FirstOrDefault(txt => txt.gameObject.name == "HealthText");

                if (bosshealthBarImage == null)
                {
                    Debug.LogError("FillAmount 에러");
                }
                if (bossHealthText == null)
                {
                    Debug.LogError("HealthText 에러");
                }
            }
            else
            {
                Debug.LogWarning("BossHealthBar 오브젝트를 찾을 수 없습니다.");
            }
            #endregion

            #region PlayerComponents 연결
            GameObject playerHealthBar = GameObject.Find("PlayerHealthBar");

            if (playerHealthBar != null)
            {
                // playerHealthBar의 모든 하위 오브젝트에서 컴포넌트를 가진 오브젝트들을 찾음
                // FirstOrDefault()는 첫 번째로 조건을 만족하는 요소를 반환하고, 없으면 null을 반환
                playerhealthBarImage = playerHealthBar.GetComponentsInChildren<Image>()
                    .FirstOrDefault(img => img.gameObject.name == "FillAmount");

                playerHealthText = playerHealthBar.GetComponentsInChildren<TextMeshProUGUI>()
                    .FirstOrDefault(txt => txt.gameObject.name == "HealthText");

                potionText = playerHealthBar.GetComponentsInChildren<TextMeshProUGUI>()
                    .FirstOrDefault(txt => txt.gameObject.name == "HealText");

                if (playerhealthBarImage == null)
                {
                    Debug.LogError("FillAmount 에러");
                }
                if (playerHealthText == null)
                {
                    Debug.LogError("HealthText 에러");
                }
            }
            else
            {
                Debug.LogWarning("PlayerHealthBar 오브젝트를 찾을 수 없습니다.");
            }
            #endregion


            potion = FindAnyObjectByType<UsePotion>();
            if (potion == null)
            {
                Debug.LogWarning("Potion Script를 찾을 수 없습니다.");
            }
            potion.UsedPotion += PosionCountChecker;

            PosionCountChecker();
        }

        private void BossOnDamaged(float Ratio)
        {
            bosshealthBarImage.fillAmount = bossHealth.GetRatio();
            bossHealthText.text = $"{bossHealth.GetRatio() * 100:F0}%";
        }

        private void PlayerOnDamaged(float Ratio)
        {
            playerhealthBarImage.fillAmount = playerHealth.GetRatio();
            playerHealthText.text = $"{playerHealth.GetRatio() * 100:F0}%";
        }

        private void PlayerOnHealed(float Ratio)
        {
            playerhealthBarImage.fillAmount = playerHealth.GetRatio();
            playerHealthText.text = $"{playerHealth.GetRatio() * 100:F0}%";
        }

        private void PosionCountChecker()
        {
            potionText.text = potion.PotionCount.ToString();
        }
    }
}
