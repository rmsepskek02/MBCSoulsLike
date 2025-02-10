using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using BS.Utility;
using NUnit.Framework;
using System.Collections.Generic;

namespace BS.Achievement
{
    /// <summary>
    /// 업적 데이터 풀 클래스.
    /// 업적 데이터를 XML 파일로부터 읽어와서 메모리에 로드합니다.
    /// 이 클래스는 ScriptableObject로 작성되어 에디터와 런타임에서 사용 가능합니다.
    /// </summary>

    [System.Serializable]
    public class stream
    {
        public string bossName;
        public List<AchievementData> list;
    }

    public class AchievementDataPool : ScriptableObject
    {
        #region Variables
        /// <summary>
        /// 업적 데이터를 저장하는 객체.
        /// XML에서 읽어온 데이터를 이 변수에 할당합니다.
        /// </summary>
        public Achievements Achievements;
        public List<stream> streams = new List<stream>();


        /// <summary>
        /// Resources 폴더 내 업적 데이터 파일 경로.
        /// 데이터 파일은 "Resources/Data/AchievementData.xml"에 위치해야 합니다.
        /// </summary>
        private string dataPath = "Data/Achievements/AchievementData";
        #endregion

        #region Methods
        /// <summary>
        /// XML 파일에서 업적 데이터를 읽어와 Achievements 변수에 저장합니다.
        /// </summary>
        public void LoadData()
        {
            // ResourcesManager를 사용하여 XML 파일을 로드.
            TextAsset asset = (TextAsset)ResourcesManager.Load(dataPath);

            // 파일이 없거나 내용이 비어있다면 로드를 중단.
            if (asset == null || asset.text == null)
            {
                Debug.LogWarning($"Failed to load achievement data from path: {dataPath}");
                return;
            }

            List<string> preBossName = new List<string>();

            // XML 데이터를 읽고 역직렬화하여 Achievements 객체에 저장.
            using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
            {
                // Achievements 클래스 타입으로 역직렬화를 위한 XmlSerializer 생성.
                var xs = new XmlSerializer(typeof(Achievements));

                // XML 데이터를 Achievements 타입으로 역직렬화.
                Achievements = (Achievements)xs.Deserialize(reader);
                Dictionary<string, List<AchievementData>> test = new Dictionary<string, List<AchievementData>>();
                foreach (var ac in Achievements.achievements)
                {

                    if (!test.ContainsKey(ac.bossType.ToString()))
                    {
                        test[ac.bossType.ToString()] = new List<AchievementData>();
                    }
                    test[ac.bossType.ToString()].Add(ac);
                }
                foreach (var key in test)
                {
                    stream stream = new stream();
                    stream.bossName = key.Key;
                    stream.list = key.Value;
                    streams.Add(stream);
                }
            }
        }

        /// <summary>
        /// Achievements 변수를 XML 파일로 저장합니다.
        /// </summary>
        public void SaveData()
        {
            // XML 직렬화를 위한 XmlSerializer 생성.
            var xs = new XmlSerializer(typeof(Achievements));

            // XML 파일을 저장할 경로 설정.
            string filePath = Path.Combine(Application.dataPath, "Resources", dataPath + ".xml");

            // 파일 스트림을 열고 직렬화하여 저장.
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                xs.Serialize(stream, Achievements);
            }

            Debug.Log($"Achievement data saved to: {filePath}");
        }
        #endregion
    }
}
