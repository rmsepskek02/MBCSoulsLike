using System;
using UnityEngine;

namespace BS.Achievement
{
    /// <summary>
    /// 업적 진행 데이터 관리 클래스
    /// </summary>
    [Serializable]
    public class AchievementObject
    {
        #region Variables최초 처치

        public int number;                          //업적의 인덱스
        public AchievementType achievementType;     //업적의 타입
        public AchievementGoal achievementGoal;     //업적 목표

        public bool isUnlock = false;
        public bool isClear = false;

        #endregion

        public AchievementObject(AchievementData achievementData)
        {
            number = achievementData.number;
            achievementType = achievementData.achievementType;

            achievementGoal = new AchievementGoal(achievementData);
        }

        public void BossKill()
        {
            if (achievementGoal.achievementType == AchievementType.KillCount)
            {
                achievementGoal.currentAmount = Mathf.Min(++achievementGoal.currentAmount, achievementGoal.goalAmount);
            }
        }

        public void ClearTime(float accumulatedTime)
        {
            if (achievementGoal.achievementType == AchievementType.TimeBased)
            {
                // 최초 기록이거나 기존 값보다 작은 경우 갱신
                if (achievementGoal.currentAmount == 0 || accumulatedTime < achievementGoal.currentAmount)
                {
                    achievementGoal.currentAmount = accumulatedTime;
                }
            }
        }

        public void DamageCheck(float ratio)
        {
            if (achievementGoal.achievementType == AchievementType.HealthBased)
            {
                achievementGoal.currentAmount = ratio;
            }
        }
    }
}