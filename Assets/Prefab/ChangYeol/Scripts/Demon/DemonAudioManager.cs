using BS.Utility;
using System.Collections.Generic;
using UnityEngine;


namespace BS.Audio
{
    /// <summary>
    /// Sound 종류별로 정리한 클래스
    /// </summary>
    [System.Serializable]
    public class Sounds
    {
        public string audioName;
        public AudioClip audioClip;
        public AudioUtility.AudioGroups group;
    }
    /// <summary>
    /// Sounds 클래스의 모든 데이터를 내보내는 Manager
    /// </summary>
    public class DemonAudioManager : MonoBehaviour
    {
        #region Variables
        public List<Sounds> sounds;
        #endregion

        public AudioClip SetAudioClip(int index)
        {
            return sounds[index].audioClip;
        }
        public AudioUtility.AudioGroups SetGroups(int index)
        {
            return sounds[index].group;
        }
    }
}