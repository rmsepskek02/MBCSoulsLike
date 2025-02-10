using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BS.Achievement
{
    [Serializable]
    public class AchievementData
    {
        public int number;
        public BossType bossType;            // 업적 대상 보스 타입
        public AchievementType achievementType;         // 업적 유형
        public AchievementName name;       // 업적 이름
        public string description;           // 업적 설명
        public float requiredHealth;         // 체력 기준
        public int requiredKills;            // 처치 횟수 기준
        public float requiredTime;           // 시간 기준
        public int next;
    }

    public enum AchievementName
    {
        도전자,
        전문가,
        고인물,
    }

    //public enum AchievementName
    //{
    //    Challenger,
    //    Expert,
    //    Master,
    //}

    public enum AchievementType
    {
        HealthBased,   // 체력 관련 업적
        KillCount,     // 처치 관련 업적
        TimeBased      // 시간 관련 업적
    }

    public enum BossType
    {
        Vampire,
        Set,
        Alien,
        Demon,
    }

    public class Achievements
    {
        public List<AchievementData> achievements = new List<AchievementData>();
    }
}