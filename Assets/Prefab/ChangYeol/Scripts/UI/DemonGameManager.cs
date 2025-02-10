using BS.Achievement;
using BS.Player;
using BS.PlayerInput;
using BS.UI;
using UnityEngine;

namespace BS.Demon
{
    public class DemonGameManager : MonoBehaviour
    {
        #region Variables
        private PlayerHealth playerHealth;
        public DemonPattern bossHealth;
        public GameObject player;
        private DungeonClearTime dungeEndGame;
        private float actorTime = 5f;
        private PlayerInputActions playerInputActions;

        [HideInInspector] public bool gameEnded = false;
        #endregion
        private void Start()
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
            dungeEndGame = FindFirstObjectByType<DungeonClearTime>();
            playerInputActions = player.GetComponent<PlayerInputActions>();
        }
        private void Update()
        {
            if (!gameEnded)
            {
                if (playerHealth.CurrentHealth <= 0)
                {
                    PrepareDefeat();
                }
            }
        }
        private void PrepareDefeat()
        {
            gameEnded = true;
            PlayerHealth health = player.GetComponentInChildren<PlayerHealth>();
            AchievementManager.Instance.UpdateAchievement(AchievementType.HealthBased, health.TotalDamagePersentage);
            bossHealth.demon.sceneManager.drectingCamera.SetActive(true);
            bossHealth.demon.animator.SetBool("IsDefeat", true);
            playerInputActions.UnInputActions();
            bossHealth.demon.source.clip = bossHealth.audioManager.SetAudioClip(8);
            bossHealth.demon.source.Play();
            Invoke("Defeat", actorTime);
        }
        private void Defeat()
        {
            dungeEndGame.DefeatDungeon();
            Destroy(bossHealth.gameObject);
        }
    }
}