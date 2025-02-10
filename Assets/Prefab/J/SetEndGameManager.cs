using BS.Player;
using UnityEngine;
using BS.UI;
using BS.Enemy.Set;
using BS.Managers;
using System.Collections;
using BS.Achievement;

namespace BS.Utility
{
    public class SetEndGameManager : MonoBehaviour
    {
        #region Variables
        private PlayerHealth playerHealth;
        private SetHealth bossHealth;

        private SetDungeonClearTime dungeonEndGame;

        public AudioSource audioSource;

        public AudioClip clearBGM;
        public AudioClip defeatBGM;

        private Camera mainCamera;

        private bool isEnding;
        private bool clearOrNot;

        [SerializeField] private float endingFieldOfView = 30f;
        [SerializeField] private float zoomSpeed = 2f;

        [SerializeField] private float showAchievementUITimer = 5f;


        #endregion

        private void Start()
        {
            isEnding = false;

            mainCamera = Camera.main;

            dungeonEndGame = FindFirstObjectByType<SetDungeonClearTime>();
            if (dungeonEndGame != null)
            {
                Debug.Log("dungeonEndGame 값있음");
            }
            else
            {
                Debug.Log("dungeonEndGame 값없음");
            }
            bossHealth = FindFirstObjectByType<SetHealth>();
            playerHealth = FindFirstObjectByType<PlayerHealth>();

            bossHealth.OnDie += EndingProduction;
            playerHealth.OnDie += EndingProduction;
        }

        private void Update()
        {
            if (isEnding)
            {
                EndingCameraProduction();
            }
        }

        private void EndingCameraProduction()
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, endingFieldOfView, Time.unscaledDeltaTime * zoomSpeed);
        }

        private void EndingProduction()
        {
            AchievementManager.Instance.UpdateAchievement(AchievementType.HealthBased, playerHealth.TotalDamagePersentage);
            dungeonEndGame.StopTimer();
            bossHealth.gameObject.GetComponent<SetController>().enabled = false;
            Time.timeScale = 0.3f;
            mainCamera.GetComponent<CameraManager>().enabled = false;

            clearOrNot = playerHealth.GetRatio() > bossHealth.GetRatio() ? true : false;

            if (playerHealth.GetRatio() > bossHealth.GetRatio())
            {
                mainCamera.transform.LookAt(bossHealth.gameObject.transform);
                audioSource.clip = clearBGM;
            }
            else
            {
                mainCamera.transform.LookAt(playerHealth.gameObject.transform);
                audioSource.clip = defeatBGM;
            }
            audioSource.Play();
            isEnding = true;
            StartCoroutine(ShowAchivementManager(clearOrNot));
        }

        private IEnumerator ShowAchivementManager(bool clearOrNot)
        {
            yield return new WaitForSecondsRealtime(showAchievementUITimer);
            Time.timeScale = 1f;
            if (clearOrNot)
            {
                dungeonEndGame.CompleteDungeon();
            }
            else
            {
                dungeonEndGame.DefeatDungeon();
            }

        }
    }
}
