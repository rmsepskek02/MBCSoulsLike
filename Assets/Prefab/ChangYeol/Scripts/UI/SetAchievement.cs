using BS.Player;
using BS.UI;
using NUnit.Framework;
using TMPro;
using UnityEngine;

namespace BS.Achievement
{
    public class SetAchievement : MonoBehaviour
    {
        #region Variables
        public GameObject achievement;
        public GameObject achievementList;
        public Sprite selectSprite;
        public Sprite noneSprite;
        public Color selectColor;
        private GameObject achievementobject;
        private AchievementManager manager;

        public SetDungeonClearTime dungeon;
        #endregion
        private void Start()
        {
            manager = AchievementManager.Instance;
            if (manager == null)
            {
                Debug.LogWarning("manager 오류");
            }
            CheckAchievement();
            manager.SaveAchievementData();
        }
        void CheckAchievement()
        {
            for (int i = 0; i < manager.achievementsGoalCondition.Count; i++)
            {
                //텍스트 색, 폰트스타일, 스프라이트, 이미지 색
                achievementobject = Instantiate(achievement);
                achievementobject.transform.SetParent(achievementList.transform, false);
                //achievementobject.transform.localScale = Vector3.one;
                if (achievementobject)
                {
                    AchievementObject gameachievement = manager.achievementsGoalCondition[i];
                    AchievementList achievement = achievementobject.GetComponent<AchievementList>();
                    //true이면 text 출력 false이면 ""출력
                    if (manager.achievementsGoalCondition[i].achievementType == AchievementType.KillCount)
                    {
                        achievement.achievementCondition.gameObject.SetActive(true);
                        achievement.achievementCondition.text = $"({manager.achievementsGoalCondition[i].achievementGoal.currentAmount} / " + $"{manager.achievementsGoalCondition[i].achievementGoal.goalAmount} )";
                    }
                    else
                    {
                        achievement.achievementCondition.gameObject.SetActive(false);
                        achievement.achievementCondition.text = $"";
                    }

                    achievement.achievementText.text = manager.realAchievements[i].description;
                    achievement.achievementCondition.color = gameachievement.isUnlock ? gameachievement.isClear ? selectColor : Color.white : Color.white;
                    achievement.achievementText.color = gameachievement.isUnlock ? gameachievement.isClear ? selectColor : Color.red : Color.gray;
                    achievement.achievementText.fontStyle = gameachievement.isUnlock ? gameachievement.isClear ? FontStyles.Normal : FontStyles.Strikethrough : FontStyles.Normal;
                    achievement.achievementImage.color = gameachievement.isUnlock ? gameachievement.isClear ? selectColor : Color.red : Color.gray;
                    achievement.achievementImage.sprite = gameachievement.isUnlock ? gameachievement.isClear ? selectSprite : noneSprite : noneSprite;
                }
            }
        }
    }
}