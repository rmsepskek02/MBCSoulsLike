using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Pt1AttackRange : MonoBehaviour
{
    public float angle = 45f; // 부채꼴의 각도
    public float radius = 5f; // 부채꼴의 반지름
    public int segments = 20; // 부채꼴의 세그먼트 (선분 수)
    public Vector3 startScale = new Vector3(1f, 1f, 1f); // 초기 스케일
    public Vector3 targetScale = new Vector3(2f, 2f, 2f); // 목표 스케일
    public float scaleSpeed = 1f; // 스케일 변경 속도

    // 인스펙터에서 마테리얼을 할당할 수 있도록 public으로 설정
    public Material material;

    // 페이드 효과
    [SerializeField] private float startAlpha = 1f; // 시작 알파 값
    [SerializeField] private float targetAlpha = 0f; // 목표 알파 값
    [SerializeField] private float fadeSpeed = 0.5f; // 초당 변경 속도
    private bool isFading = true; // 알파 값 전환 활성화 여부

    // 스케일이 목표값에 도달했는지 확인하는 변수
    public bool isGrowing { get; private set; }

    private void Start()
    {
        // 초기 스케일 설정
        transform.localScale = startScale;

        CreateFanShape();

        // 페이드 효과
        material = GetComponent<MeshRenderer>().material;
        Color color = material.color;
        color.a = startAlpha;
        material.color = color;

        // isGrowing 초기화
        isGrowing = true;
    }

    private void Update()
    {
        // 스케일 점진적으로 변경
        if (transform.localScale != targetScale)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, scaleSpeed * Time.deltaTime);
        }
        else
        {
            // 목표 스케일에 도달하면 isGrowing을 false로 설정
            if (isGrowing)
            {
                isGrowing = false; // 스케일이 목표값에 도달했을 때 false로 설정
            }
        }

        if (isGrowing == false)
        {
            // 페이드 효과
            if (isFading)
            {
                // 알파 값 전환 로직
                Color color = material.color;
                color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
                material.color = color;

                // 목표 알파 값에 도달하면 전환 멈춤
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
        // Mesh 초기화
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 2]; // 원점 + 세그먼트 개수
        int[] triangles = new int[segments * 3]; // 각 세그먼트마다 2 삼각형 (각 세그먼트마다 3 인덱스)
        Vector2[] uv = new Vector2[vertices.Length];

        // 중앙점 (0,0,0) 추가
        vertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0.5f);

        // 각도를 기준으로 세그먼트 배치
        float angleStep = angle / segments; // 각 세그먼트 간의 각도
        for (int i = 0; i < segments; i++)
        {
            float angleInRadians = Mathf.Deg2Rad * (i * angleStep);
            float x = Mathf.Cos(angleInRadians) * radius;
            float y = Mathf.Sin(angleInRadians) * radius;

            // 각 세그먼트의 끝 점을 계산
            vertices[i + 1] = new Vector3(x, 0f, y);
            uv[i + 1] = new Vector2((x / radius + 1f) / 2f, (y / radius + 1f) / 2f);
        }

        // 원래의 끝 점 추가 (마지막 세그먼트의 끝 점)
        vertices[segments + 1] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, 0f, Mathf.Sin(Mathf.Deg2Rad * angle) * radius);

        // 삼각형 인덱스 설정
        for (int i = 0; i < segments; i++)
        {
            int triIndex = i * 3;
            triangles[triIndex] = 0;
            triangles[triIndex + 1] = i + 1;
            triangles[triIndex + 2] = (i + 2) % (segments + 1); // 원점으로부터 이어지는 삼각형
        }

        // Mesh에 설정
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        // Mesh의 노멀 계산
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // MeshFilter와 MeshRenderer에 할당
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (material != null)
        {
            meshRenderer.material = material; // 인스펙터에서 할당된 material을 사용
        }
    }
}
