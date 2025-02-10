using UnityEngine;
using BS.Achievement;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;

namespace BS.Utility
{
    public class DataManager : MonoBehaviour
    {
        private static AchievementDataPool achievementDataPool;

        public List<stream> stream;

        void Start()
        {
            //업적 데이터 가져오기
            if (achievementDataPool == null)
            {
                achievementDataPool = ScriptableObject.CreateInstance<AchievementDataPool>();
                achievementDataPool.LoadData();
                stream = achievementDataPool.streams;
            }
        }

        //업적 데이터 가져오기
        public static AchievementDataPool GetAchievementData()
        {
            if (achievementDataPool == null)
            {
                achievementDataPool = ScriptableObject.CreateInstance<AchievementDataPool>();
                achievementDataPool.LoadData();
            }
            return achievementDataPool;
        }

    }
}