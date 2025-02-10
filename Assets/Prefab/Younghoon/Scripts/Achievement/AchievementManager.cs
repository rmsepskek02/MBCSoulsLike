using UnityEngine;
using System.Collections.Generic;
using BS.Utility;
using UnityEngine.SceneManagement;
using System.Linq;
using BS.UI;

namespace BS.Achievement
{
    public class AchievementManager : Singleton<AchievementManager>
    {
        #region Variables
        public List<AchievementObject> achievementsGoalCondition = new List<AchievementObject>();
        public List<AchievementData> realAchievements = new List<AchievementData>();
        public AchievementSaveDatas achievementSaveDatas;

        private AchievementSaveDataManager achievementSaveDataManager;  // 데이터 매니저 참조
        #endregion

        private void Start()
        {
            achievementSaveDataManager = GetComponent<AchievementSaveDataManager>();

            #region Linq 설명
            //DataManager.GetAchievementData() 호출: 업적 데이터를 반환하는 객체를 가져옵니다.
            //.Achievements 참조: 전체 업적 데이터를 담고 있는 객체에 접근합니다.
            //.achievements 참조: 업적 데이터를 리스트 형태로 가져옵니다.
            //.Where(...) 호출: 조건(null이 아니며, bossType과 현재 씬 이름이 동일)을 만족하는 데이터만 필터링합니다.
            //.ToList() 호출: 필터링된 결과를 새로운 리스트로 변환하여 checkAchievements에 저장합니다.
            #endregion
            realAchievements = DataManager.GetAchievementData()
                    .Achievements
                    .achievements
                    .Where(achievement => achievement != null && achievement.bossType.ToString() == SceneManager.GetActiveScene().name)
                    .ToList();

            LoadAchievementData();

            for (int i = 0; i < realAchievements.Count; i++)
            {
                achievementsGoalCondition.Add(new AchievementObject(realAchievements[i]));
                achievementsGoalCondition[i].isUnlock = achievementSaveDatas.achievementSaveData[i].isUnlock;
                achievementsGoalCondition[i].isClear = achievementSaveDatas.achievementSaveData[i].isClear;
                achievementsGoalCondition[i].achievementGoal.currentAmount = achievementSaveDatas.achievementSaveData[i].currentAmount > achievementsGoalCondition[i].achievementGoal.goalAmount ?
                    achievementsGoalCondition[i].achievementGoal.goalAmount : achievementSaveDatas.achievementSaveData[i].currentAmount;
                achievementsGoalCondition[i].achievementGoal.nextStep = realAchievements[i].next > 0;
            }

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                UpdateAchievement(AchievementType.KillCount, 1);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                SaveAchievementData();
            }
        }

        private void LoadAchievementData()
        {
            // 현재 씬 이름을 기준으로 저장된 데이터를 로드
            achievementSaveDatas = achievementSaveDataManager.LoadData();

            // 로드한 데이터를 achievementSaveDatas에 추가
            //achievementSaveDatas.Add(loadData);

        }

        public void SaveAchievementData()
        {
            for (int i = 0; i < achievementSaveDatas.achievementSaveData.Count; i++)
            {
                achievementSaveDatas.achievementSaveData[i].isUnlock = achievementsGoalCondition[i].isUnlock;
                achievementSaveDatas.achievementSaveData[i].isClear = achievementsGoalCondition[i].isClear;
                achievementSaveDatas.achievementSaveData[i].currentAmount = achievementsGoalCondition[i].achievementGoal.currentAmount;
            }
            achievementSaveDataManager.SaveData(achievementSaveDatas);
        }

        public void UpdateAchievement(AchievementType type, float amount)
        {
            // 해당 타입(AchievementType)에 해당하는 업적을 찾는다.
            // achievementsGoalCondition 리스트에서 achievementType이 type과 같은 요소들을 필터링하여 list에 저장한다.
            List<AchievementObject> list = achievementsGoalCondition
                .Where(goal => goal.achievementType == type) // 조건에 맞는 업적 필터링
                .ToList(); // 필터링된 결과를 리스트로 변환

            // 찾은 업적 리스트를 순회하면서 조건을 체크하고 업데이트한다.
            foreach (var achievement in list)
            {
                // 업적 타입에 따라 다른 동작을 수행한다.
                switch (type)
                {
                    case AchievementType.HealthBased:
                        // Health 관련 업적 업데이트 로직 (아직 구현되지 않음)
                        // TODO: 체력 관련 업적 로직 추가 필요
                        achievement.DamageCheck(amount);
                        break;

                    case AchievementType.KillCount:
                        // 보스 처치 업적 업데이트
                        achievement.BossKill(); // 보스를 처치할 때 카운트 증가
                        break;

                    case AchievementType.TimeBased:
                        // 클리어 타임 업적 업데이트
                        achievement.ClearTime(amount); // 주어진 시간(amount)을 기록
                        break;

                    default:
                        // 만약 예상치 못한 업적 타입이 들어오면 경고 메시지를 출력하고 실행을 중단한다.
                        Debug.LogWarning($"{type}이 잘못되었습니다.");
                        return; // switch 문에서 잘못된 타입이면 바로 함수 종료
                }

                // 업적이 클리어되었는지 확인한다.
                if (achievement.achievementGoal.IsCleared(type)) // IsCleared(type)가 true이면 업적 완료
                {
                    achievement.isClear = true; // 업적 완료 상태로 변경

                    // 현재 업적이 완료되었으므로, 다음 업적을 잠금 해제할 수 있는지 확인한다.
                    int index = list.IndexOf(achievement); // 현재 업적의 리스트 내 인덱스를 가져옴

                    if (index + 1 < list.Count) // 다음 업적이 존재하는 경우
                    {
                        list[index + 1].isUnlock = true; // 다음 업적의 잠금을 해제한다.
                    }
                }
            }
        }

    }
}