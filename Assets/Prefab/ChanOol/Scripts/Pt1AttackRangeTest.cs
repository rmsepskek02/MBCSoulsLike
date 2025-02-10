using UnityEngine;

public class Pt1AttackRangeTest : MonoBehaviour
{
    public float growSpeed = 2f; // ���� �ӵ�
    private bool isGrowing = false; // ���� ������ Ȯ��

    private Vector3 originalScale; // �ʱ� ũ��
    private Vector3 targetScale; // ��ǥ ũ��

    // ���̵� ȿ��
    private Material material;
    [SerializeField] private float startAlpha = 1f; // ���� ���� ��
    [SerializeField] private float targetAlpha = 0f; // ��ǥ ���� ��
    [SerializeField] private float fadeSpeed = 0.5f; // �ʴ� ���� �ӵ�
    private bool isFading = true; // ���� �� ��ȯ Ȱ��ȭ ����

    // ��ä�� �޽� ���� ����
    private Mesh mesh;
    private MeshFilter meshFilter;
    private int segmentCount = 30; // ��ä���� ������ ���׸�Ʈ �� (������ ���� �ﰢ���� ���� ����)

    private void Start()
    {
        // ���� ũ�⸦ �ʱ� ũ��� ����
        originalScale = transform.localScale;

        StartGrowing(transform.localScale, 150f);

        // ���̵� ȿ��
        material = GetComponent<MeshRenderer>().material;
        Color color = material.color;
        color.a = startAlpha;
        material.color = color;

        // �޽� ���Ϳ� �޽� ����
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    private void Update()
    {
        UpdateScale();

        // �� Ŀ���� �����ð� �� ������Ʈ ����
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

            Destroy(gameObject, 2f);
        }
    }

    public void StartGrowing(Vector3 StartScale, float Range)
    {
        // ��ǥ ũ�� ����
        targetScale = new Vector3(StartScale.x * Range, StartScale.y, StartScale.z * Range);

        // ���� ����
        isGrowing = true;
    }

    public void UpdateScale()
    {
        if (isGrowing)
        {
            // �ε巴�� ũ�� ����
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, growSpeed * Time.deltaTime);

            // ��ǥ ũ�⿡ �����ߴ��� Ȯ��
            if (Vector3.Distance(transform.localScale, targetScale) < 0.01f)
            {
                isGrowing = false; // ���� ����
                Debug.Log("Reached target size!");
            }

            // ��ä�� ��� ������Ʈ
            UpdateMesh();
        }
    }

    private void UpdateMesh()
    {
        float radius = transform.localScale.x; // ������ ���� (x�� ũ�⸦ ���)
        float angleStep = 180f / segmentCount; // ���� ������ ������ (180�� ��ä���̹Ƿ�)

        // �޽� �ʱ�ȭ
        Vector3[] vertices = new Vector3[segmentCount + 2]; // +2�� �߽ɰ� �� ���� ����
        int[] triangles = new int[segmentCount * 3]; // �� ���׸�Ʈ���� 3���� �ﰢ�� �ε���
        Vector2[] uvs = new Vector2[segmentCount + 2];

        // �߽��� ����
        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f); // UV ��ǥ

        // ������ ���� ������ ���
        for (int i = 0; i < segmentCount; i++)
        {
            float angle = angleStep * i;
            float radian = Mathf.Deg2Rad * angle;
            float x = Mathf.Cos(radian) * radius;
            float z = Mathf.Sin(radian) * radius;

            vertices[i + 1] = new Vector3(x, 0, z); // ��ä�� ������
            uvs[i + 1] = new Vector2((x / radius) * 0.5f + 0.5f, (z / radius) * 0.5f + 0.5f); // UV ��ǥ

            // �ﰢ�� ���� (��ä���� �̷�� �ﰢ����)
            int triIndex = i * 3;
            triangles[triIndex] = 0; // �߽���
            triangles[triIndex + 1] = i + 1;
            triangles[triIndex + 2] = i + 2;
        }

        // ������ �������� �ﰢ���� ó��
        triangles[triangles.Length - 3] = 0;
        triangles[triangles.Length - 2] = segmentCount;
        triangles[triangles.Length - 1] = 1;

        // �޽��� ������Ʈ�� ���ؽ�, �ﰢ��, UV ��ǥ �Ҵ�
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // �޽��� ��ְ� �ٿ�� ������Ʈ
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}

