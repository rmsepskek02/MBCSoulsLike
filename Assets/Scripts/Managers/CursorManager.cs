using UnityEngine;

namespace BS.Managers
{
    /// <summary>
    /// Cursor ���� Script
    /// </summary>
    public class CursorManager : MonoBehaviour
    {
        #region Variables

        #endregion
        void Start()
        {
            // ������ ���۵� �� Ŀ���� ���̰� ����
            SetCursorVisibility();
        }

        void Update()
        {

        }

        // Ŀ���� ǥ�� ���¸� �����ϴ� �޼���
        public void SetCursorVisibility()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
