using UnityEngine;
using UnityEngine.UI;

namespace BS.UI
{
    public class FadeStart : MonoBehaviour
    {
        #region Variables
        private Material material;
        private Image image;
        private float timer = 0f; // 0에서 1로 증가할 변수
        #endregion

        private void Start()
        {
            // UI Image 컴포넌트 가져오기
            image = GetComponent<Image>();
            if (image != null)
            {
                if (image.material == null)
                {
                    Debug.LogError("[ERROR] Image에 할당된 Material이 없습니다.");
                    return;
                }

                material = new Material(image.material); // 복제해서 사용
                image.material = material; // 새로운 Material 적용

                // 초기화
                material.SetFloat("_SetTimer", 0f);
                timer = 0f;
            }
        }

        private void Update()
        {
            if (material != null)
            {
                // 1초 동안 0에서 1로 증가
                timer += Time.deltaTime;
                timer = Mathf.Clamp01(timer); // 0~1 범위 제한
                material.SetFloat("_SetTimer", timer);
            }
        }
    }
}
