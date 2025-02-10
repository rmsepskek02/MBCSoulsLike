using UnityEngine;
using System;
using System.Collections.Generic;

namespace BS.Achievement
{
    [Serializable]
    public class AchievementSaveData
    {
        public int number;
        public float currentAmount;
        public bool isUnlock;
        public bool isClear;
    }

    [Serializable]
    public class AchievementSaveDatas
    {
        public List<AchievementSaveData> achievementSaveData = new List<AchievementSaveData>();
    }
}