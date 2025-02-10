using UnityEngine;
using System.Collections;

namespace BS.Title
{
    public class TitleCamera : MonoBehaviour
    {
        #region Variables
        public Camera mainCamera;
        public float startFOV = 60f;
        public float endFOV = 20f;
        public float zoomDuration = 2f;
        #endregion
        public IEnumerator ZoomIn()
        {
            float elapsedTime = 0f;
            float startFOV = mainCamera.fieldOfView;

            while (elapsedTime < zoomDuration)
            {
                float t = elapsedTime / zoomDuration;
                mainCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            mainCamera.fieldOfView = endFOV;
        }
    }
}