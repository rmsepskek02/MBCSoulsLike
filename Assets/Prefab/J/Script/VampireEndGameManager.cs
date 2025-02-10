using BS.Player;
using UnityEngine;
using BS.UI;
using BS.PlayerInput;
using BS.vampire;
using BS.Achievement;

namespace BS.Utility
{
    public class VampireEndGameManager : MonoBehaviour
    {
        #region Variables
        private PlayerHealth playerHealth;
        private VampireHealth bossHealth;
        private DungeonClearTime dungeEndGame;
        public GameObject player;
        public GameObject boss;
        public GameObject VampireDummy;
        [SerializeField] private float actorTime=3f;
        public AudioSource audioSource;
        public AudioClip clearSound;
        public AudioClip defeatSound;
        public Animator animator;
   
        private PlayerInputActions playerInputActions;

        private bool gameEnded = false;
        #endregion

        private void Start()
        {
            animator= VampireDummy.GetComponent<Animator>();
            dungeEndGame = FindFirstObjectByType<DungeonClearTime>();
            bossHealth = FindFirstObjectByType<VampireHealth>();
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
            dungeEndGame.StopTimer();
            AchievementManager.Instance.UpdateAchievement(AchievementType.KillCount, 1);
            AchievementManager.Instance.UpdateAchievement(AchievementType.HealthBased, playerHealth.TotalDamagePersentage);
            gameEnded = true;
            playerInputActions.enabled = false;
            VampireDummy.SetActive(true);
            animator.SetTrigger("Win");
            boss.GetComponent<VampireController>().enabled = false;
            boss.GetComponent<PattonSummon>().enabled = false;
            audioSource.clip = clearSound;
            audioSource.Play();
            Destroy(boss);
            Debug.Log("패배");
            Invoke("Clear", actorTime); 
        }

        private void PrepareDefeat()
        {
            AchievementManager.Instance.UpdateAchievement(AchievementType.HealthBased, playerHealth.TotalDamagePersentage);
            gameEnded = true;
            playerInputActions.enabled = false;
            VampireDummy.SetActive(true);
            boss.GetComponent<VampireController>().enabled = false;
            boss.GetComponent<PattonSummon>().enabled = false;
            animator.SetTrigger("Defeat");
            audioSource.clip = defeatSound;
            audioSource.Play();
            Destroy(boss);
            Invoke("Defeat", actorTime); 
        }

        private void Clear()
        {
          
           
         
        
            dungeEndGame.CompleteDungeon();
        }

        private void Defeat()
        {
          
        
            dungeEndGame.DefeatDungeon();
        }
    }
}
