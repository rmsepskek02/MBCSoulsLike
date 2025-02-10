using BS.Player;
using UnityEngine;
using BS.UI;
using BS.Audio;
using UnityEngine.Audio;
using BS.PlayerInput;
using BS.Achievement;

namespace BS.Utility
{
    public class AlienEndGameManager : MonoBehaviour
    {
        #region Variables
        private PlayerHealth playerHealth;
        private AlienHealth bossHealth;
        private DungeonClearTime dungeEndGame;
        public GameObject player;
        public GameObject boss;
        private float actorTime = 5f;
        public AudioSource audioSource;
        public AudioClip clearSound;
        public AudioClip defeatSound;
        public AudioClip titleBgm;
        private PlayerInputActions playerInputActions;

        private bool gameEnded = false;
        #endregion

        private void Start()
        {
            dungeEndGame = FindFirstObjectByType<DungeonClearTime>();
            bossHealth = FindFirstObjectByType<AlienHealth>();
            playerHealth = FindFirstObjectByType<PlayerHealth>();
            playerInputActions = player.GetComponent<PlayerInputActions>();
        }

        private void Update()
        {
            if (!gameEnded)
            {
                CheckGameEnd();
            }
        }

        private void CheckGameEnd()
        {
            if (bossHealth.currentHealth <= 0)
            {
                PrepareClear();
            }
            else if (playerHealth.CurrentHealth <= 0)
            {
                PrepareDefeat();
            }
        }

        private void PrepareClear()
        {
            AchievementManager.Instance.UpdateAchievement(AchievementType.HealthBased, playerHealth.TotalDamagePersentage);
            dungeEndGame.StopTimer();
            gameEnded = true;
            playerInputActions.enabled = false;
            audioSource.clip = clearSound;
            audioSource.Play();
            Invoke("Clear", actorTime);
        }

        private void PrepareDefeat()
        {
            gameEnded = true;
            playerInputActions.enabled = false;
            audioSource.clip = defeatSound;
            audioSource.Play();
            Invoke("Defeat", actorTime);
        }

        private void Clear()
        {
            //Destroy(boss);
            dungeEndGame.CompleteDungeon();
        }

        private void Defeat()
        {
            //Destroy(boss); 
            dungeEndGame.DefeatDungeon();
        }
    }
}
