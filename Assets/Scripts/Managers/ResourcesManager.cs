using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace BS.Utility
{
    /// <summary>
    /// 리소스 로드 및 인스턴스화를 관리하는 유틸리티 클래스.
    /// Unity의 Resources 폴더에서 오브젝트를 로드하거나, 
    /// 해당 오브젝트를 인스턴스화(Instantiate)하는 기능을 제공합니다.
    /// </summary>
    public class ResourcesManager : MonoBehaviour
    {
        /// <summary>
        /// 지정된 경로에서 Unity 오브젝트를 로드합니다.
        /// </summary>
        /// <param name="path">Resources 폴더 내의 파일 경로. 확장자는 제외합니다.</param>
        /// <returns>로드된 UnityObject를 반환합니다. 로드 실패 시 null을 반환합니다.</returns>
        public static UnityObject Load(string path)
        {
            // Unity의 Resources.Load 메서드를 사용하여 오브젝트를 로드.
            return Resources.Load(path);
        }

        /// <summary>
        /// 지정된 경로에서 Unity 오브젝트를 로드한 후, GameObject로 인스턴스화합니다.
        /// </summary>
        /// <param name="path">Resources 폴더 내의 파일 경로. 확장자는 제외합니다.</param>
        /// <returns>
        /// 인스턴스화된 GameObject를 반환합니다.
        /// 로드 실패 시 또는 해당 오브젝트가 GameObject가 아닌 경우 null을 반환합니다.
        /// </returns>
        public static GameObject LoadAndInstantiate(string path)
        {
            // 지정된 경로에서 UnityObject를 로드합니다.
            UnityObject source = Load(path);

            // 로드된 오브젝트가 null인지 확인.
            // null이면 로드 실패이므로 null 반환.
            if (source == null)
            {
                return null;
            }

            // 로드된 오브젝트를 GameObject로 인스턴스화합니다.
            // UnityObject가 GameObject로 캐스팅 가능하면 인스턴스 반환, 불가능하면 null 반환.
            return Instantiate(source) as GameObject;
        }
    }
}
