using UnityEngine;

public class Pt3AttackRange : MonoBehaviour
{
    // ���̵� ȿ��
    private Material material;
    [SerializeField] private float startAlpha = 1f;  // ���� ���� ��
    [SerializeField] private float targetAlpha = 0f; // ��ǥ ���� ��
    [SerializeField] private float fadeSpeed = 0.5f; // �ʴ� ���� �ӵ�
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;

        // ���� ���� �� ����
        Color color = material.color;
        color.a = startAlpha;
        material.color = color;

        Destroy(gameObject, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        Color color = material.color;
        color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
        material.color = color;

        // ��ǥ ���� ���� �����ϸ� Update ��Ȱ��ȭ
        if (Mathf.Approximately(color.a, targetAlpha))
        {
            enabled = false;
        }
    }
}
