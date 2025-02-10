using UnityEngine;

namespace BS.Managers
{
    /// <summary>
    /// Cursor 관련 Script
    /// </summary>
    public class CursorManager : MonoBehaviour
    {
        #region Variables

        #endregion
        void Start()
        {
            // 게임이 시작될 때 커서를 보이게 설정
            SetCursorVisibility();
        }

        void Update()
        {

        }

        // 커서의 표시 상태를 설정하는 메서드
        public void SetCursorVisibility()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
