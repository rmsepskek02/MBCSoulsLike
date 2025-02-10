using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Pt1AttackRange : MonoBehaviour
{
    public float angle = 45f; // ��ä���� ����
    public float radius = 5f; // ��ä���� ������
    public int segments = 20; // ��ä���� ���׸�Ʈ (���� ��)
    public Vector3 startScale = new Vector3(1f, 1f, 1f); // �ʱ� ������
    public Vector3 targetScale = new Vector3(2f, 2f, 2f); // ��ǥ ������
    public float scaleSpeed = 1f; // ������ ���� �ӵ�

    // �ν����Ϳ��� ���׸����� �Ҵ��� �� �ֵ��� public���� ����
    public Material material;

    // ���̵� ȿ��
    [SerializeField] private float startAlpha = 1f; // ���� ���� ��
    [SerializeField] private float targetAlpha = 0f; // ��ǥ ���� ��
    [SerializeField] private float fadeSpeed = 0.5f; // �ʴ� ���� �ӵ�
    private bool isFading = true; // ���� �� ��ȯ Ȱ��ȭ ����

    // �������� ��ǥ���� �����ߴ��� Ȯ���ϴ� ����
    public bool isGrowing { get; private set; }

    private void Start()
    {
        // �ʱ� ������ ����
        transform.localScale = startScale;

        CreateFanShape();

        // ���̵� ȿ��
        material = GetComponent<MeshRenderer>().material;
        Color color = material.color;
        color.a = startAlpha;
        material.color = color;

        // isGrowing �ʱ�ȭ
        isGrowing = true;
    }

    private void Update()
    {
        // ������ ���������� ����
        if (transform.localScale != targetScale)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, scaleSpeed * Time.deltaTime);
        }
        else
        {
            // ��ǥ �����Ͽ� �����ϸ� isGrowing�� false�� ����
            if (isGrowing)
            {
                isGrowing = false; // �������� ��ǥ���� �������� �� false�� ����
            }
        }

        if (isGrowing == false)
        {
            // ���̵� ȿ��
            if (isFading)
            {
                // ���� �� ��ȯ ����
                Color color = material.color;
                color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
                material.color = color;

                // ��ǥ ���� ���� �����ϸ� ��ȯ ����
                if (Mathf.Approximately(color.a, targetAlpha))
                {
                    isFading = false;
                }
            }
            Destroy(gameObject, 1.2f);
        }
    }

    private void CreateFanShape()
    {
        // Mesh �ʱ�ȭ
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 2]; // ���� + ���׸�Ʈ ����
        int[] triangles = new int[segments * 3]; // �� ���׸�Ʈ���� 2 �ﰢ�� (�� ���׸�Ʈ���� 3 �ε���)
        Vector2[] uv = new Vector2[vertices.Length];

        // �߾��� (0,0,0) �߰�
        vertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0.5f);

        // ������ �������� ���׸�Ʈ ��ġ
        float angleStep = angle / segments; // �� ���׸�Ʈ ���� ����
        for (int i = 0; i < segments; i++)
        {
            float angleInRadians = Mathf.Deg2Rad * (i * angleStep);
            float x = Mathf.Cos(angleInRadians) * radius;
            float y = Mathf.Sin(angleInRadians) * radius;

            // �� ���׸�Ʈ�� �� ���� ���
            vertices[i + 1] = new Vector3(x, 0f, y);
            uv[i + 1] = new Vector2((x / radius + 1f) / 2f, (y / radius + 1f) / 2f);
        }

        // ������ �� �� �߰� (������ ���׸�Ʈ�� �� ��)
        vertices[segments + 1] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, 0f, Mathf.Sin(Mathf.Deg2Rad * angle) * radius);

        // �ﰢ�� �ε��� ����
        for (int i = 0; i < segments; i++)
        {
            int triIndex = i * 3;
            triangles[triIndex] = 0;
            triangles[triIndex + 1] = i + 1;
            triangles[triIndex + 2] = (i + 2) % (segments + 1); // �������κ��� �̾����� �ﰢ��
        }

        // Mesh�� ����
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        // Mesh�� ��� ���
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // MeshFilter�� MeshRenderer�� �Ҵ�
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (material != null)
        {
            meshRenderer.material = material; // �ν����Ϳ��� �Ҵ�� material�� ���
        }
    }
}
