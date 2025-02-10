using UnityEngine;

public class SetTransparency : MonoBehaviour
{
    public float transparency = 0.3f; // 설정할 투명도

    void Start()
    {
        SetChildrenTransparency(transform, transparency);
    }

    void SetChildrenTransparency(Transform parent, float transparency)
    {
        // 부모 오브젝트의 자식들에 대해 반복
        foreach (Transform child in parent)
        {
            // 자식의 Renderer가 있으면 알파 값을 변경
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Material을 복사하여 변경
                Material material = renderer.material;

                // 기본 쉐이더에서 투명도를 지원하는 경우 (_Color 프로퍼티가 있을 때)
                if (material.HasProperty("_Color"))
                {
                    Color color = material.color;
                    color.a = transparency;  // 알파 값 변경
                    material.color = color;

                    // 쉐이더의 렌더링 모드 변경 (Transparent로 설정)
                    material.SetFloat("_Mode", 3); // Transparent 모드로 설정
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000; // Transparency 우선순위 설정
                }
            }

            // 자식 오브젝트도 재귀적으로 처리
            SetChildrenTransparency(child, transparency);
        }
    }
}
