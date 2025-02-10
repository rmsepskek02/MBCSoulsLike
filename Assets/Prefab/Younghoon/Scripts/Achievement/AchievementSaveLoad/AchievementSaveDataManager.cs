using BS.Utility;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace BS.Achievement
{
    public class AchievementSaveDataManager : MonoBehaviour
    {
        private string savePath;

        private void Awake()
        {
            string directoryPath;

            // 에디터에서는 Assets/SaveLoad/ 폴더에 저장
#if UNITY_EDITOR
            directoryPath = Path.Combine(Application.dataPath, "AchievementSaveLoad");
#else
    // 빌드된 버전에서는 Application.persistentDataPath 사용
    directoryPath = Path.Combine(Application.persistentDataPath, "AchievementSaveLoad");
#endif

            // 폴더가 존재하지 않으면 폴더 생성
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // 파일 경로 설정
            savePath = Path.Combine(directoryPath, SceneManager.GetActiveScene().name + "AchievementSaveData" + ".json");
        }

        /// <summary>
        /// 데이터를 JSON 형식으로 저장합니다.
        /// </summary>
        public void SaveData(AchievementSaveDatas dataContainer)
        {
            // 데이터를 JSON 문자열로 직렬화
            string json = JsonUtility.ToJson(dataContainer, true);

            // 파일로 저장
            File.WriteAllText(savePath, json);
            Debug.Log($"Data saved to {savePath}");
        }

        /// <summary>
        /// JSON 파일에서 데이터를 로드합니다.
        /// </summary>
        public AchievementSaveDatas LoadData()
        {
            // 파일이 존재하지 않으면 초기화된 컨테이너 반환
            if (!File.Exists(savePath))
            {
                AchievementSaveDatas achievementSaveDatas = new AchievementSaveDatas();

                List<AchievementData> achievements = DataManager.GetAchievementData()
                    .Achievements
                    .achievements
                    .Where(achievement => achievement != null && achievement.bossType.ToString() == SceneManager.GetActiveScene().name)
                    .ToList();

                int startNum = achievements[0].number;
                for (int i = startNum; i < startNum + achievements.Count; i++)
                {
                    AchievementSaveData achievementSaveData = new AchievementSaveData();
                    achievementSaveData.number = i;
                    achievementSaveData.currentAmount = 0;
                    //achievementSaveData.isUnlock = false;
                    achievementSaveData.isClear = false;

                    // 3의 배수(0, 3, 6, ...)인 경우 isUnlock = true
                    if ((i - startNum) % 3 == 0)
                    {
                        achievementSaveData.isUnlock = true;
                    }
                    else
                    {
                        achievementSaveData.isUnlock = false;
                    }

                    achievementSaveDatas.achievementSaveData.Add(achievementSaveData);
                }


                SaveData(achievementSaveDatas);
                return achievementSaveDatas;
            }

            // JSON 파일 읽기
            string json = File.ReadAllText(savePath);

            // JSON 문자열을 객체로 역직렬화
            AchievementSaveDatas dataContainer = JsonUtility.FromJson<AchievementSaveDatas>(json);

            //Debug.Log($"Data loaded from {savePath}");
            return dataContainer;
        }
    }
}