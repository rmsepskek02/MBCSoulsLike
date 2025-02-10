using UnityEngine;

public class SetTransparency : MonoBehaviour
{
    public float transparency = 0.3f; // ������ ����

    void Start()
    {
        SetChildrenTransparency(transform, transparency);
    }

    void SetChildrenTransparency(Transform parent, float transparency)
    {
        // �θ� ������Ʈ�� �ڽĵ鿡 ���� �ݺ�
        foreach (Transform child in parent)
        {
            // �ڽ��� Renderer�� ������ ���� ���� ����
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Material�� �����Ͽ� ����
                Material material = renderer.material;

                // �⺻ ���̴����� ������ �����ϴ� ��� (_Color ������Ƽ�� ���� ��)
                if (material.HasProperty("_Color"))
                {
                    Color color = material.color;
                    color.a = transparency;  // ���� �� ����
                    material.color = color;

                    // ���̴��� ������ ��� ���� (Transparent�� ����)
                    material.SetFloat("_Mode", 3); // Transparent ���� ����
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000; // Transparency �켱���� ����
                }
            }

            // �ڽ� ������Ʈ�� ��������� ó��
            SetChildrenTransparency(child, transparency);
        }
    }
}
