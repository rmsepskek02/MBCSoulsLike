using BS.UI;
using BS.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace BS.Player
{
    public class UsePotion : MonoBehaviour
    {
        private PlayerHealth playerHealth;
        public AudioClip potionSound;
        private DungeonClearTime dungeEndGame;
        private int potionCount = 3;
        public int PotionCount
        {
            get { return potionCount; }
        }
        private float healAmount;

        public UnityAction UsedPotion;

        private void Start()
        {
            if(dungeEndGame != null)
            {
                dungeEndGame = FindFirstObjectByType<DungeonClearTime>();
            }
            playerHealth = GetComponent<PlayerHealth>();
            //Debug.Log(playerHealth.MaxHealth + "맥스힐스 1번");
            healAmount = playerHealth.MaxHealth;
        }

        private void Update()
        {
            if(dungeEndGame != null)
            {
                if (dungeEndGame.isDungeonActive) return;
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                PotionUse();
            }
        }

        private void PotionUse()
        {
            if (potionCount <= 0 || playerHealth.IsDeath )
            {
                //포션이 없거나 죽었다면 리턴
                return;
            }

            playerHealth.TakeHeal();
            potionCount--;
            UsedPotion?.Invoke();
            AudioUtility.CreateSFX(potionSound, transform.position, AudioUtility.AudioGroups.Sound);

        }
    }
}
